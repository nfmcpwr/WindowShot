using System.IO.Pipes;
using System.Text;
using WindowShot.Shared;

namespace WindowShotService
{
    internal class IpcServer
    {
        public async Task Run(CancellationToken token)
        {
            try
            {
                while (!token.IsCancellationRequested)
                {
                    NamedPipeServerStream server = new NamedPipeServerStream("WindowShot", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous);

                    await server.WaitForConnectionAsync(token);
                    Console.WriteLine("[IpcServer] Connected");
                    while (server.IsConnected)
                    {
                        byte[] buffer = new byte[128];
                        int count = await server.ReadAsync(buffer, 0, buffer.Length, token);

                        if (count == 0)
                        {
                            break;
                        }

                        switch (Encoding.UTF8.GetString(buffer, 0, count))
                        {
                            case "StopService":
                                await Worker.TokenSource.CancelAsync();
                                break;

                            case "QueryMode":
                                byte[] wbuf = Encoding.UTF8.GetBytes(Worker.Config!.CaptureMode.ToString());
                                await server.WriteAsync(wbuf, 0, wbuf.Length, token);
                                break;

                            case "SetWindowMode":
                                Worker.Config!.CaptureMode = CaptureMode.Window;
                                break;

                            case "SetScreenMode":
                                Worker.Config!.CaptureMode = CaptureMode.Screen;
                                break;
                        }
                    }

                    Console.WriteLine("[IpcServer] Disconnected");
                    server.Disconnect();
                    server.Close();
                    await server.DisposeAsync();
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[IpcServer] Cancel requested");
            }
        }
    }
}