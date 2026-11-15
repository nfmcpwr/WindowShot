using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    internal class Kernel32
    {
        public static int GetLastError()
        {
            return Marshal.GetLastWin32Error();
        }
    }
}