using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Air.Connection;
using Avalonia.Media;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Exception = System.Exception;

namespace Air.ViewModels;

public partial class CallPanelViewModel : BasePanelViewModel
{
    public CallPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel)
    {
        _callConnectionManager = callConnectionManager;
        
        SubscribeToCallEvents();
    }
    
    public static TextTrimming ErrorMessageTrimming => TextTrimming.CharacterEllipsis;
    
    public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = new();

    [ObservableProperty] private string _userMessage = string.Empty;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string? _errorMessageToolTip;

    private readonly CallConnectionManager _callConnectionManager;
    
    partial void OnErrorMessageChanged(string? value)
    {
        ErrorMessageToolTip = value;
    }

    [RelayCommand(CanExecute = nameof(IsValidMessage))]
    private async Task SendMessage(string message)
    {
        try
        {
            await _callConnectionManager.SendTextMessage(message);
            Dispatcher.UIThread.Post(() => UserMessage = string.Empty);
            Dispatcher.UIThread.Post(() => ErrorMessage = null);
        }
        catch (Exception e)
        {
            ErrorMessage = e.Message;
            // TODO: try to reconnect
        }
    }

    [RelayCommand]
    private async Task LeaveRoom()
    {
        try
        {
            await _callConnectionManager.LeaveRoom();
        }
        catch
        {
            await _callConnectionManager.EnsureDisconnectAsync();
        }
        
        MainWindow.ShowJoinRoomPanelView();
    }

    private bool IsValidMessage(string message)
    {
        return !string.IsNullOrWhiteSpace(message);
    }

    private void AddChatMessage(string user, string message, DateTime dateTime)
    {
        ChatMessages.Add(new ChatMessageViewModel(user, message, dateTime));
    }

    private void SubscribeToCallEvents()
    {
        _callConnectionManager.RoomJoinEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.User, "joined the room", DateTime.Now));
        };

        _callConnectionManager.MessageReceiveEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.Sender, args.Message, DateTime.Now));
        };

        _callConnectionManager.UserLeftRoomEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.User, "left the room", DateTime.Now));
        };
    }
}