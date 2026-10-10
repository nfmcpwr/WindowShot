using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.ApplicationModel.Resources;
using SettingsUI.Interop;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics;
using WindowShot.Shared;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SettingsUI
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private Config?        _Config;
        private ResourceLoader _ResourceLoader;
        private bool           _SkipEvent;

        public MainWindow()
        {
            InitializeComponent();

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;

            this.AppWindow.SetPresenter(presenter);
            this.AppWindow.ResizeClient(new SizeInt32(720, 520));

            this._ResourceLoader = new ResourceLoader();
            this._SkipEvent = false;

            if (Shell32.IsUserAnAdmin())
            {
                this.Title += $" [{this._ResourceLoader.GetString("Admin")}]";
            }

            this._Config = Config.Load(Config.ConfigPath);
            if (this._Config == null)
            {
                Log.Warning("SettingsUI", "Load default config");
                this._Config = Config.DefaultConfig;
                this._Config.Save(Config.ConfigPath);
            }

            foreach (string s in Enum.GetNames<VirtualKeyCode>())
            {
                this.ShortcutKey.Items.Add(s);
            }

            this.Startup.IsOn = this._Config.Startup;
            this.Admin.IsEnabled = this.Startup.IsOn;
            this.Admin.IsOn = this._Config.Admin;
            this.Mode.SelectedIndex = (int)this._Config.CaptureMode;
            this.ShortcutKey.SelectedItem = this._Config.WindowShotKey.ToString();
            this.Method.SelectedIndex = (int)this._Config.CaptureMethod;
        }

        private void Startup_OnToggled(object sender, RoutedEventArgs e)
        {
            this._Config!.Startup = this.Startup.IsOn;
            this.Admin.IsEnabled = this.Startup.IsOn;
        }

        private void Admin_OnToggled(object sender, RoutedEventArgs e)
        {
            if (!Shell32.IsUserAnAdmin())
            {
                if (this._SkipEvent)
                {
                    this._SkipEvent = false;
                    return;
                }

                Process? p = null;
                try
                {
                    p = Process.Start(new ProcessStartInfo
                    {
                        FileName = Path.Combine(Environment.CurrentDirectory, "WSSettingsUI.exe"),
                        Verb = "RunAs",
                        UseShellExecute = true,
                    });
                }
                catch (Win32Exception)
                {
                }
                finally
                {
                    if (p != null)
                    {
                        Close();
                    }

                    this._SkipEvent = true;
                    this.Admin.IsOn = false;
                }

                return;
            }

            this._Config!.Admin = this.Admin.IsOn;
        }

        private void Mode_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._Config!.CaptureMode = (CaptureMode)this.Mode.SelectedIndex;
        }

        private void ShortcutKey_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._Config!.WindowShotKey = Enum.Parse<VirtualKeyCode>(this.ShortcutKey.SelectedItem.ToString()!);
            this._Config!.ScreenShotKey = Enum.Parse<VirtualKeyCode>(this.ShortcutKey.SelectedItem.ToString()!);
        }

        private void Method_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._Config!.CaptureMethod = (CaptureMethod)this.Method.SelectedIndex;
        }

        private async void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            this.Apply.IsEnabled = false;
            this._Config!.Save(Config.ConfigPath);

            try
            {
                SettingsUI.Startup.Update(this.Startup.IsOn, this.Admin.IsOn && this.Admin.IsEnabled);
            }
            catch (Exception exception)
            {
                Log.Error("SettingsUI", exception.ToString());
            }

            await ReloadConfig();

            Close();
        }

        private async Task ReloadConfig()
        {
            if (Process.GetProcessesByName("WindowShotService").Length == 0)
            {
                Log.Error("SettingsUI", "[ReloadConfig] Service not running");
                await new ContentDialog
                {
                    Title = this._ResourceLoader.GetString("Error"),
                    Content = this._ResourceLoader.GetString("ServiceNotRunning"),
                    CloseButtonText = "OK",
                    XamlRoot = this.Content.XamlRoot,
                }.ShowAsync();
                return;
            }

            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                await client.ConnectAsync(5000);

                if (client.IsConnected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes("ReloadConfig");
                    await client.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            catch (Exception e)
            {
                Log.Error("SettingsUI", e.Message);
                Log.Error("SettingsUI", e.StackTrace);
            }
            finally
            {
                client.Close();
                await client.DisposeAsync();
            }
        }
    }
}