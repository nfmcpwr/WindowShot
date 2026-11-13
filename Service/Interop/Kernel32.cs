using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll")]
        public static extern DWORD GetLastError();
    }
}