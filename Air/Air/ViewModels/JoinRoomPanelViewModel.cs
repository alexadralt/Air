using System;
using System.Threading.Tasks;
using Air.Connection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Air.ViewModels;

public partial class JoinRoomPanelViewModel : BasePanelViewModel
{
    public JoinRoomPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel)
    {
        _callConnectionManager = callConnectionManager;
    }
    
    [ObservableProperty] private string _nameOfRoomToJoin = string.Empty;

    private readonly CallConnectionManager _callConnectionManager;

    public void ShowMainPanelView()
    {
        MainWindow.ShowMainPanelView();
    }

    // NOTE: by default, RelayCommand does not allow concurrent executions. Therefore CanExecute() returns false
    // when join task is still running, what makes "Join" button inactive
    [RelayCommand(CanExecute = nameof(CanJoin))]
    private async Task Join(string roomId)
    {
        try
        {
            await _callConnectionManager.JoinRoom(roomId, Guid.NewGuid().ToString()[..5]);
            MainWindow.ShowCallPanelView();
        }
        catch
        {
            await _callConnectionManager.EnsureDisconnectAsync();
        }
    }

    private bool CanJoin(string roomId)
    {
        return !string.IsNullOrWhiteSpace(roomId);
    }
}