using System.Diagnostics;
using System.IO.Pipes;
using System.Runtime.InteropServices;
using System.Text;
using WindowShot.Shared;
using WindowShot.Tray.Interop;

namespace WindowShot.Tray
{
    internal class IpcClient
    {
        public static async Task UpdateMode(CaptureMode? mode)
        {
            if (mode == null)
            {
                return;
            }

            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                if (!IsServiceRunning())
                {
                    throw new Exception(ErrorCode.ServiceNotRunning.ToString());
                }

                await client.ConnectAsync(5000);

                if (client.IsConnected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes($"Set{mode.ToString()}Mode");
                    await client.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    throw new Exception(ErrorCode.ConnectionFailed.ToString());
                }
            }
            finally
            {
                client.Close();
                await client.DisposeAsync();
            }
        }

        public static async Task<CaptureMode> QueryMode()
        {
            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                if (!IsServiceRunning())
                {
                    throw new Exception(ErrorCode.ServiceNotRunning.ToString());
                }

                await client.ConnectAsync(5000);

                if (client.IsConnected)
                {
                    byte[] wbuf = Encoding.UTF8.GetBytes("QueryMode");
                    await client.WriteAsync(wbuf, 0, wbuf.Length);

                    byte[] buffer = new byte[128];
                    int count = await client.ReadAsync(buffer, 0, 128);

                    return Enum.Parse<CaptureMode>(Encoding.UTF8.GetString(buffer, 0, count));
                }
                else
                {
                    throw new Exception(ErrorCode.ConnectionFailed.ToString());
                }
            }
            finally
            {
                client.Close();
                await client.DisposeAsync();
            }
        }

        public static bool IsServiceRunning()
        {
            if (Process.GetProcessesByName("WindowShotService").Length == 0)
            {
                return false;
            }

            return true;
        }

        public static async Task StopService()
        {
            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                if (!IsServiceRunning())
                {
                    throw new Exception(ErrorCode.ServiceNotRunning.ToString());
                }

                await client.ConnectAsync(5000);

                if (client.IsConnected)
                {
                    byte[] wbuf = Encoding.UTF8.GetBytes("StopService");
                    await client.WriteAsync(wbuf, 0, wbuf.Length);
                }
                else
                {
                    throw new Exception(ErrorCode.ConnectionFailed.ToString());
                }
            }
            finally
            {
                client.Close();
                await client.DisposeAsync();
            }
        }

        public static Process? GetServiceProcess()
        {
            if (!IsServiceRunning())
            {
                return null;
            }

            return Process.GetProcessesByName("WindowShotService").FirstOrDefault();
        }

        public static bool IsServiceElevated()
        {
            if (!IsServiceRunning())
            {
                return false;
            }

            Process? sProcess = GetServiceProcess();
            if (sProcess == null)
            {
                return false;
            }

            HANDLE hProcess = Kernel32.OpenProcess(
                Consts.PROCESS_QUERY_LIMITED_INFORMATION,
                false,
                (DWORD)sProcess.Id);

            if (hProcess == IntPtr.Zero)
            {
                Log.Error("Tray", $"OpenProcess error: 0x{Kernel32.GetLastError():X8}");

                return false;
            }

            if (!Advapi32.OpenProcessToken(hProcess, 0x0008, out HANDLE hToken))
            {
                Log.Error("Tray", $"OpenProcessToken error: 0x{Kernel32.GetLastError():X8}");

                Kernel32.CloseHandle(hProcess);
                return false;
            }

            if (!Advapi32.GetTokenInformation(
                    hToken,
                    TOKEN_INFORMATION_CLASS.TokenElevation,
                    out IntPtr tokenElevation,
                    (DWORD)Marshal.SizeOf<TOKEN_ELEVATION>(),
                    out DWORD returnLength))
            {
                Log.Error("Tray", $"GetTokenInformation error: 0x{Kernel32.GetLastError():X8}");

                Kernel32.CloseHandle(hToken);
                Kernel32.CloseHandle(hProcess);
                return false;
            }

            Kernel32.CloseHandle(hToken);
            Kernel32.CloseHandle(hProcess);

            return tokenElevation.ToInt32() != 0;
        }
    }
}