using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Sonar.AutoSwitch.Services;
using Sonar.AutoSwitch.ViewModels;

namespace Sonar.AutoSwitch.Pages;

public partial class Home : UserControl
{
    public Home()
    {
        InitializeComponent();
        DataContext = HomeViewModel.LoadHomeViewModel();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}