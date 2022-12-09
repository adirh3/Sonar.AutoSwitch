using Sonar.AutoSwitch.Services;

namespace Sonar.AutoSwitch.ViewModels;

public class AutoSwitchProfileViewModel : ViewModelBase
{
    private string _exeName = "MyGame";
    private SonarGamingConfiguration _sonarGamingConfiguration = new(null, "Unset");
    private string _title;

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value;
            OnPropertyChanged();
        }
    }

    public string ExeName
    {
        get => _exeName;
        set
        {
            if (value == _exeName) return;
            _exeName = value;
            OnPropertyChanged();
        }
    }

    public SonarGamingConfiguration SonarGamingConfiguration
    {
        get => _sonarGamingConfiguration;
        set
        {
            if (Equals(value, _sonarGamingConfiguration)) return;
            _sonarGamingConfiguration = value;
            OnPropertyChanged(nameof(SonarGamingConfiguration));
        }
    }

    public override string ToString()
    {
        return ExeName;
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        StateManager.Instance.SaveState<HomeViewModel>();
    }
}