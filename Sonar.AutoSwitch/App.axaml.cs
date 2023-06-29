using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.Services.Win32;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch;

public class App : Application
{
    public override void Initialize()
    {
        DataContext = new AppViewModel();
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var firstLoad = !StateManager.Instance.CheckStateExists<SettingsViewModel>();
        var settingsViewModel = StateManager.Instance.GetOrLoadState<SettingsViewModel>();
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
            if (firstLoad)
            {
                desktop.MainWindow = new MainWindow();
                StateManager.Instance.SaveState<SettingsViewModel>();
            }
        }

        if (settingsViewModel.Enabled)
            AutoSwitchService.Instance.ToggleEnabled(settingsViewModel.Enabled);
        if (settingsViewModel.StartAtStartup)
            StartupService.RegisterInStartup(true);

        base.OnFrameworkInitializationCompleted();
    }
}