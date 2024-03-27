using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia_Mapping_Application.DataModels;
using Avalonia_Mapping_Application.ViewModels;
using Mapsui;
using Mapsui.Animations;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.Widgets.MouseCoordinatesWidget;
using Mapsui.Widgets.Zoom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia_Mapping_Application.Views;

public partial class MainView : UserControl
{
    // This method binds the events to the MainViewModel
    // and initializes the MapControl and its components
    public MainView()
    {
        InitializeComponent();

        SizeChanged += (sender, args) => ResizeLoginPopup(LoginPopup);
        Loaded += (sender, args) =>
        {
            if (DataContext is MainViewModel mainViewModel)
            {
                mainViewModel.GeoCoordinatesChanged += GeoCoordinatesChanged;
                mainViewModel.GeoCoordinatesSelected += GeoCoordinatesSelected;
                mainViewModel.UserLoggedOut += UserLoggedOut;
            }
        };
    }

    // This method initializes the MapControl and its components
    private void InitializeComponent()
    {
        InitializeComponent(true);

        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MapControl.Map.Navigator.RotationLock = false;
        MapControl.Map.Widgets.Add(CreateMouseCoordinatesWidget(MapControl.Map, Mapsui.Widgets.VerticalAlignment.Bottom, Mapsui.Widgets.HorizontalAlignment.Right));
        MapControl.Map.Widgets.Add(CreateZoomInOutWidget(Orientation.Horizontal, Mapsui.Widgets.VerticalAlignment.Top, Mapsui.Widgets.HorizontalAlignment.Right));

        MapControl.Map.Info += MapOnInfo;

        MapControl.Map.Widgets.Add(CreateMapInfoWidget(MapControl.Map, Mapsui.Widgets.VerticalAlignment.Bottom, Mapsui.Widgets.HorizontalAlignment.Left));
    }

    // Static method to create the MapInfoWidget
    private static MapInfoWidget CreateMapInfoWidget(Map map, Mapsui.Widgets.VerticalAlignment verticalAlignment, Mapsui.Widgets.HorizontalAlignment horizontalAlignment)
    {
        return new MapInfoWidget(map)
        {
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            MarginX = 20,
            MarginY = 20,
        };
    }

    // Static method to create the MouseCoordinatesWidget
    private static MouseCoordinatesWidget CreateMouseCoordinatesWidget(Map map, Mapsui.Widgets.VerticalAlignment verticalAlignment, Mapsui.Widgets.HorizontalAlignment horizontalAlignment)
    {
        return new MouseCoordinatesWidget(map)
        {
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            MarginX = 20,
            MarginY = 20,
        };
    }

    // Static method to create the ZoomInOutWidget
    private static ZoomInOutWidget CreateZoomInOutWidget(Orientation orientation, Mapsui.Widgets.VerticalAlignment verticalAlignment, Mapsui.Widgets.HorizontalAlignment horizontalAlignment)
    {
        return new ZoomInOutWidget
        {
            Orientation = orientation,
            VerticalAlignment = verticalAlignment,
            HorizontalAlignment = horizontalAlignment,
            MarginX = 20,
            MarginY = 20,
        };
    }

    // Static method to create the PointLayer
    // This method creates a layer with the given geoCoordinates
    private static MemoryLayer CreatePointLayer(IEnumerable<IFeature> geoCoordinates)
    {
        return new MemoryLayer
        {
            Name = "Geo Coordinates Layer",
            IsMapInfoLayer = true,
            Features = new MemoryProvider(geoCoordinates).Features,
            Style = SymbolStyles.CreatePinStyle(symbolScale: 0.7),
        };
    }

    // Static method to create the CalloutStyle
    // This is the style of the callout that appears when a point is clicked
    private static CalloutStyle CreateCalloutStyle(string content)
    {
        return new CalloutStyle
        {
            Title = content,
            TitleFont = { FontFamily = null, Size = 12, Italic = false, Bold = true },
            TitleFontColor = Mapsui.Styles.Color.Gray,
            MaxWidth = 120,
            RectRadius = 10,
            ShadowWidth = 4,
            Enabled = false,
            SymbolOffset = new Offset(0, SymbolStyle.DefaultHeight * 1f)
        };
    }

    // This method is called when the LoginPopup is loaded
    // It resizes the LoginPopup to the size of the top level window
    private void LoginPopupLoaded(object sender, RoutedEventArgs e)
    {
        ResizeLoginPopup((Popup)sender);
    }

    // This method is called when the LoginPopup is opened
    // It resizes the LoginPopup to the size of the top level window
    private void ResizeLoginPopup(Popup popup)
    {
        var topLevel = TopLevel.GetTopLevel(this);

        if (topLevel != null && popup != null)
        {
            popup.Height = topLevel.Height;
            popup.Width = topLevel.Width;
        }
    }

