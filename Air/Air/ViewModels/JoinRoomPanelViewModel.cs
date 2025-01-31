using System;
using System.Threading.Tasks;
using Air.Connection;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Air.ViewModels;

public partial class JoinRoomPanelViewModel : ViewModelBase
{
    public JoinRoomPanelViewModel(CallConnectionManager callConnectionManager)
    {
        _callConnectionManager = callConnectionManager;
    }
    
    [ObservableProperty] private string _nameOfRoomToJoin = string.Empty;

    private readonly CallConnectionManager _callConnectionManager;

    [RelayCommand(CanExecute = nameof(CanJoin))]
    private async Task Join(string roomId)
    {
        await _callConnectionManager.StartConnectionAsync();
        await _callConnectionManager.JoinRoom(roomId, Guid.NewGuid().ToString()[..5]);
    }

    private bool CanJoin(string roomId)
    {
        return !string.IsNullOrWhiteSpace(roomId);
    }
}