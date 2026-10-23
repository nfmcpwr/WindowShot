using System;
using System.Runtime.InteropServices;

namespace SettingsUI.Interop
{
    internal class User32
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetDpiForWindow(IntPtr hwnd);
    }
}