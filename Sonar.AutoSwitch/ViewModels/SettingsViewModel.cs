using Avalonia.Controls;
using Avalonia.Threading;
using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.Services.Win32;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;
namespace Sonar.AutoSwitch.ViewModels;

public class SettingsViewModel : ViewModelBase
{
    private bool _enabled = true;
    private bool _startAtStartup = true;
    private int? _selectedMonitorId;
    private MonitorOption? _selectedMonitor;

    public SettingsViewModel()
    {
        var screens = new Window().Screens.All;
        List<MonitorOption> monitors = new() { new MonitorOption("Any", null) };

        for (int i = 0; i < screens.Count; i++)
        {
            var screen = screens[i];
            // Create a name, marking the primary screen
            var name = $"Display {i + 1} ({screen.Bounds.Width}x{screen.Bounds.Height})";
            if (screen.IsPrimary)
                name += " [Primary]";

            monitors.Add(new MonitorOption(name, screen));
        }

        AvailableMonitors = new ObservableCollection<MonitorOption>(monitors);
        Dispatcher.UIThread.Post(() =>
        {
            if (_selectedMonitorId.HasValue)
            {
                SelectedMonitor = monitors.Find(m => m.Id == _selectedMonitorId.Value) ?? monitors[0];
            }
            else
            {
                SelectedMonitor = monitors[0];
            }
        });
    }

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

    public int? SelectedMonitorId
    {
        get => _selectedMonitorId;
        set
        {
            if (_selectedMonitorId == value) return;
            _selectedMonitorId = value;
            OnPropertyChanged();
        }
    }

    [JsonIgnore]
    public ObservableCollection<MonitorOption> AvailableMonitors { get; }

    [JsonIgnore]
    public MonitorOption? SelectedMonitor
    {
        get => _selectedMonitor;
        set
        {
            if (_selectedMonitor == value) return;
            _selectedMonitor = value;
            _selectedMonitorId = value?.Id;
            OnPropertyChanged();
        }
    }

    protected override void OnPropertyChanged(string? propertyName = null)
    {
        base.OnPropertyChanged(propertyName);
        StateManager.Instance.SaveState<SettingsViewModel>();
    }
}