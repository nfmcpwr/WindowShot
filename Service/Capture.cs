using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using WindowShot.Service.Interop;
using WindowShot.Shared;

namespace WindowShot.Service
{
    internal static partial class Capture
    {
        public static bool SaveCapture(CaptureMode mode, CaptureMethod method)
        {
            Log.Info("Service", $"[SaveCapture] Mode: {mode.ToString()}, Method: {method.ToString()}");

            Bitmap? result = null;

            switch (mode)
            {
                case CaptureMode.Screen:
                    HMONITOR hmon = User32.MonitorFromWindow(
                        User32.GetForegroundWindow(),
                        Consts.MONITOR_DEFAULTTONEAREST);

                    MONITORINFOEX info = new MONITORINFOEX();
                    info.cbSize = (DWORD)Marshal.SizeOf<MONITORINFOEX>();

                    if (!User32.GetMonitorInfo(hmon, ref info))
                    {
                        Log.Error("Service", $"GetMonitorInfo error: 0x{Kernel32.GetLastError():X8}");
                        return false;
                    }

                    switch (method)
                    {
                        case CaptureMethod.BitBlt:
                            result = BitBlt(IntPtr.Zero, info.rcMonitor);
                            break;

                        case CaptureMethod.PrintWindow:
                            result = PrintWindow(IntPtr.Zero, info.rcMonitor);
                            break;
                    }

                    break;

                case CaptureMode.Window:
                    HWND hwnd = User32.GetForegroundWindow();
                    if (!User32.GetClientRect(hwnd, out RECT rect))
                    {
                        Log.Error("Service", $"GetClientRect error: 0x{Kernel32.GetLastError():X8}");
                        return false;
                    }

                    switch (method)
                    {
                        case CaptureMethod.BitBlt:
                            result = BitBlt(hwnd, rect);
                            break;

                        case CaptureMethod.PrintWindow:
                            result = PrintWindow(hwnd, rect);
                            break;
                    }

                    break;
            }

            if (result == null)
            {
                return false;
            }

            if (!Directory.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "WindowShot")))
            {
                Directory.CreateDirectory(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "WindowShot"));
            }

            result.Save(
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures),
                    "WindowShot",
                    $"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss")}.png"),
                ImageFormat.Png);

            return true;
        }
    }
}