using System;
using System.Collections.Generic;
using System.Linq;
using Plugin.Geolocator;
using Xamarin.Forms.GoogleMaps;

namespace PaketGlobal
{
    public partial class MapPickerPage : BasePage
    {
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

            MapView.UiSettings.ZoomControlsEnabled = false;

            MapView.CameraIdled += (sender, e) =>
            {
            };

            MapView.MapLongClicked += async (sender, e) =>
            {
                MapView.Pins.Clear();

                var pin = new Pin() { Label = "", Position = new Position(e.Point.Latitude, e.Point.Longitude) };
                MapView.Pins.Add(pin);

                await MapView.AnimateCamera(CameraUpdateFactory.NewPositionZoom(
                    pin.Position, MapView.CameraPosition.Zoom), TimeSpan.FromSeconds(1));

                LoadAddress(e.Point.Latitude,e.Point.Longitude);
            };
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MapView = null;
        }

        protected override async void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

                if (hasPermission)
                {
                    App.ShowLoading(true);

                    var locator = CrossGeolocator.Current;
                    var position = await locator.GetPositionAsync(new TimeSpan(10000));

                    var pin = new Pin() { Label = "", Position = new Position(position.Latitude, position.Longitude) };
                    MapView.Pins.Add(pin);

                    await MapView.AnimateCamera(CameraUpdateFactory.NewPositionZoom(
                        pin.Position, 16d), TimeSpan.FromSeconds(1));

                    LoadAddress(position.Latitude, position.Longitude);

                    App.ShowLoading(false);
                }
            }
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void OnSelect(object sender, System.EventArgs e)
        {
            //Navigation.PopAsync();
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
