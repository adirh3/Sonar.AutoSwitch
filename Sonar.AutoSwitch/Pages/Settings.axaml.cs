using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch.Pages;

public partial class Settings : UserControl
{
    public Settings()
    {
        InitializeComponent();
        DataContext = StateManager.Instance.GetOrLoadState<SettingsViewModel>();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}