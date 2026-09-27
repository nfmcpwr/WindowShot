using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
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
        private Config? _Config;

        public MainWindow()
        {
            InitializeComponent();

            OverlappedPresenter presenter = OverlappedPresenter.Create();
            presenter.IsResizable = false;

            this.AppWindow.SetPresenter(presenter);
            this.AppWindow.ResizeClient(new SizeInt32(720, 480));

            this._Config = Config.Load(Config.ConfigPath);
            if (this._Config == null)
            {
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

        private void Apply_OnClick(object sender, RoutedEventArgs e)
        {
            this._Config!.Save(Config.ConfigPath);
            Close();
        }
    }
}