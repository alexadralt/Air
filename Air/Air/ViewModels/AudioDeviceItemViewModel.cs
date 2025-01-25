using System;
using System.Windows.Input;

namespace Air.ViewModels;

public class AudioDeviceItemViewModel : ViewModelBase
{
    public static AudioDeviceItemViewModel OutputDevice(string header, Guid outputDeviceGuid)
        => new AudioDeviceItemViewModel(header, outputDeviceGuid);

    public static AudioDeviceItemViewModel InputDevice(string header, string inputDeviceId)
        => new AudioDeviceItemViewModel(header, inputDeviceId);
    
    private AudioDeviceItemViewModel(string header, Guid outputDeviceGuid)
    {
        Header = header;
        OutputDeviceGuid = outputDeviceGuid;
        InputDeviceId = null;
    }

    private AudioDeviceItemViewModel(string header, string? inputDeviceId)
    {
        Header = header;
        OutputDeviceGuid = Guid.Empty;
        InputDeviceId = inputDeviceId;
    }
    
    public string Header { get; set; }
    public ICommand? Command { get; set; }
    public Guid OutputDeviceGuid { get; set; }
    public string? InputDeviceId { get; set; }
}