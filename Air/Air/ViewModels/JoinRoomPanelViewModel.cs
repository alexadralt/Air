using System;
using System.Threading;
using System.Threading.Tasks;
using Air.Connection;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Air.ViewModels;

public partial class JoinRoomPanelViewModel : BasePanelViewModel
{
    public JoinRoomPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel)
    {
        _callConnectionManager = callConnectionManager;
        _tokenSource = new CancellationTokenSource();
    }

    public static TextTrimming ErrorMessageTrimming => TextTrimming.CharacterEllipsis;
    
    [ObservableProperty] private string _nameOfRoomToJoin = string.Empty;
    [ObservableProperty] private string _errorMessage = string.Empty;
    [ObservableProperty] private string? _errorMessageToolTip;

    private readonly CallConnectionManager _callConnectionManager;
    private readonly CancellationTokenSource _tokenSource;

    partial void OnErrorMessageChanged(string value)
    {
        ErrorMessageToolTip = value;
    }

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
            await _callConnectionManager.JoinRoom(roomId, Guid.NewGuid().ToString()[..5], _tokenSource.Token);
            MainWindow.ShowCallPanelView();
        }
        catch (Exception e)
        {
            await _callConnectionManager.EnsureDisconnectAsync();
            ErrorMessage = e.Message;
        }
    }

    private bool CanJoin(string roomId)
    {
        return !string.IsNullOrWhiteSpace(roomId);
    }
}