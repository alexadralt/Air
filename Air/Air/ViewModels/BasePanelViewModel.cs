namespace Air.ViewModels;

public abstract class BasePanelViewModel : ViewModelBase
{
    protected BasePanelViewModel(MainWindowViewModel mainWindow)
    {
        MainWindow = mainWindow;
    }
    
    protected readonly MainWindowViewModel MainWindow;
}