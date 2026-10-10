using Microsoft.Win32;
using Microsoft.Win32.TaskScheduler;
using SettingsUI.Interop;
using System;
using System.IO;

namespace SettingsUI
{
    internal class Startup
    {
        public static void Update(bool enabled, bool isAdmin)
        {
            if (isAdmin)
            {
                UpdateRegistry(false);
                UpdateTaskScheduler(enabled);

                return;
            }

            UpdateRegistry(enabled);
            UpdateTaskScheduler(false);
        }

        private static void UpdateRegistry(bool enabled)
        {
            RegistryKey runKey = Registry.CurrentUser.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true)!;
            if (enabled)
            {
                runKey.SetValue("WindowShot", Path.Combine(Environment.CurrentDirectory, "WindowShotService.exe"), RegistryValueKind.String);
            }
            else
            {
                runKey.DeleteValue("WindowShot", false);
            }
        }

        private static void UpdateTaskScheduler(bool enabled)
        {
            if (!Shell32.IsUserAnAdmin())
            {
                return;
            }

            if (enabled)
            {
                if (TaskService.Instance.FindTask("WindowShot", false) != null)
                {
                    return;
                }

                TaskDefinition def = TaskService.Instance.NewTask();
                def.Triggers.Add(new LogonTrigger());
                def.Actions.Add(new ExecAction(Path.Combine(Environment.CurrentDirectory, "WindowShotService.exe")));
                def.Settings.Compatibility = TaskCompatibility.V2_3;
                def.Settings.AllowHardTerminate = false;
                def.Principal.RunLevel = TaskRunLevel.Highest;
                def.RegistrationInfo.Author = "WindowShot";

                TaskService.Instance.RootFolder.RegisterTaskDefinition("WindowShot", def);

                return;
            }

            TaskService.Instance.RootFolder.DeleteTask("WindowShot", false);
        }
    }
}