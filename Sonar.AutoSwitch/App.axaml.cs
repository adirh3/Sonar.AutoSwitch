using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.Services.Win32;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch
{
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

    public partial class App : Application
    {
        public override void Initialize()
        {
            DataContext = new AppViewModel();
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.ShutdownMode = ShutdownMode.OnExplicitShutdown;
                desktop.MainWindow = new MainWindow();
            }

            var settingsViewModel = StateManager.Instance.GetOrLoadState<SettingsViewModel>();
            if (settingsViewModel.Enabled)
                AutoSwitchService.Instance.ToggleEnabled(settingsViewModel.Enabled);
            if (settingsViewModel.StartAtStartup)
                StartupService.RegisterInStartup(true);
            base.OnFrameworkInitializationCompleted();
        }
    }
}