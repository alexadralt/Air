using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using ReactiveUI;

namespace Air.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public static Color DarkThemeBackgroundColor => new Color(255, 43, 41, 51);

    public ObservableCollection<AudioDeviceItemViewModel> OutputDevices { get; } = new();
    public ObservableCollection<AudioDeviceItemViewModel> InputDevices { get; } = new();

    [ObservableProperty] private string? _chosenOutputDeviceName;
    [ObservableProperty] private string? _chosenInputDeviceName;
    
    public void LoadAudioDevicesInfo()
    {
        OutputDevices.Clear();
        foreach (var device in DirectSoundOut.Devices)
        {
            OutputDevices.Add(new AudioDeviceItemViewModel(device.Description,
                ReactiveCommand.Create(() => ChosenOutputDeviceName = device.Description)));
        }
        
        if (ChosenOutputDeviceName == null || OutputDevices.All(item => item.Header != ChosenOutputDeviceName))
            ChosenOutputDeviceName = OutputDevices.Count > 0 ? OutputDevices[0].Header : null;
        
        InputDevices.Clear();
        var inputDevicesList = new MMDeviceEnumerator()
            .EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
        foreach (var device in inputDevicesList)
        {
            InputDevices.Add(new AudioDeviceItemViewModel(device.FriendlyName,
                ReactiveCommand.Create(() => ChosenInputDeviceName = device.FriendlyName)));
        }
        
        if (ChosenInputDeviceName == null || InputDevices.All(item => item.Header != ChosenInputDeviceName))
            ChosenInputDeviceName = InputDevices.Count > 0 ? InputDevices[0].Header : null;
    }
}