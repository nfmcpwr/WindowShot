using System.ComponentModel;
using System.Runtime.InteropServices;
using WindowShot.Service.Interop;
using WindowShot.Shared;

namespace WindowShot.Service
{
    internal class Worker : BackgroundService
    {
        private readonly ILogger<Worker>      _Logger;
        private          HHOOK                _HKeyboardHook;
        private readonly KeyboardHookCallback _KeyboardHookCallback;
        private          Thread?              _HookThread;
        private          IpcServer            _Server;

        public static Config?                 Config;
        public static CancellationTokenSource TokenSource;

        public Worker(ILogger<Worker> logger)
        {
            this._Logger = logger;
            this._KeyboardHookCallback = KeyboardCallback.KeyboardCallbackHandler;
            this._Server = new IpcServer();

            TokenSource = new CancellationTokenSource();
            Config = Config.Load(Path.Combine(Environment.CurrentDirectory, "Config.json"));

            if (Config == null)
            {
                this._Logger.LogError("Failed to load config");

                Config = Config.DefaultConfig;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this._HookThread = new Thread(() =>
            {
                try
                {
                    Hook(TokenSource.Token);
                }
                catch (Win32Exception e)
                {
                    Environment.Exit(-1);
                }
            })
            {
                IsBackground = true,
                Name = "WindowShotService",
            };

            this._HookThread.Start();

            await this._Server.Run(TokenSource.Token);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await TokenSource.CancelAsync();

            await base.StopAsync(cancellationToken);
        }

        private void Hook(CancellationToken token)
        {
            this._HKeyboardHook = User32.SetWindowsHookEx(
                Consts.WH_KEYBOARD_LL,
                Marshal.GetFunctionPointerForDelegate(this._KeyboardHookCallback),
                IntPtr.Zero,
                0);

            if (this._HKeyboardHook == IntPtr.Zero)
            {
                throw new Win32Exception($"Hook create error: 0x{Kernel32.GetLastError():X8}");
            }

            Console.WriteLine("[Hook] Hook created");

            while (!token.IsCancellationRequested)
            {
                while (User32.PeekMessage(out MSG msg, IntPtr.Zero, 0, 0, Consts.PM_REMOVE))
                {
                    User32.TranslateMessage(ref msg);
                    User32.DispatchMessage(ref msg);
                }
            }

            bool b = User32.UnhookWindowsHookEx(this._HKeyboardHook);

            if (!b)
            {
                throw new Win32Exception($"Hook remove error: 0x{Kernel32.GetLastError():X8}");
            }

            Console.WriteLine("[Hook] Hook removed");

            Environment.Exit(0);
        }
    }
}