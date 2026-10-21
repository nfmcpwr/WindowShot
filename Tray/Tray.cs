using System.Diagnostics;
using WindowShot.Resources;
using WindowShot.Shared;

namespace WindowShot.Tray
{
    public partial class Tray : ApplicationContext
    {
        private CaptureMode? _Mode;

        public Tray()
        {
            InitializeComponent();

            this._Mode = null;

            CheckStatus();
        }

        private async Task CheckStatus()
        {
            this.trayIcon!.Text = "WindowShot";
            this.statusText!.Text = $"{Resource.StatusPrefix}: ";

            if (!IpcClient.IsServiceRunning())
            {
                this.captureMode!.Enabled = false;
                this.statusText!.Text += Resource.StatusStopped;

                return;
            }

            this.captureMode!.Enabled = true;
            this.statusText!.Text += Resource.StatusRunning;

            try
            {
                this._Mode = await IpcClient.QueryMode();
            }
            catch (Exception e)
            {
                Error.ShowDialog(e);
            }

            switch (this._Mode)
            {
                case CaptureMode.Screen:
                    this.modeScreen!.Checked = true;
                    break;

                case CaptureMode.Window:
                    this.modeWindow!.Checked = true;
                    break;
            }

            if (IpcClient.IsServiceElevated())
            {
                this.trayIcon!.Text += $" [{Resource.Admin}]";
                this.statusText!.Text += $" [{Resource.Admin}]";
            }
        }


        private async void modeWindow_Click(object? sender, EventArgs e)
        {
            if (this.modeScreen!.Checked)
            {
                this.modeScreen!.Checked = false;
                this.modeWindow!.Checked = true;

                this._Mode = CaptureMode.Window;

                try
                {
                    await IpcClient.UpdateMode(this._Mode);
                }
                catch (Exception ex)
                {
                    Error.ShowDialog(ex);
                }
            }
        }

        private async void modeScreen_Click(object? sender, EventArgs e)
        {
            if (this.modeWindow!.Checked)
            {
                this.modeWindow!.Checked = false;
                this.modeScreen!.Checked = true;

                this._Mode = CaptureMode.Screen;

                try
                {
                    await IpcClient.UpdateMode(this._Mode);
                }
                catch (Exception ex)
                {
                    Error.ShowDialog(ex);
                }
            }
        }

        private async void exitButton_Click(object? sender, EventArgs e)
        {
            if (IpcClient.IsServiceRunning())
            {
                try
                {
                    await IpcClient.StopService();
                }
                catch (Exception ex)
                {
                    Error.ShowDialog(ex);
                }
            }

            ExitThread();
        }

        private async void trayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                try
                {
                    await CheckStatus();
                }
                catch (Exception ex)
                {
                    Error.ShowDialog(ex);
                }
            }
        }

        private void openSettings_Click(object? sender, EventArgs e)
        {
            try
            {
                Process.Start(Path.Combine(Environment.CurrentDirectory, "WSSettingsUI.exe"));
            }
            catch (Exception ex)
            {
                Error.ShowDialog(ex);
            }
        }
    }
}