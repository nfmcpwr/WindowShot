using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Win32;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
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

        public MainWindow()
        {
            InitializeComponent();

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = false;
            presenter.IsMaximizable = false;

            this.AppWindow.SetPresenter(presenter);
            this.AppWindow.ResizeClient(new SizeInt32(720, 480));

            this._ResourceLoader = new ResourceLoader();

            this._Config = Config.Load(Config.ConfigPath);
            if (this._Config == null)
            {
                Log.Warning("SettingsUI", "Load default config");
                this._Config = Config.DefaultConfig;
            }

            foreach (string s in Enum.GetNames<VirtualKeyCode>())
            {
                this.ShortcutKey.Items.Add(s);
            }

            this.Startup.IsOn = this._Config.Startup;
            this.Mode.SelectedIndex = (int)this._Config.CaptureMode;
            this.ShortcutKey.SelectedItem = this._Config.WindowShotKey.ToString();
        }

        private void Startup_OnToggled(object sender, RoutedEventArgs e)
        {
            this._Config!.Startup = this.Startup.IsOn;
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

        private async void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            this.Apply.IsEnabled = false;
            this._Config!.Save(Config.ConfigPath);

            await UpdateSettings();

            Close();
        }

        private async Task UpdateSettings()
        {
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;
            if (this._Config!.Startup)
            {
                runKey.SetValue("WindowShotService", Path.Combine(Environment.CurrentDirectory, "WindowShotService.exe"), RegistryValueKind.String);
            }
            else
            {
                runKey.DeleteValue("WindowShotService", false);
            }

            await ReloadConfig();
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