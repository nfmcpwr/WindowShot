using WindowShot.Resources;
using WindowShot.Shared;

namespace WindowShot.Tray
{
    public partial class Tray : Form
    {
        private CaptureMode? _Mode;

        public Tray()
        {
            InitializeComponent();

            this._Mode = null;
        }

        private async Task CheckStatus()
        {
            if (!IpcClient.IsServiceRunning())
            {
                goto Skip;
            }

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
                    this.modeScreen.Checked = true;
                    break;

                case CaptureMode.Window:
                    this.modeWindow.Checked = true;
                    break;
            }

            Skip:

            this.captureMode.Enabled = IpcClient.IsServiceRunning();
            this.statusText.Text = $"{Resource.StatusPrefix}: " + (IpcClient.IsServiceRunning() ? Resource.StatusRunning : Resource.StatusStopped);
        }


        private async void modeWindow_Click(object sender, EventArgs e)
        {
            if (this.modeScreen.Checked)
            {
                this.modeScreen.Checked = false;
                this.modeWindow.Checked = true;

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

        private async void modeScreen_Click(object sender, EventArgs e)
        {
            if (this.modeWindow.Checked)
            {
                this.modeWindow.Checked = false;
                this.modeScreen.Checked = true;

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

        private async void exitButton_Click(object sender, EventArgs e)
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

            Close();
        }

        private async void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this._Mode == null)
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
    }
}