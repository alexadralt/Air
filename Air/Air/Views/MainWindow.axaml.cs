using System.Diagnostics;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Styling;
using NAudio.CoreAudioApi;
using NAudio.Wave;

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

    private void CreateRoom_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void JoinRoom_OnClick(object? sender, RoutedEventArgs e)
    {
        
    }

    private void AudioSettings_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.IsVisible = false;
        AudioSettingsPanel.IsVisible = true;
    }

    private void GoBack_OnClick(object? sender, RoutedEventArgs e)
    {
        MainPanel.IsVisible = true;
        AudioSettingsPanel.IsVisible = false;
    }
}