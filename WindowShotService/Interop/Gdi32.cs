using System.Runtime.InteropServices;

namespace WindowShotService.Interop
{
    internal class Gdi32
    {
        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool BitBlt(
            HDC   hdc,
            int   x,
            int   y,
            int   cx,
            int   cy,
            HDC   hdcSrc,
            int   x1,
            int   y1,
            DWORD rop);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern HBITMAP CreateCompatibleBitmap(
            HDC hdc,
            int cx,
            int cy);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern HDC CreateCompatibleDC(HDC hdc);

        [DllImport("gdi32.dll", SetLastError = true)]
        public static extern HGDIOBJ SelectObject(HDC hdc, HGDIOBJ h);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(HGDIOBJ ho);

        [DllImport("gdi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(HDC hdc);
    }
}