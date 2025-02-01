using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Air.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void ThemeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RequestedThemeVariant =
            RequestedThemeVariant == ThemeVariant.Light
            ? ThemeVariant.Dark
            : ThemeVariant.Light;
    }
}