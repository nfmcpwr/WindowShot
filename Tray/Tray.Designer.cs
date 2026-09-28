namespace WindowShot.Tray
{
    partial class Tray
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tray));
            trayIcon = new NotifyIcon(components);
            trayMenu = new ContextMenuStrip(components);
            statusText = new ToolStripMenuItem();
            captureMode = new ToolStripMenuItem();
            modeWindow = new ToolStripMenuItem();
            modeScreen = new ToolStripMenuItem();
            exitButton = new ToolStripMenuItem();
            trayMenu.SuspendLayout();
            SuspendLayout();
            // 
            // trayIcon
            // 
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;
            trayIcon.BalloonTipText = "text";
            trayIcon.BalloonTipTitle = "test";
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Icon = (Icon)resources.GetObject("trayIcon.Icon");
            trayIcon.Text = "notifyIcon1";
            trayIcon.Visible = true;
            trayIcon.MouseClick += trayIcon_MouseClick;
            // 
            // trayMenu
            // 
            trayMenu.ImageScalingSize = new Size(24, 24);
            trayMenu.Items.AddRange(new ToolStripItem[] { statusText, captureMode, exitButton });
            trayMenu.Name = "contextMenuStrip1";
            trayMenu.Size = new Size(143, 100);
            // 
            // statusText
            // 
            statusText.Enabled = false;
            statusText.Name = "statusText";
            statusText.Size = new Size(142, 32);
            statusText.Text = "Status: ";
            // 
            // captureMode
            // 
            captureMode.DropDownItems.AddRange(new ToolStripItem[] { modeWindow, modeScreen });
            captureMode.Enabled = false;
            captureMode.Name = "captureMode";
            captureMode.Size = new Size(142, 32);
            captureMode.Text = "Mode";
            // 
            // modeWindow
            // 
            modeWindow.Name = "modeWindow";
            modeWindow.Size = new Size(180, 34);
            modeWindow.Text = "Window";
            modeWindow.Click += modeWindow_Click;
            // 
            // modeScreen
            // 
            modeScreen.Name = "modeScreen";
            modeScreen.Size = new Size(180, 34);
            modeScreen.Text = "Screen";
            modeScreen.Click += modeScreen_Click;
            // 
            // exitButton
            // 
            exitButton.Name = "exitButton";
            exitButton.Size = new Size(142, 32);
            exitButton.Text = "Exit";
            exitButton.Click += exitButton_Click;
            // 
            // Tray
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Tray";
            ShowInTaskbar = false;
            Text = "WindowShot Tray";
            WindowState = FormWindowState.Minimized;
            trayMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private ToolStripMenuItem statusText;
        private ToolStripMenuItem captureMode;
        private ToolStripMenuItem exitButton;
        private ToolStripMenuItem modeWindow;
        private ToolStripMenuItem modeScreen;
    }
}
