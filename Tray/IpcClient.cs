using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using WindowShot.Shared;

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
    }
}