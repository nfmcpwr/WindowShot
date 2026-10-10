using System.Runtime.InteropServices;

namespace SettingsUI.Interop
{
    internal class Shell32
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsUserAnAdmin();
    }
}