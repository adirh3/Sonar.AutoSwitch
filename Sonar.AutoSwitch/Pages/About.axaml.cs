using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Sonar.AutoSwitch.Pages;

public partial class About : UserControl
{
    public About()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}