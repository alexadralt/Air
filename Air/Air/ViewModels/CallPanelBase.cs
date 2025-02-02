using Air.Connection;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Air.ViewModels;

public abstract partial class CallPanelBase : BasePanelViewModel
{
    protected CallPanelBase(MainWindowViewModel mainWindow, CallConnectionManager callConnectionManager)
        : base(mainWindow)
    {
        CallConnectionManager = callConnectionManager;
    }
    
    public static TextTrimming ErrorMessageTrimming => TextTrimming.CharacterEllipsis;
    
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private string? _errorMessageToolTip;

    protected readonly CallConnectionManager CallConnectionManager;
    
    partial void OnErrorMessageChanged(string? value)
    {
        ErrorMessageToolTip = value;
    }
}