using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Air.Connection;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Exception = System.Exception;

namespace Air.ViewModels;

public partial class CallPanelViewModel : CallPanelBase
{
    public CallPanelViewModel(CallConnectionManager callConnectionManager, MainWindowViewModel mainWindowViewModel)
        : base(mainWindowViewModel, callConnectionManager)
    {
        SubscribeToCallEvents();
    }
    
    public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = new();
    
    [ObservableProperty] private string _userMessage = string.Empty;

    [RelayCommand(CanExecute = nameof(IsValidMessage))]
    private async Task SendMessage(string message)
    {
        try
        {
            await CallConnectionManager.SendTextMessage(message);
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
            await CallConnectionManager.LeaveRoom();
        }
        catch
        {
            await CallConnectionManager.EnsureDisconnectAsync();
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
        CallConnectionManager.RoomJoinEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.User, "joined the room", DateTime.Now));
        };

        CallConnectionManager.MessageReceiveEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.Sender, args.Message, DateTime.Now));
        };

        CallConnectionManager.UserLeftRoomEvent += (_, args) =>
        {
            Dispatcher.UIThread.Post(() => AddChatMessage(args.User, "left the room", DateTime.Now));
        };
    }
}