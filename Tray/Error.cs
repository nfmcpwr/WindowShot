using System.Diagnostics;
using WindowShot.Resources;
using WindowShot.Shared;

namespace WindowShot.Tray
{
    internal class Error
    {
        public static void ShowDialog(Exception e)
        {
            Log.Error("Tray", e.Message);
            Log.Error("Tray", e.StackTrace);

            if (!Enum.TryParse<ErrorCode>(e.Message, true, out ErrorCode code))
            {
                code = ErrorCode.Unknown;
            }

            switch (code)
            {
                case ErrorCode.ServiceNotRunning:
                    ShowMessageBox(Resource.ServiceNotRunning);
                    return;

                case ErrorCode.ConnectionFailed:
                    ShowMessageBox(Resource.ConnectionFailed);
                    return;

                case ErrorCode.Unknown:
                    ShowMessageBox(e.Message);
                    return;
            }
        }

        private static void ShowMessageBox(string msg)
        {
            MessageBox.Show(
                msg,
                $"{Process.GetCurrentProcess().MainModule!.FileName.Split('\\').Last()} - {Resource.Error}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}