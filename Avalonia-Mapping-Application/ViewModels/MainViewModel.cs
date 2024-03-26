using Avalonia_Mapping_Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia_Mapping_Application.DataModels;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Avalonia.Controls;

namespace Avalonia_Mapping_Application.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly DataBaseService DataBaseService;

    public MainViewModel()
    {
        DataBaseService = new DataBaseService();
        GeoCoordinates = new ObservableCollection<GeoCoordinate>();
        SelectedGeoCoordinate = null;
    }

    [ObservableProperty]
    private ObservableCollection<GeoCoordinate> _GeoCoordinates;

    [ObservableProperty]
    private bool _UpdatingGeoCoordinate = false;

    [ObservableProperty]
    private string _Name = string.Empty;

    [ObservableProperty]
    private string _Latitude = string.Empty;

    [ObservableProperty]
    private string _Longitude = string.Empty;

    [ObservableProperty]
    private string _Email = "mrtstephens123@gmail.com";

    [ObservableProperty]
    private string _Password = "TStephens123!";

    [ObservableProperty]
    private bool _IsUserLoggedIn = false;

    [ObservableProperty]
    private bool _LoginStatusNotEmpty = false;

    [ObservableProperty]
    private string _NewDatasetName = string.Empty;

    private GeoCoordinate? _SelectedGeoCoordinate;

    public GeoCoordinate? SelectedGeoCoordinate
    {
        get => _SelectedGeoCoordinate;
        set
        {
            SetProperty(ref _SelectedGeoCoordinate, value);
            SelectionChanged(value);
        }
    }

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

            if (IsUserLoggedIn)
            {
                GeoCoordinates = new ObservableCollection<GeoCoordinate>(await DataBaseService.GetGeoCoordinates());
            }
        }
    }

    [RelayCommand]
    private void Logout()
    {
        IsUserLoggedIn = false;
        LoginStatus = string.Empty;
        DataBaseService.Disconnect();
    }

    [RelayCommand]
    private async Task AddGeoCoordinate()
    {
        if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Latitude) || !string.IsNullOrEmpty(Longitude))
        {
            DataBaseService.AddGeoCoordinate(new GeoCoordinate { Name = Name, Latitude = double.Parse(Latitude), Longitude = double.Parse(Longitude) });

            Name = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;

            GeoCoordinates = new ObservableCollection<GeoCoordinate>(await DataBaseService.GetGeoCoordinates());
        }
    }

    private void SelectionChanged(GeoCoordinate? geoCoordinate)
    {
        if (SelectedGeoCoordinate != null)
        {
            Name = SelectedGeoCoordinate.Name;
            Latitude = SelectedGeoCoordinate.Latitude.ToString();
            Longitude = SelectedGeoCoordinate.Longitude.ToString();

            UpdatingGeoCoordinate = true;
        }
        else
        {
            Name = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;

            UpdatingGeoCoordinate = false;
        }
    }
}
