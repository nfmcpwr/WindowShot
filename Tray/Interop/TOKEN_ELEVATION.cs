using System.Runtime.InteropServices;

namespace WindowShot.Tray.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct TOKEN_ELEVATION
    {
        public DWORD TokenIsElevated;
    }
}