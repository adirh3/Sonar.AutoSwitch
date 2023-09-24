using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.Services.Win32;

namespace Sonar.AutoSwitch.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool _enabled = true;
    private bool _startAtStartup = true;

    public bool Enabled
    {
        get => _enabled;
        set
        {
            if (value == _enabled) return;
            _enabled = value;
            AutoSwitchService.Instance.ToggleEnabled(_enabled);
            OnPropertyChanged();
        }
    }

    public bool StartAtStartup
    {
        get => _startAtStartup;
        set
        {
            if (value == _startAtStartup) return;
            _startAtStartup = value;
            StartupService.RegisterInStartup(_startAtStartup);
            OnPropertyChanged();
        }
    }

    public bool UseGithubConfigs { get; set; } = true;

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        StateManager.Instance.SaveState<SettingsViewModel>();
    }
}