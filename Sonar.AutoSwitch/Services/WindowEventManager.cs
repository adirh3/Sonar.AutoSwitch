using Sonar.AutoSwitch.Services.Win32;

namespace Sonar.AutoSwitch.Services;

public static class WindowEventManager
{
    public static IWindowEventManager Instance { get; } = new Win32WindowEventManager();
}