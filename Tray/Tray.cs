using System.IO.Pipes;
using System.Text;
using WindowShot.Shared;

namespace Tray
{
    public partial class Tray : Form
    {
        private CaptureMode? _Mode;
        private bool         _ServiceStatus;

        public Tray()
        {
            InitializeComponent();

            this._ServiceStatus = false;
            this._Mode = null;
        }

        private async Task CheckStatus()
        {
            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                await client.ConnectAsync(5000);
            }
            catch (TimeoutException)
            {
            }

            if (client.IsConnected)
            {
                this._ServiceStatus = true;

                byte[] wbuf = Encoding.UTF8.GetBytes("QueryMode");
                await client.WriteAsync(wbuf, 0, wbuf.Length);

                byte[] buffer = new byte[128];
                int count = await client.ReadAsync(buffer, 0, 128);

                this._Mode = Enum.Parse<CaptureMode>(Encoding.UTF8.GetString(buffer, 0, count));
            }
            else
            {
                MessageBox.Show("Failed to connect WindowShot service", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            client.Close();
            await client.DisposeAsync();

            switch (this._Mode)
            {
                case CaptureMode.Screen:
                    this.modeScreen.Checked = true;
                    break;

                case CaptureMode.Window:
                    this.modeWindow.Checked = true;
                    break;
            }

            this.captureMode.Enabled = this._ServiceStatus;
            this.statusText.Text += this._ServiceStatus ? "Running" : "Stopped";
        }

        private async Task UpdateMode()
        {
            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                await client.ConnectAsync(5000);
            }
            catch (TimeoutException)
            {
            }

            if (client.IsConnected)
            {
                byte[] buffer = Encoding.UTF8.GetBytes($"Set{this._Mode.ToString()}Mode");
                await client.WriteAsync(buffer, 0, buffer.Length);
            }

            client.Close();
            await client.DisposeAsync();
        }

        private async void modeWindow_Click(object sender, EventArgs e)
        {
            if (this.modeScreen.Checked)
            {
                this.modeScreen.Checked = false;
                this.modeWindow.Checked = true;

                this._Mode = CaptureMode.Window;

                await UpdateMode();
            }
        }

        private async void modeScreen_Click(object sender, EventArgs e)
        {
            if (this.modeWindow.Checked)
            {
                this.modeWindow.Checked = false;
                this.modeScreen.Checked = true;

                this._Mode = CaptureMode.Screen;

                await UpdateMode();
            }
        }

        private async void exitButton_Click(object sender, EventArgs e)
        {
            NamedPipeClientStream client = new NamedPipeClientStream("WindowShot");

            try
            {
                await client.ConnectAsync(5000);
            }
            catch (TimeoutException)
            {
            }

            if (client.IsConnected)
            {
                this._ServiceStatus = true;

                byte[] wbuf = Encoding.UTF8.GetBytes("StopService");
                await client.WriteAsync(wbuf, 0, wbuf.Length);
            }
            else
            {
                MessageBox.Show("Failed to connect WindowShot service", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            client.Close();
            await client.DisposeAsync();

            Close();
        }

        private async void trayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && this._Mode == null)
            {
                await CheckStatus();
            }
        }
    }
}