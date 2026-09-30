using System.Drawing;
using WindowShot.Service.Interop;
using WindowShot.Shared;

namespace WindowShot.Service
{
    internal static partial class Capture
    {
        private static Bitmap? BitBlt(HWND hwnd, RECT targetRect)
        {
            HDC hsrcDC = User32.GetDC(hwnd);
            if (hsrcDC == IntPtr.Zero)
            {
                Log.Error("Service", $"GetDC error: 0x{Kernel32.GetLastError():X8}");

                return null;
            }

            HDC hMemoryDC = Gdi32.CreateCompatibleDC(hsrcDC);
            if (hMemoryDC == IntPtr.Zero)
            {
                Log.Error("Service", $"CreateCompatibleDC error: 0x{Kernel32.GetLastError():X8}");

                User32.ReleaseDC(hwnd, hsrcDC);

                return null;
            }

            HBITMAP hBitmap = Gdi32.CreateCompatibleBitmap(
                hsrcDC,
                targetRect.right - targetRect.left,
                targetRect.bottom - targetRect.top);

            if (hBitmap == IntPtr.Zero)
            {
                Log.Error("Service", $"CreateCompatibleBitmap error: 0x{Kernel32.GetLastError():X8}");

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
                Log.Error("Service", $"BitBlt error: 0x{Kernel32.GetLastError():X8}");

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