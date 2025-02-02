using System;
using System.Threading;
using System.Threading.Tasks;
using Air.Connection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Air.ViewModels;

public partial class JoinRoomPanelViewModel : CallPanelBase
{
    public JoinRoomPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel, callConnectionManager)
    {
        _tokenSource = new CancellationTokenSource();
    }

    [ObservableProperty] private string _nameOfRoomToJoin = string.Empty;

    private readonly CancellationTokenSource _tokenSource;

    public void ShowMainPanelView()
    {
        _tokenSource.Cancel();
        MainWindow.ShowMainPanelView();
    }

    // NOTE: by default, RelayCommand does not allow concurrent executions. Therefore CanExecute() returns false
    // when join task is still running, what makes "Join" button inactive
    [RelayCommand(CanExecute = nameof(CanJoin))]
    private async Task Join(string roomId)
    {
        try
        {
            await CallConnectionManager.JoinRoom(roomId, Guid.NewGuid().ToString()[..5], _tokenSource.Token);
            MainWindow.ShowCallPanelView();
        }
        catch (Exception e)
        {
            await CallConnectionManager.EnsureDisconnectAsync();
            ErrorMessage = e.Message;
        }
    }

    private bool CanJoin(string roomId)
    {
        return !string.IsNullOrWhiteSpace(roomId);
    }
}