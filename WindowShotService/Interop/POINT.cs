using System.Runtime.InteropServices;

namespace WindowShotService.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }
}