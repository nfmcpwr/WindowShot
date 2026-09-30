using Newtonsoft.Json;

namespace WindowShot.Shared
{
    public record class Config
    {
        public bool           Startup       { get; set; }
        public CaptureMode    CaptureMode   { get; set; }
        public VirtualKeyCode WindowShotKey { get; set; }
        public VirtualKeyCode ScreenShotKey { get; set; }
        public CaptureMethod  CaptureMethod { get; set; }

        public static readonly string ConfigPath = Path.Combine(Environment.CurrentDirectory, "Config.json");

        public static Config? Load(string path)
        {
            try
            {
                return JsonConvert.DeserializeObject<Config>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public void Save(string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
        }

        public static readonly Config DefaultConfig = new Config
        {
            Startup = true,
            CaptureMode = CaptureMode.Window,
            WindowShotKey = VirtualKeyCode.ImeConvert,
            ScreenShotKey = VirtualKeyCode.ImeConvert,
            CaptureMethod = CaptureMethod.BitBlt,
        };
    }
}