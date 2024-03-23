using Avalonia.Controls;
using Avalonia.Interactivity;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Tiling;
using Mapsui.Widgets;
using Mapsui.Widgets.MouseCoordinatesWidget;
using Mapsui.Widgets.Zoom;

namespace Avalonia_Mapping_Application.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        InitializeComponent(true);

        MapControl.Map.Layers.Add(OpenStreetMap.CreateTileLayer());
        MapControl.Map.Navigator.RotationLock = false;
        MapControl.Map.Widgets.Add(CreateMouseCoordinatesWidget(MapControl.Map, Mapsui.Widgets.VerticalAlignment.Bottom, Mapsui.Widgets.HorizontalAlignment.Right));
        MapControl.Map.Widgets.Add(CreateZoomInOutWidget(Orientation.Horizontal, Mapsui.Widgets.VerticalAlignment.Top, Mapsui.Widgets.HorizontalAlignment.Right));
    }

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

    private async void CurrentLocationButtonClick(object sender, RoutedEventArgs e)
    {

        //if (location != null)
        //{
        //    var mPoint = SphericalMercator.FromLonLat(location.Longitude, location.Latitude).ToMPoint();

        //    MapControl.Map.Navigator.CenterOnAndZoomTo(mPoint, 0.5);
        //}
    }
}