    // This method is called when the map is clicked
    // It updates the Latitude and Longitude properties of the MainViewModel
    // and toggles the callout style of the clicked point
    private void MapOnInfo(object? sender, MapInfoEventArgs e)
    {
        if (DataContext is MainViewModel mainViewModel)
        {
            var mPoint = SphericalMercator.ToLonLat(e.MapInfo?.WorldPosition);

            mainViewModel.Latitude = mPoint.Y.ToString();
            mainViewModel.Longitude = mPoint.X.ToString();
        }

        var calloutStyle = e.MapInfo?.Feature?.Styles.Where(s => s is CalloutStyle).Cast<CalloutStyle>().FirstOrDefault();

        if (calloutStyle != null)
        {
            calloutStyle.Enabled = !calloutStyle.Enabled;
            e.MapInfo?.Layer?.DataHasChanged();
        }
    }

    // This method is called when the GeoCoordinatesSelected event is raised
    // It centers the map on the selected GeoCoordinate, if there is a geoCoordinate
    // Otherwise, it centers the map on the default coordinates
    private void GeoCoordinatesSelected(object? sender, EventArgs e)
    {
        if (sender is GeoCoordinate geoCoordinate)
        {
            MapControl.Map.Navigator.CenterOnAndZoomTo(SphericalMercator.FromLonLat(geoCoordinate.Longitude, geoCoordinate.Latitude).ToMPoint(), 1000.0, 1000, Easing.CubicInOut);
        }
        else
        {
            MapControl.Map.Navigator.CenterOnAndZoomTo(SphericalMercator.FromLonLat(0, 0).ToMPoint(), 20000.0, 1000, Easing.CubicInOut);
        }
    }

    // This method is called when the GeoCoordinatesChanged event is raised
    // It updates the GeoCoordinates layer with the new GeoCoordinates
    // If the GeoCoordinates layer already exists, it updates the features
    // Otherwise, it creates a new layer with the new GeoCoordinates
    private void GeoCoordinatesChanged(object? sender, EventArgs e)
    {
        if (sender is List<GeoCoordinate> geoCoordinates)
        {
            if (MapControl.Map.Layers.Count > 1)
            {
                ((MemoryLayer)MapControl.Map.Layers[1]).Features = geoCoordinates.Select(c =>
                {
                    var feature = new PointFeature(SphericalMercator.FromLonLat(c.Longitude, c.Latitude).ToMPoint());

                    feature[nameof(GeoCoordinate.Name)] = c.Name;
                    feature[nameof(GeoCoordinate.Latitude)] = c.Latitude;
                    feature[nameof(GeoCoordinate.Longitude)] = c.Longitude;

                    feature.Styles.Add(CreateCalloutStyle(feature.ToStringOfKeyValuePairs()));

                    return feature;
                });

                MapControl.Map.Layers[1].DataHasChanged();
            }
            else
            {
                MapControl.Map.Layers.Add(CreatePointLayer(geoCoordinates.Select(c =>
                {
                    var feature = new PointFeature(SphericalMercator.FromLonLat(c.Longitude, c.Latitude).ToMPoint());

                    feature[nameof(GeoCoordinate.Name)] = c.Name;
                    feature[nameof(GeoCoordinate.Latitude)] = c.Latitude;
                    feature[nameof(GeoCoordinate.Longitude)] = c.Longitude;

                    feature.Styles.Add(CreateCalloutStyle(feature.ToStringOfKeyValuePairs()));

                    return feature;
                })));
            }
        }
    }

    // This method is called when the UserLoggedOut event is raised
    // It clears the map layers and adds the OpenStreetMap layer
    private void UserLoggedOut(object? sender, EventArgs e)
    {
        MapControl.Map.Layers.Clear();
        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
    }

    // This method is called when the LightDarkModeButton is clicked
    // It switches the theme between Light and Dark mode
    private void LightDarkModeButtonClicked(object sender, RoutedEventArgs args)
    {
        if (sender is Button button)
        {
            if (App.Current?.ActualThemeVariant.ToString() == "Dark")
            {
                PathIcon pathIcon = new PathIcon();
                pathIcon.Data = (StreamGeometry)App.Current?.Resources["DarkModeIcon"];
                pathIcon.Width = 24;
                pathIcon.Height = 24;
                button.Content = pathIcon;
                App.ThemeManager?.Switch(0);
            }
            else
            {
                PathIcon pathIcon = new PathIcon();
                pathIcon.Data = (StreamGeometry)App.Current?.Resources["LightModeIcon"];
                pathIcon.Width = 24;
                pathIcon.Height = 24;
                button.Content = pathIcon;
                App.ThemeManager?.Switch(1);
            }
        }
    }

    // This method is called when the LightDarkModeButton is loaded
    // It sets the icon of the LightDarkModeButton based on the current theme
    private void LightDarkModeButtonLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            if (App.Current?.ActualThemeVariant.ToString() == "Dark")
            {
                PathIcon pathIcon = new PathIcon();
                pathIcon.Data = (StreamGeometry)App.Current?.Resources["LightModeIcon"];
                pathIcon.Width = 24;
                pathIcon.Height = 24;
                button.Content = pathIcon;
            }
            else
            {
                PathIcon pathIcon = new PathIcon();
                pathIcon.Data = (StreamGeometry)App.Current?.Resources["DarkModeIcon"];
                pathIcon.Width = 24;
                pathIcon.Height = 24;
                button.Content = pathIcon;
            }
        }
    }
}
