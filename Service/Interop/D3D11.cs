using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    internal class D3D11
    {
        [DllImport("d3d11.dll", SetLastError = true)]
        public static extern HRESULT CreateDirect3D11DeviceFromDXGIDevice(IntPtr dxgiDevice, out IntPtr graphicsDevice);
    }
}