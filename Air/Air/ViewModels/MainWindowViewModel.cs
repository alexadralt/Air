using Air.Connection;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Air.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        _callConnectionManager = new CallConnectionManager();
        _currentPanel = new MainPanelViewModel(this);
    }
    
    public static Color DarkThemeBackgroundColor => new(255, 43, 41, 51);

    [ObservableProperty] private BasePanelViewModel _currentPanel;

    private readonly CallConnectionManager _callConnectionManager;

    public void ShowMainPanelView()
    {
        CurrentPanel = new MainPanelViewModel(this);
    }

    public void ShowCallPanelView()
    {
        CurrentPanel = new CallPanelViewModel(_callConnectionManager, this);
    }

    public void ShowJoinRoomPanelView()
    {
        CurrentPanel = new JoinRoomPanelViewModel(_callConnectionManager, this);
    }

    public void ShowAudioSettingsPanelView()
    {
        CurrentPanel = new AudioSettingsPanelViewModel(this);
    }
}