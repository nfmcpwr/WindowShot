using System.Runtime.InteropServices;

namespace WindowShot.Tray.Interop
{
    internal class Advapi32
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool OpenProcessToken(
            HANDLE     ProcessHandle,
            DWORD      DesiredAccess,
            out HANDLE TokenHandle);

        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetTokenInformation(
            HANDLE                  TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            out IntPtr              TokenInformation,
            DWORD                   TokenInformationLength,
            out DWORD               ReturnLength);
    }
}