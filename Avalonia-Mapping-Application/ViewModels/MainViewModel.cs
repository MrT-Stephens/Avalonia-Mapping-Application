using Avalonia_Mapping_Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splat;
using System.Reactive.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Avalonia_Mapping_Application.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DataBaseService DataBaseService;

    public MainViewModel()
    {
        DataBaseService = new DataBaseService();
    }

    [ObservableProperty]
    private string _Email = string.Empty;

    [ObservableProperty]
    private string _Password = string.Empty;

    [ObservableProperty]
    private bool _IsUserLoggedIn = false;

    [ObservableProperty]
    private bool _LoginStatusNotEmpty = false;

    private string _LoginStatus = string.Empty;

    public string LoginStatus
    {
        get => _LoginStatus;
        set 
        { 
            SetProperty(ref _LoginStatus, value); 
            LoginStatusNotEmpty = !string.IsNullOrEmpty(value);
        }
    }

    [RelayCommand]
    private async Task Login()
    {
        if (!Regex.IsMatch(Email, @"^[a-z0-9][-a-z0-9._]+@([-a-z0-9]+\.)+[a-z]{2,5}$"))
        {
            LoginStatus = "Please enter a valid email. E.g. example@example.com";
            return;
        }
        else if (!Regex.IsMatch(Password, @"((?=.*\d)(?=.*[A-Z]).{8,50})"))
        {
            LoginStatus = "Please enter a valid password. It must contain, one digit, one uppercase, one symbol, and at least 8 characters.";
            return;
        }
        else
        {
            LoginStatus = "Logging in...";
            IsUserLoggedIn = await DataBaseService.Connect(Email, Password);
            Email = string.Empty;
            Password = string.Empty;
        }
    }

    [RelayCommand]
    private void Logout()
    {
        IsUserLoggedIn = false;
        LoginStatus = string.Empty;
        DataBaseService.Disconnect();
    }
}
