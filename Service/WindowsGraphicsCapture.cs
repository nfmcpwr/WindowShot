using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Vortice.Direct3D;
using Vortice.Direct3D11;
using Vortice.DXGI;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Graphics.DirectX.Direct3D11;
using WindowShot.Service.Interop;
using WindowShot.Shared;
using WinRT;

namespace WindowShot.Service
{
    internal static partial class Capture
    {
        private static readonly Guid GraphicsCaptureItemIid = new Guid("79C3F95B-31F7-4EC2-A464-632EF5D30760");

        public static async Task<Bitmap?> WindowsGraphicsCapture(HWND window, HMONITOR monitor)
        {
            if (!GraphicsCaptureSession.IsSupported())
            {
                Log.Error("Service", "Windows.Graphics.Capture API not supported");
                return null;
            }

            GraphicsCaptureItem item;
            if (monitor != IntPtr.Zero)
            {
                item = CreateItemForMonitor(monitor);
            }
            else
            {
                item = CreateItemForWindow(window);
            }

            SizeInt32 size = item.Size;
            if (size.Width <= 0 || size.Height <= 0)
            {
                Log.Error("Service", "Target window has no capturable content");
                return null;
            }

            using ID3D11Device d3dDevice = CreateDevice(out ID3D11DeviceContext d3dContext);
            using (d3dContext)
            {
                IDirect3DDevice? winrtDevice = CreateWinRtDevice(d3dDevice);
                if (winrtDevice == null)
                {
                    return null;
                }

                Direct3D11CaptureFramePool framePool = Direct3D11CaptureFramePool.CreateFreeThreaded(
                    winrtDevice,
                    DirectXPixelFormat.B8G8R8A8UIntNormalized,
                    1,
                    size);

                GraphicsCaptureSession session = framePool.CreateCaptureSession(item);
                session.IsCursorCaptureEnabled = false;

                TaskCompletionSource<Direct3D11CaptureFrame> firstFrame = new TaskCompletionSource<Direct3D11CaptureFrame>(TaskCreationOptions.RunContinuationsAsynchronously);
                TypedEventHandler<Direct3D11CaptureFramePool, object> frameArrived = (sender, args) =>
                {
                    try
                    {
                        Direct3D11CaptureFrame? next = framePool.TryGetNextFrame();
                        if (next != null && !firstFrame.TrySetResult(next))
                        {
                            next.Dispose();
                        }
                    }
                    catch (Exception exception)
                    {
                        firstFrame.TrySetException(exception);
                    }
                };

                framePool.FrameArrived += frameArrived;
                try
                {
                    session.StartCapture();
                    Direct3D11CaptureFrame frame = await firstFrame.Task;
                    try
                    {
                        return CopyFrameToBitmap(frame, d3dDevice, d3dContext);
                    }
                    finally
                    {
                        DisposeIfSupported(frame);
                    }
                }
                finally
                {
                    framePool.FrameArrived -= frameArrived;
                    DisposeIfSupported(session);
                    DisposeIfSupported(framePool);
                    DisposeIfSupported(winrtDevice);
                }
            }
        }

        private static ID3D11Device CreateDevice(out ID3D11DeviceContext context)
        {
            ID3D11Device device = Vortice.Direct3D11.D3D11.D3D11CreateDevice(
                DriverType.Hardware,
                DeviceCreationFlags.BgraSupport);

            context = device.ImmediateContext;

            return device;
        }

        private static IDirect3DDevice? CreateWinRtDevice(ID3D11Device d3dDevice)
        {
            using IDXGIDevice dxgiDevice = d3dDevice.QueryInterface<IDXGIDevice>();

            HRESULT result = Interop.D3D11.CreateDirect3D11DeviceFromDXGIDevice(dxgiDevice.NativePointer, out IntPtr devicePointer);
            if (result < 0)
            {
                Log.Error("Service", $"CreateDirect3D11DeviceFromDXGIDevice error: 0x{result:X8}");

                return null;
            }

            try
            {
                return MarshalInterface<IDirect3DDevice>.FromAbi(devicePointer)!;
            }
            finally
            {
                if (devicePointer != IntPtr.Zero)
                {
                    Marshal.Release(devicePointer);
                }
            }
        }

