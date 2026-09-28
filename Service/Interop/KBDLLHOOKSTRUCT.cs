using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal class KBDLLHOOKSTRUCT
    {
        public DWORD   vkCode;
        public DWORD   scanCode;
        public DWORD   flags;
        public DWORD   time;
        public UIntPtr dwExtraInfo;
    }
}