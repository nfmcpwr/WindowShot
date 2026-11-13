using System.Diagnostics;
using System.IO.Pipes;
using System.Text;
using WindowShot.Shared;

namespace WindowShot.Service
{
    internal class Notification
    {
        public static async Task Send(string msg)
        {
            if (Process.GetProcessesByName("WSNotifier").Length == 0)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer.exe",
                    Arguments = Path.Combine(Environment.CurrentDirectory, "WSNotifier.exe"),
                });
            }

            NamedPipeClientStream client = new NamedPipeClientStream("WSNotifier");

            try
            {
                await client.ConnectAsync(5000);

                if (client.IsConnected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(msg);
                    await client.WriteAsync(buffer, 0, buffer.Length);
                }
                else
                {
                    Log.Error("Service", "Failed to connect to Notifier");
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