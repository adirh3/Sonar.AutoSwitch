using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text.Json.Serialization;
using Sonar.AutoSwitch.Services;

namespace Sonar.AutoSwitch.ViewModels;

public class HomeViewModel : ViewModelBase
{
    private ObservableCollection<AutoSwitchProfileViewModel> _autoSwitchProfiles =
        new() { new AutoSwitchProfileViewModel() };

    private SonarGamingConfiguration _defaultSonarGamingConfiguration = new(null, "unset");
    private AutoSwitchProfileViewModel? _selectedAutoSwitchProfileViewModel;
    private SonarGamingConfiguration _activeProfile;

    public HomeViewModel()
    {
        SelectedAutoSwitchProfileViewModel = AutoSwitchProfiles.FirstOrDefault();
        AutoSwitchProfiles.CollectionChanged += AutoSwitchProfilesOnCollectionChanged;
    }

    public SonarGamingConfiguration DefaultSonarGamingConfiguration
    {
        get => _defaultSonarGamingConfiguration;
        set
        {
            if (Equals(value, _defaultSonarGamingConfiguration)) return;
            _defaultSonarGamingConfiguration = value;
            OnPropertyChanged(nameof(DefaultSonarGamingConfiguration));
        }
    }

    public ObservableCollection<AutoSwitchProfileViewModel> AutoSwitchProfiles
    {
        get => _autoSwitchProfiles;
        set
        {
            _autoSwitchProfiles = value;
            SelectedAutoSwitchProfileViewModel = AutoSwitchProfiles.FirstOrDefault();
        }
    }

    [JsonIgnore]
    public AutoSwitchProfileViewModel? SelectedAutoSwitchProfileViewModel
    {
        get => _selectedAutoSwitchProfileViewModel;
        set
        {
            if (Equals(value, _selectedAutoSwitchProfileViewModel)) return;
            _selectedAutoSwitchProfileViewModel = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public SonarGamingConfiguration ActiveProfile
    {
        get => _activeProfile;
        set
        {
            if (Equals(value, _activeProfile)) return;
            _activeProfile = value;
            OnPropertyChanged();
        }
    }

    public static HomeViewModel LoadHomeViewModel()
    {
        _ = AutoSwitchProfilesDatabase.Instance.LoadDatabaseAsync();
        bool firstLoad = !StateManager.Instance.CheckStateExists<HomeViewModel>();
        var homeViewModel = StateManager.Instance.GetOrLoadState<HomeViewModel>();
        var steelSeriesSonarService = SteelSeriesSonarService.Instance;
        string selectedConfigId = steelSeriesSonarService.GetSelectedGamingConfiguration();
        var activeProfile = steelSeriesSonarService.GetGamingConfigurations()
            .FirstOrDefault(gc => gc.Id == selectedConfigId);
        if (firstLoad)
        {
            homeViewModel.DefaultSonarGamingConfiguration =
                activeProfile ?? homeViewModel.DefaultSonarGamingConfiguration;
        }

        homeViewModel.ActiveProfile = activeProfile ?? homeViewModel.DefaultSonarGamingConfiguration;
        return homeViewModel;
    }

    public void RemoveAutoSwitchProfile()
    {
        if (SelectedAutoSwitchProfileViewModel != null) AutoSwitchProfiles.Remove(SelectedAutoSwitchProfileViewModel);
        if (!AutoSwitchProfiles.Any())
            AutoSwitchProfiles.Add(new AutoSwitchProfileViewModel());
        SelectedAutoSwitchProfileViewModel = AutoSwitchProfiles.FirstOrDefault();
    }

    public void AddAutoSwitchProfile()
    {
        AutoSwitchProfiles.Add(new AutoSwitchProfileViewModel());
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        StateManager.Instance.SaveState<HomeViewModel>();
    }

    private void AutoSwitchProfilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        StateManager.Instance.SaveState<HomeViewModel>();
    }
}