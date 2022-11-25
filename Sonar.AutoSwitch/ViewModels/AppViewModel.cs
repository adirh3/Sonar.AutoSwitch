using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace Sonar.AutoSwitch.ViewModels;

public class AppViewModel
{
    public void Open()
    {
        if (Application.Current!.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
        {
            lifetime.MainWindow ??= new MainWindow();
            lifetime.MainWindow.Show();
        }
    }

    public void Exit()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime lifetime)
            lifetime.Shutdown();
    }
}