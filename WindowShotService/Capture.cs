using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using WindowShot.Shared;
using WindowShotService.Interop;

namespace WindowShotService
{
    internal static class Capture
    {
        public static bool SaveCapture(CaptureMode mode)
        {
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
                        return false;
                    }

                    result = BitBlt(IntPtr.Zero, info.rcMonitor);
                    break;

                case CaptureMode.Window:
                    HWND hwnd = User32.GetForegroundWindow();
                    if (!User32.GetClientRect(hwnd, out RECT rect))
                    {
                        return false;
                    }

                    result = BitBlt(hwnd, rect);
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

        private static Bitmap? BitBlt(HWND hwnd, RECT targetRect)
        {
            HDC hsrcDC = User32.GetDC(hwnd);
            if (hsrcDC == IntPtr.Zero)
            {
                Console.WriteLine("GetDC error: {0:X8}", Kernel32.GetLastError());

                return null;
            }

            HDC hMemoryDC = Gdi32.CreateCompatibleDC(hsrcDC);
            if (hMemoryDC == IntPtr.Zero)
            {
                Console.WriteLine("CreateCompatibleDC error: {0:X8}", Kernel32.GetLastError());

                User32.ReleaseDC(hwnd, hsrcDC);

                return null;
            }

            HBITMAP hBitmap = Gdi32.CreateCompatibleBitmap(
                hsrcDC,
                targetRect.right - targetRect.left,
                targetRect.bottom - targetRect.top);

            if (hBitmap == IntPtr.Zero)
            {
                Console.WriteLine("CreateCompatibleBitmap error: {0:X8}", Kernel32.GetLastError());

                User32.ReleaseDC(hwnd, hsrcDC);
                Gdi32.DeleteDC(hMemoryDC);

                return null;
            }

            HGDIOBJ hPrevObject = Gdi32.SelectObject(hMemoryDC, hBitmap);

            if (!Gdi32.BitBlt(
                    hMemoryDC,
                    0,
                    0,
                    targetRect.right - targetRect.left,
                    targetRect.bottom - targetRect.top,
                    hsrcDC,
                    targetRect.left,
                    targetRect.top,
                    Consts.SRCCOPY))
            {
                Console.WriteLine("BitBlt error: {0:X8}", Kernel32.GetLastError());

                Gdi32.SelectObject(hMemoryDC, hPrevObject);
                Gdi32.DeleteObject(hBitmap);
                Gdi32.DeleteDC(hMemoryDC);
                User32.ReleaseDC(hwnd, hsrcDC);

                return null;
            }

            Bitmap result = Bitmap.FromHbitmap(hBitmap);

            Gdi32.SelectObject(hMemoryDC, hPrevObject);
            Gdi32.DeleteObject(hBitmap);
            Gdi32.DeleteDC(hMemoryDC);
            User32.ReleaseDC(hwnd, hsrcDC);

            return result;
        }
    }
}