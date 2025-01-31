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

    private void OpenCreateRoomPanel_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void OpenJoinRoomPanel_OnClick(object? sender, RoutedEventArgs e)
    {
        JoinRoomPanel.IsVisible = true;
        MainPanel.IsVisible = false;
    }

    private void AudioSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.IsVisible = false;
        AudioSettingsPanel.IsVisible = true;
    }

    private void GoBackFromAudioSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.IsVisible = true;
        AudioSettingsPanel.IsVisible = false;
    }

    private void GoBackFromJoinRoom_OnClick(object? sender, RoutedEventArgs e)
    {
        JoinRoomPanel.IsVisible = false;
        MainPanel.IsVisible = true;
    }

    private void JoinRoom_OnClick(object? sender, RoutedEventArgs e)
    {
        CallPanel.IsVisible = true;
        JoinRoomPanel.IsVisible = false;
    }

    private void LeaveCall_OnClick(object? sender, RoutedEventArgs e)
    {
        CallPanel.IsVisible = false;
        JoinRoomPanel.IsVisible = true;
    }
}