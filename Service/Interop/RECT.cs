using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }
}