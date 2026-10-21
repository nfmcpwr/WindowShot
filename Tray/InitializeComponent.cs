using WindowShot.Resources;

namespace WindowShot.Tray
{
    public partial class Tray
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.components != null)
            {
                this.components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tray));
            this.trayIcon = new NotifyIcon(this.components);
            this.trayMenu = new ContextMenuStrip(this.components);
            this.statusText = new ToolStripMenuItem();
            this.captureMode = new ToolStripMenuItem();
            this.modeWindow = new ToolStripMenuItem();
            this.modeScreen = new ToolStripMenuItem();
            this.openSettings = new ToolStripMenuItem();
            this.exitButton = new ToolStripMenuItem();
            this.trayMenu.SuspendLayout();
            // 
            // trayIcon
            // 
            this.trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            this.trayIcon.ContextMenuStrip = this.trayMenu;
            this.trayIcon.Icon = (Icon)resources.GetObject("trayIcon.Icon")!;
            this.trayIcon.Text = "WindowShot";
            this.trayIcon.Visible = true;
            this.trayIcon.MouseClick += trayIcon_MouseClick;
            // 
            // trayMenu
            // 
            this.trayMenu.ImageScalingSize = new Size(24, 24);
            this.trayMenu.Items.AddRange(new ToolStripItem[] { this.statusText, this.captureMode, this.openSettings, this.exitButton });
            this.trayMenu.Name = "contextMenuStrip1";
            this.trayMenu.Size = new Size(149, 132);
            // 
            // statusText
            // 
            this.statusText.Enabled = false;
            this.statusText.Name = "statusText";
            this.statusText.Size = new Size(148, 32);
            this.statusText.Text = "Status: ";
            // 
            // captureMode
            // 
            this.captureMode.DropDownItems.AddRange(new ToolStripItem[] { this.modeWindow, this.modeScreen });
            this.captureMode.Enabled = false;
            this.captureMode.Name = "captureMode";
            this.captureMode.Size = new Size(148, 32);
            this.captureMode.Text = Resource.ModeText;
            // 
            // modeWindow
            // 
            this.modeWindow.Name = "modeWindow";
            this.modeWindow.Size = new Size(180, 34);
            this.modeWindow.Text = Resource.WindowText;
            this.modeWindow.Click += modeWindow_Click;
            // 
            // modeScreen
            // 
            this.modeScreen.Name = "modeScreen";
            this.modeScreen.Size = new Size(180, 34);
            this.modeScreen.Text = Resource.ScreenText;
            this.modeScreen.Click += modeScreen_Click;
            // 
            // openSettings
            // 
            this.openSettings.Name = "openSettings";
            this.openSettings.Size = new Size(148, 32);
            this.openSettings.Text = Resource.SettingsText;
            this.openSettings.Click += openSettings_Click;
            // 
            // exitButton
            // 
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new Size(148, 32);
            this.exitButton.Text = Resource.ExitText;
            this.exitButton.Click += exitButton_Click;

            this.trayMenu.ResumeLayout(false);
        }

        #endregion

        private NotifyIcon?        trayIcon;
        private ContextMenuStrip?  trayMenu;
        private ToolStripMenuItem? statusText;
        private ToolStripMenuItem? captureMode;
        private ToolStripMenuItem? exitButton;
        private ToolStripMenuItem? modeWindow;
        private ToolStripMenuItem? modeScreen;
        private ToolStripMenuItem? openSettings;
    }
}