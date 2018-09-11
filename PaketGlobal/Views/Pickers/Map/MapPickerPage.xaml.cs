using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Geolocator;
using Xamarin.Forms.GoogleMaps;

namespace PaketGlobal
{
    public partial class MapPickerPage : BasePage
    {
        public delegate void LocationPickerPageEventHandler(object sender, LocationPickerPageEventArgs args);
        public LocationPickerPageEventHandler eventHandler;

        public MapPickerPage()
        {
            InitializeComponent();

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif

            SelectButton.Opacity = 0;
            MapView.UiSettings.ZoomControlsEnabled = false;
            MapView.MyLocationEnabled = true;

            MapView.CameraIdled += (sender, e) =>
            {
                if(SelectButton.Opacity > 0)
                {
                    LoadAddress(MapView.CameraPosition.Target.Latitude, MapView.CameraPosition.Target.Longitude); 
                }
            };

            if(!App.Locator.LocationHelper.lat.Equals(0.0))
            {
                MapView.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(App.Locator.LocationHelper.lat, App.Locator.LocationHelper.lng), 20d);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MapView = null;
        
        }
        protected override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();

            if (fl)
            {
                LoadInitialPosition();
            }
        }

        private async void LoadInitialPosition()
        {
            var position = await App.Locator.LocationHelper.GetLocation(true);

            if (position != null && MapView != null)
            {
                var pin = new Pin() { Label = "", Position = new Position(position.Latitude, position.Longitude) };

                SelectButton.Opacity = 1;

                await MapView.AnimateCamera(CameraUpdateFactory.NewPositionZoom(
                   pin.Position, 20d), TimeSpan.FromSeconds(0.01));

                LoadAddress(position.Latitude, position.Longitude);
            }
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnSelect(object sender, System.EventArgs e)
        {
            if(AddressLabel.Text==null)
            {
                return;
            }
            else if(AddressLabel.Text.Length==0)
            {
                return;
            }

            var place = new GooglePlace();
            place.Address = AddressLabel.Text;
            place.Latitude = MapView.CameraPosition.Target.Latitude;
            place.Longitude = MapView.CameraPosition.Target.Longitude;

            Navigation.PopAsync(false);

            eventHandler(this, new LocationPickerPageEventArgs(place));
        }


        private async void LoadAddress(double lat, double lng)
        {
            try{
                var position = new Plugin.Geolocator.Abstractions.Position(lat, lng);

                var locator = CrossGeolocator.Current;
                var result = await locator.GetAddressesForPositionAsync(position, Config.GooglePlacesKEY);

                AddressLabel.Text = "";

                if (result != null)
                {
                    var addresses = result.ToList();

                    if (addresses.Count > 0)
                    {
                        var address = addresses[0];
                        string formatedAddress = "";

                        if (address.SubThoroughfare != null)
                        {
                            formatedAddress = address.Thoroughfare + " " + address.SubThoroughfare + ", " + address.Locality;
                        }
                        else
                        {
                            formatedAddress = address.Thoroughfare + ", " + address.Locality;
                        }
                        AddressLabel.Text = formatedAddress;
                    }
                }
            }
            catch(Exception)
            {
                AddressLabel.Text = "";   
            }
        }

    }
}
