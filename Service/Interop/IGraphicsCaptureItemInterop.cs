using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [ComImport]
    [Guid("3628E81B-3CAC-4C60-B7F4-23CE0E0C3356")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IGraphicsCaptureItemInterop
    {
        IntPtr CreateForWindow(HWND      window,  in Guid iid);
        IntPtr CreateForMonitor(HMONITOR monitor, in Guid iid);
    }
}