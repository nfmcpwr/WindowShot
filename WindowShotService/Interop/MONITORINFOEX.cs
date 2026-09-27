using System.Runtime.InteropServices;

namespace WindowShotService.Interop
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    internal struct MONITORINFOEX
    {
        public DWORD cbSize;
        public RECT  rcMonitor;
        public RECT  rcWork;
        public DWORD dwFlags;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Consts.CCHDEVICENAME)]
        public string szDevice;
    }
}