using Sonar.AutoSwitch.Services;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
namespace Sonar.AutoSwitch.ViewModels;

public class AutoSwitchProfileViewModel : ViewModelBase
{
    private static readonly RegexOptions REGEX_OPTIONS = RegexOptions.IgnoreCase;
    private string _exeName = "MyGame";
    private string _title = "";
    private Regex _titleRegex = new Regex("", REGEX_OPTIONS);
    private SonarGamingConfiguration _sonarGamingConfiguration = new(null, "Unset");

    public string Title
    {
        get => _title;
        set
        {
            if (value == _title) return;
            _title = value ??= "";
            _titleRegex = new Regex(_title, REGEX_OPTIONS);
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayName)); // Notify that DisplayName has changed
        }
    }

    public bool MatchTitle(string title)
    {
        return _titleRegex.IsMatch(title);
    }

    public string ExeName
    {
        get => _exeName;
        set
        {
            if (value == _exeName) return;
            _exeName = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(DisplayName)); // Notify that DisplayName has changed
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

    public string DisplayName => string.IsNullOrWhiteSpace(Title) ? ExeName : Title;

    protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        StateManager.Instance.SaveState<HomeViewModel>();
    }

    public override string ToString()
    {
        return $"Title: {Title}, ExeName: {ExeName}, SonarGamingConfiguration: {SonarGamingConfiguration}, DisplayName: {DisplayName}";
    }
}