        private static Bitmap? CopyFrameToBitmap(
            Direct3D11CaptureFrame frame,
            ID3D11Device           device,
            ID3D11DeviceContext    context)
        {
            SizeInt32 size = frame.ContentSize;
            if (size.Width <= 0 || size.Height <= 0)
            {
                Log.Error("Service", "The captured frame has no content");

                return null;
            }

            using ID3D11Texture2D? source = GetTexture(frame.Surface);
            if (source == null)
            {
                return null;
            }

            Texture2DDescription sourceDescription = source.Description;

            if (sourceDescription.Format != Format.B8G8R8A8_UNorm ||
                sourceDescription.Width < size.Width ||
                sourceDescription.Height < size.Height)
            {
                Log.Error("Service", "The captured frame has an unsupported texture layout");

                return null;
            }

            Texture2DDescription stagingDescription = new Texture2DDescription(
                Format.B8G8R8A8_UNorm,
                sourceDescription.Width,
                sourceDescription.Height,
                1,
                1,
                BindFlags.None,
                ResourceUsage.Staging,
                CpuAccessFlags.Read,
                1,
                0,
                ResourceOptionFlags.None);

            using ID3D11Texture2D staging = device.CreateTexture2D(stagingDescription);

            context.CopyResource(staging, source);
            MappedSubresource mapped = context.Map(staging, 0, MapMode.Read, Vortice.Direct3D11.MapFlags.None);

            try
            {
                return CopyMappedPixelsToBitmap(mapped, size);
            }
            finally
            {
                context.Unmap(staging, 0);
            }
        }

        private static ID3D11Texture2D? GetTexture(IDirect3DSurface surface)
        {
            Direct3DDxgiInterfaceAccess access = surface.As<Direct3DDxgiInterfaceAccess>();

            try
            {
                HRESULT result = access.GetInterface(typeof(ID3D11Texture2D).GUID, out IntPtr texturePointer);
                if (result < 0)
                {
                    Log.Error("Service", $"GetInterface error: 0x{result:X8}");

                    return null;
                }

                return new ID3D11Texture2D(texturePointer);
            }
            finally
            {
                Marshal.ReleaseComObject(access);
            }
        }

        private static Bitmap? CopyMappedPixelsToBitmap(MappedSubresource mapped, SizeInt32 size)
        {
            const int bytesPerPixel = 4;
            int sourceRowBytes = checked(size.Width * bytesPerPixel);
            byte[] row = GC.AllocateUninitializedArray<byte>(sourceRowBytes);
            Bitmap bitmap = new Bitmap(size.Width, size.Height, PixelFormat.Format32bppArgb);
            BitmapData? bitmapData = null;

            try
            {
                bitmapData = bitmap.LockBits(
                    new Rectangle(0, 0, size.Width, size.Height),
                    ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);

                for (int y = 0; y < size.Height; y++)
                {
                    IntPtr sourceRow = IntPtr.Add(mapped.DataPointer, checked(y * (int)mapped.RowPitch));
                    IntPtr destinationRow = IntPtr.Add(bitmapData.Scan0, checked(y * bitmapData.Stride));

                    Marshal.Copy(sourceRow, row, 0, sourceRowBytes);
                    Marshal.Copy(row, 0, destinationRow, sourceRowBytes);
                }

                return bitmap;
            }
            catch (Exception e)
            {
                Log.Error("Service", $"[CopyMappedPixelsToBitmap] {e.Message}");
                bitmap.Dispose();

                return null;
            }
            finally
            {
                if (bitmapData != null)
                {
                    bitmap.UnlockBits(bitmapData);
                }
            }
        }

        private static GraphicsCaptureItem CreateItemForWindow(HWND window)
        {
            IGraphicsCaptureItemInterop factory = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
            IntPtr itemPointer = factory.CreateForWindow(window, in GraphicsCaptureItemIid);

            try
            {
                return ABI.Windows.Graphics.Capture.GraphicsCaptureItem.FromAbi(itemPointer)!;
            }
            finally
            {
                if (itemPointer != IntPtr.Zero)
                {
                    Marshal.Release(itemPointer);
                }
            }
        }

        private static GraphicsCaptureItem CreateItemForMonitor(HMONITOR monitor)
        {
            IGraphicsCaptureItemInterop factory = GraphicsCaptureItem.As<IGraphicsCaptureItemInterop>();
            IntPtr itemPointer = factory.CreateForMonitor(monitor, in GraphicsCaptureItemIid);

            try
            {
                return ABI.Windows.Graphics.Capture.GraphicsCaptureItem.FromAbi(itemPointer)!;
            }
            finally
            {
                if (itemPointer != IntPtr.Zero)
                {
                    Marshal.Release(itemPointer);
                }
            }
        }

        private static void DisposeIfSupported(object value)
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
    }
}