namespace Air.ViewModels;

public class MainPanelViewModel : BasePanelViewModel
{
    public MainPanelViewModel(MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel)
    {
        
    }

    public void ShowAudioSettingsPanelView()
    {
        MainWindow.ShowAudioSettingsPanelView();
    }

    public void ShowJoinRoomPanelView()
    {
        MainWindow.ShowJoinRoomPanelView();
    }
}