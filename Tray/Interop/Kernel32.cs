using System.Runtime.InteropServices;

namespace WindowShot.Tray.Interop
{
    internal class Kernel32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern HANDLE OpenProcess(
            DWORD dwDesiredAccess,
            bool  bInheritHandle,
            DWORD dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(HANDLE hObject);

        [DllImport("kernel32.dll")]
        public static extern uint GetLastError();
    }
}