using System.Runtime.InteropServices;

namespace WindowShot.Service.Interop
{
    [ComImport]
    [Guid("A9B3D012-3DF2-4EE3-B8D1-8695F457D3C1")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface Direct3DDxgiInterfaceAccess
    {
        [PreserveSig]
        HRESULT GetInterface(in Guid iid, out IntPtr result);
    }
}