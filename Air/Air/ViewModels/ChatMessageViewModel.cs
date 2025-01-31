using System;

namespace Air.ViewModels;

public class ChatMessageViewModel : ViewModelBase
{
    public ChatMessageViewModel(string sender, string message, DateTime messageTime)
    {
        Sender = sender;
        Message = message;
        MessageTime = messageTime;
    }
    
    public string Sender { get; }
    public string Message { get; }
    public DateTime MessageTime { get; }
}