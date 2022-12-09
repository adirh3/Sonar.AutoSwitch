using System;
using System.Linq;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch.Services;

public class AutoSwitchService
{
    private readonly HomeViewModel _homeViewModel;
    private SonarGamingConfiguration _selectedGamingConfiguration;

    public AutoSwitchService()
    {
        WindowEventManager.Instance.ForegroundWindowChanged += InstanceOnForegroundWindowChanged;
        _homeViewModel = StateManager.Instance.GetOrLoadState<HomeViewModel>()!;
    }

    public static AutoSwitchService Instance { get; } = new();

    public void ToggleEnabled(bool enable)
    {
        if (enable)
            WindowEventManager.Instance.SubscribeToWindowEvents();
        else
            WindowEventManager.Instance.UnsubscribeToWindowsEvents();
    }

    private void InstanceOnForegroundWindowChanged(object? sender, WindowInfo e)
    {
        string? windowExeName = e.ExeName;
        if (string.Equals(windowExeName, "explorer", StringComparison.OrdinalIgnoreCase))
            return;

        AutoSwitchProfileViewModel? autoSwitchProfileViewModel =
            _homeViewModel.AutoSwitchProfiles.FirstOrDefault(p =>
                (string.IsNullOrEmpty(p.ExeName) ||
                 string.Equals(p.ExeName, windowExeName, StringComparison.OrdinalIgnoreCase)) &&
                (string.IsNullOrEmpty(p.Title) || e.Title.Contains(p.Title, StringComparison.OrdinalIgnoreCase)));
        SonarGamingConfiguration? sonarGamingConfiguration = autoSwitchProfileViewModel?.SonarGamingConfiguration;
        sonarGamingConfiguration ??= _homeViewModel.DefaultSonarGamingConfiguration;
        if (string.IsNullOrEmpty(sonarGamingConfiguration.Id) ||
            _selectedGamingConfiguration == sonarGamingConfiguration)
            return;

        string selectedGamingConfigurationId =
            SteelSeriesSonarService.Instance.GetSelectedGamingConfiguration();
        _selectedGamingConfiguration = sonarGamingConfiguration;
        if (sonarGamingConfiguration.Id == selectedGamingConfigurationId)
            return;
        _ = SteelSeriesSonarService.Instance.ChangeSelectedGamingConfiguration(sonarGamingConfiguration);
    }
}