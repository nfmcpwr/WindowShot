using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    internal class User32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern HHOOK SetWindowsHookEx(
            int       idHook,
            HOOKPROC  lpfn,
            HINSTANCE hmod,
            DWORD     dwThreadId);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool UnhookWindowsHookEx(HHOOK hhk);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern LRESULT CallNextHookEx(
            HHOOK  hhk,
            int    nCode,
            WPARAM wParam,
            LPARAM lParam);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern HWND GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern HDC GetDC(HWND hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int ReleaseDC(HWND hWnd, HDC hDC);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(
            HWND hwnd,
            HDC  hdcBlt,
            uint nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetClientRect(
            HWND     hWnd,
            out RECT lpRect);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern HMONITOR MonitorFromWindow(HWND hwnd, DWORD dwFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetMonitorInfo(HMONITOR hMonitor, ref MONITORINFOEX lpmi);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PeekMessage(
            out MSG lpMsg,
            HWND    hWnd,
            uint    wMsgFilterMin,
            uint    wMsgFilterMax,
            uint    wRemoveMsg);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool TranslateMessage(ref MSG lpMsg);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern LRESULT DispatchMessage(ref MSG lpMsg);
    }
}