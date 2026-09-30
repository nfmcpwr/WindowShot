using System.Runtime.InteropServices;
using WindowShot.Service.Interop;
using WindowShot.Shared;

namespace WindowShot.Service
{
    [UnmanagedFunctionPointer(CallingConvention.Winapi)]
    internal delegate LRESULT KeyboardHookCallback(int code, WPARAM wParam, LPARAM lParam);

    internal static class KeyboardCallback
    {
        public static LRESULT KeyboardCallbackHandler(int code, WPARAM wParam, LPARAM lParam)
        {
            if (code < 0 || wParam.ToUInt32() != Consts.WM_KEYDOWN)
            {
                return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            KBDLLHOOKSTRUCT? kbd = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
            if (kbd == null)
            {
                Log.Warning("Service", "[Callback] lParam null");
                return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }

            if (kbd.vkCode == (uint)Worker.Config!.WindowShotKey)
            {
                Log.Info("Service", "[Callback] Capture key pressed");
                Capture.SaveCapture(Worker.Config!.CaptureMode);
                return 1;
            }

            return User32.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
    }
}