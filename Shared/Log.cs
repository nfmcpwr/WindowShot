namespace WindowShot.Shared
{
    public class Log
    {
        private static readonly string LogDir = Path.Combine(Environment.CurrentDirectory, "Log");

        public static void Info(string name, string? msg)
        {
            Write(name, msg != null ? msg : "", LogType.Info);
        }

        public static void Warning(string name, string? msg)
        {
            Write(name, msg != null ? msg : "", LogType.Warning);
        }

        public static void Error(string name, string? msg)
        {
            Write(name, msg != null ? msg : "", LogType.Error);
        }

        private static void Write(string name, string msg, LogType type)
        {
            if (!Directory.Exists(LogDir))
            {
                Directory.CreateDirectory(LogDir);
            }

            File.AppendAllText(Path.Combine(LogDir, name + ".log"), string.Format(
                "[{0}][{1}] {2}",
                DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                type.ToString(),
                msg + Environment.NewLine));
        }
    }
}