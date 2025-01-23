using System.Windows.Input;

namespace Air.ViewModels;

public class AudioDeviceItemViewModel : ViewModelBase
{
    public AudioDeviceItemViewModel(string header, ICommand command)
    {
        Header = header;
        Command = command;
    }
    
    public string Header { get; set; }
    public ICommand Command { get; set; }
}