using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HMODULE GetModuleHandle(LPCSTR lpModuleName);

        [DllImport("kernel32.dll")]
        public static extern DWORD GetLastError();
    }
}