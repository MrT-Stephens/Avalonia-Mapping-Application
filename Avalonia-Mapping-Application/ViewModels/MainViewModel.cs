using Avalonia_Mapping_Application.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia_Mapping_Application.DataModels;
using System.Collections.ObjectModel;
using System;

namespace Avalonia_Mapping_Application.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    // The DataBaseService class is used to connect to the MongoDB database and perform CRUD operations.
    private readonly DataBaseService DataBaseService;

    // Constructor for the MainViewModel class.
    public MainViewModel()
    {
        DataBaseService = new DataBaseService();
        GeoCoordinates = new ObservableCollection<GeoCoordinate>();
        SelectedGeoCoordinate = null;
        GeoCoordinatesChanged = null;
        GeoCoordinatesSelected = null;
    }

    // The GeoCoordinates property is an ObservableCollection of GeoCoordinate objects.
    [ObservableProperty]
    private ObservableCollection<GeoCoordinate> _GeoCoordinates;
    
    // The UpdatingGeoCoordinate property is a boolean value that indicates whether the user is updating a GeoCoordinate object.
    [ObservableProperty]
    private bool _UpdatingGeoCoordinate = false;

    // The Name property is a string value that represents the name of a GeoCoordinate object.
    [ObservableProperty]
    private string _Name = string.Empty;

    // The Latitude property is a string value that represents the latitude of a GeoCoordinate object.
    [ObservableProperty]
    private string _Latitude = string.Empty;

    // The Longitude property is a string value that represents the longitude of a GeoCoordinate object.
    [ObservableProperty]
    private string _Longitude = string.Empty;

    // The Email property is a string value that represents the email address of the user.
    [ObservableProperty]
    private string _Email = string.Empty;

    // The Password property is a string value that represents the password of the user.
    [ObservableProperty]
    private string _Password = string.Empty;

    // The IsUserLoggedIn property is a boolean value that indicates whether the user is logged in.
    [ObservableProperty]
    private bool _IsUserLoggedIn = false;

    // The LoginStatusNotEmpty property is a boolean value that indicates whether the login status is not empty.
    [ObservableProperty]
    private bool _LoginStatusNotEmpty = false;

    // The internal SelectedGeoCoordinate property is a GeoCoordinate object that represents the selected GeoCoordinate.
    private GeoCoordinate? _SelectedGeoCoordinate;

    // The external SelectedGeoCoordinate property is a GeoCoordinate object that represents the selected GeoCoordinate.
    // Will raise the SelectionChanged event when set.
    public GeoCoordinate? SelectedGeoCoordinate
    {
        get => _SelectedGeoCoordinate;
        set
        {
            SetProperty(ref _SelectedGeoCoordinate, value);
            SelectionChanged();
        }
    }

    // The internal LoginStatus property is a string value that represents the login status.
    private string _LoginStatus = string.Empty;

    // The external LoginStatus property is a string value that represents the login status.
    // Will set the LoginStatusNotEmpty property when set.
    public string LoginStatus
    {
        get => _LoginStatus;
        set 
        { 
            SetProperty(ref _LoginStatus, value); 
            LoginStatusNotEmpty = !string.IsNullOrEmpty(value);
        }
    }

    // An event handler for if the GeoCoordinates have changed.
    // Used to send an event to the view.
    public event EventHandler? GeoCoordinatesChanged;

    // An event handler for if a GeoCoordinate has been selected.
    // Used to send an event to the view.
    public event EventHandler? GeoCoordinatesSelected;

    // An event handler for if the user has logged out.
    // Used to send an event to the view.
    public event EventHandler? UserLoggedOut;

    // Login command that will attempt to log the user in.
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
                var geoCoordinates = await DataBaseService.GetGeoCoordinates();

                GeoCoordinates = new ObservableCollection<GeoCoordinate>(geoCoordinates);
                GeoCoordinatesChanged?.Invoke(geoCoordinates, EventArgs.Empty);
            }
        }
    }

    // Logout command that will log the user out.
    [RelayCommand]
    private void Logout()
    {
        IsUserLoggedIn = false;
        LoginStatus = string.Empty;
        DataBaseService.Disconnect();

        UserLoggedOut?.Invoke(null, EventArgs.Empty);
    }

    // AddGeoCoordinate command that will add a GeoCoordinate object to the database.
    [RelayCommand]
    private async Task AddGeoCoordinate()
    {
        if (!string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Latitude) || !string.IsNullOrEmpty(Longitude))
        {
            DataBaseService.AddGeoCoordinate(new GeoCoordinate { Name = Name, Latitude = double.Parse(Latitude), Longitude = double.Parse(Longitude) });

            Name = string.Empty;
            Latitude = string.Empty;
            Longitude = string.Empty;

            var geoCoordinates = await DataBaseService.GetGeoCoordinates();

            GeoCoordinates = new ObservableCollection<GeoCoordinate>(geoCoordinates);
            GeoCoordinatesChanged?.Invoke(geoCoordinates, EventArgs.Empty);
        }
    }

    // RemoveGeoCoordinate command that will remove a GeoCoordinate object from the database.
    [RelayCommand]
    private async Task RemoveGeoCoordinate()
    {
        if (SelectedGeoCoordinate != null)
        { 
            DataBaseService.DeleteGeoCoordinate(SelectedGeoCoordinate.ID);

            var geoCoordinates = await DataBaseService.GetGeoCoordinates();

            GeoCoordinates = new ObservableCollection<GeoCoordinate>(geoCoordinates);
            GeoCoordinatesChanged?.Invoke(geoCoordinates, EventArgs.Empty);
        }
    }

    // UpdateGeoCoordinate command that will update a GeoCoordinate object in the database.
    [RelayCommand]
    private async Task UpdateGeoCoordinate()
    {
        if (SelectedGeoCoordinate != null)
        {
            SelectedGeoCoordinate.Name = Name;
            SelectedGeoCoordinate.Latitude = double.Parse(Latitude);
            SelectedGeoCoordinate.Longitude = double.Parse(Longitude);

            DataBaseService.UpdateGeoCoordinate(SelectedGeoCoordinate);

            var geoCoordinates = await DataBaseService.GetGeoCoordinates();

            GeoCoordinates = new ObservableCollection<GeoCoordinate>(geoCoordinates);
            GeoCoordinatesChanged?.Invoke(geoCoordinates, EventArgs.Empty);
        }
    }

    // The SelectionChanged method is called when the selected GeoCoordinate changes.
    // Also invokes the GeoCoordinatesSelected event.
    private void SelectionChanged()
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

        GeoCoordinatesSelected?.Invoke(SelectedGeoCoordinate, EventArgs.Empty);
    }
}
