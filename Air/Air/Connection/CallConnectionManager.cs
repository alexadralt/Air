using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace Air.Connection;

public class RoomJointEventArgs : EventArgs
{
    public string User { get; init; }
}

public class UserLeftRoomEventArgs : EventArgs
{
    public string User { get; init; }
}

public class MessageReceivedEventArgs : EventArgs
{
    public string Sender { get; init; }
    public string Message { get; init; }
}

public class CallConnectionManager
{
    public CallConnectionManager()
    {
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7181/call")
            .Build();

        _connection.On<string>("HandleNewRoomUser", OnHandleNewRoomUser);
        _connection.On<string>("HandleUserLeftRoom", OnHandleUserLeftRoom);
        _connection.On<string, string>("ReceiveTextMessage", OnReceiveTextMessage);
    }

    public event EventHandler<RoomJointEventArgs> RoomJoinEvent;
    public event EventHandler<MessageReceivedEventArgs> MessageReceiveEvent;
    public event EventHandler<UserLeftRoomEventArgs> UserLeftRoomEvent;
    
    private readonly HubConnection _connection;

    public async Task EnsureDisconnectAsync()
    {
        if (_connection.State is HubConnectionState.Connected or HubConnectionState.Connecting)
            await _connection.StopAsync();
    }

    public async Task JoinRoom(string roomId, string userName, CancellationToken ct)
    {
        await _connection.StartAsync(ct);
        await _connection.InvokeAsync("JoinRoom", roomId, userName, ct);
    }

    public async Task LeaveRoom()
    {
        await _connection.InvokeAsync("LeaveRoom");
        await _connection.StopAsync();
    }

    public async Task SendTextMessage(string message)
    {
        await _connection.InvokeAsync("SendTextMessage", message);
    }

    private void OnHandleNewRoomUser(string user)
    {
        RoomJoinEvent?.Invoke(this, new RoomJointEventArgs() { User = user });
    }

    private void OnReceiveTextMessage(string sender, string message)
    {
        MessageReceiveEvent?.Invoke(this, new MessageReceivedEventArgs() { Sender = sender, Message = message });
    }

    private void OnHandleUserLeftRoom(string user)
    {
        UserLeftRoomEvent?.Invoke(this, new UserLeftRoomEventArgs() { User = user });
    }
}