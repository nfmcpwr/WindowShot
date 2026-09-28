using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct POINT
    {
        public int x;
        public int y;
    }
}