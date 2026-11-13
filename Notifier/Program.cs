using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;
using Notifier.Interop;
using Notifier.Resources;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using WindowShot.Shared;

namespace Notifier
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (1 < Process.GetProcessesByName("WSNotifier").Length)
            {
                Log.Error("Notifier", "Notifier process already running");
                return;
            }

            if (Shell32.IsUserAnAdmin())
            {
                return;
            }

            Environment.CurrentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            bool exit = false;

            PipeSecurity sec = new PipeSecurity();
            sec.AddAccessRule(new PipeAccessRule(
                new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null),
                PipeAccessRights.ReadWrite,
                AccessControlType.Allow));

            while (!exit)
            {
                NamedPipeServerStream server = NamedPipeServerStreamAcl.Create(
                    "WSNotifier",
                    PipeDirection.InOut,
                    1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous,
                    0,
                    0,
                    sec);

                server.WaitForConnection();
                while (server.IsConnected)
                {
                    byte[] buffer = new byte[128];
                    int count = server.Read(buffer, 0, 128);

                    string msg = Encoding.UTF8.GetString(buffer, 0, count);
                    if (msg == "Exit")
                    {
                        Log.Info("Notifier", "Exit request received");
                        exit = true;
                    }
                    else if (!string.IsNullOrEmpty(msg))
                    {
                        AppNotificationManager manager = AppNotificationManager.Default;
                        manager.Register();
                        manager.Show(new AppNotificationBuilder()
                            .AddText("WindowShot")
                            .AddText(Resource.Saved.Replace("{0}", msg))
                            .BuildNotification());
                        Task.Delay(100).Wait();
                        manager.Unregister();

                        Log.Info("Notifier", $"Post notification: {msg}");
                    }
                }

                server.Disconnect();
                server.Close();
                server.Dispose();
            }
        }
    }
}