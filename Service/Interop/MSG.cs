using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct MSG
    {
        public HWND   hwnd;
        public uint   message;
        public WPARAM wParam;
        public LPARAM lParam;
        public DWORD  time;
        public POINT  pt;
        public DWORD  lPrivate;
    }
}