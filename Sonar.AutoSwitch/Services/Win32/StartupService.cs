using System;
using System.IO;
using System.Reflection;
using Microsoft.Win32;

namespace Sonar.AutoSwitch.Services.Win32;

public class StartupService
{
    public static void RegisterInStartup(bool isChecked)
    {
        RegistryKey registryKey = Registry.CurrentUser.OpenSubKey
            ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        if (isChecked)
            registryKey.SetValue("Sonar.AutoSwitch",
                Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Sonar.AutoSwitch.exe"));
        else
            try
            {
                registryKey.DeleteValue("Sonar.AutoSwitch");
            }
            catch (Exception)
            {
                // ignored
            }
    }
}