using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Air.Connection;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Air.ViewModels;

public partial class CallPanelViewModel : BasePanelViewModel
{
    public CallPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel)
    {
        _callConnectionManager = callConnectionManager;

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
    
    public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = new();

    [ObservableProperty] private string _userMessage = string.Empty;

    private readonly CallConnectionManager _callConnectionManager;

    [RelayCommand(CanExecute = nameof(IsValidMessage))]
    private async Task SendMessage(string message)
    {
        await _callConnectionManager.SendTextMessage(message);
        Dispatcher.UIThread.Post(() => UserMessage = string.Empty);
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
        
        Dispatcher.UIThread.Post(() => ChatMessages.Clear());
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
}