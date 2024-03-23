using CommunityToolkit.Mvvm.ComponentModel;
using Splat;

namespace Avalonia_Mapping_Application.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _Email = string.Empty;

    [ObservableProperty]
    private string _Password = string.Empty;

    void Login()
    {
        Locator.
    }
}
