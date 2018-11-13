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

        private AddressData addressData;
        private GooglePlace SelectedAddress;
        public LocationPickerType PickerType;

        public MapPickerPage(LocationPickerType pickerType, GooglePlace selectedAddress = null)
        {
            InitializeComponent();

            SelectedAddress = selectedAddress;
            PickerType = pickerType;

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

            if(SelectedAddress!=null)
            {
                SelectButton.Opacity = 1;

                MapView.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(SelectedAddress.Latitude, SelectedAddress.Longitude), 20d);

                LoadAddress(SelectedAddress.Latitude, SelectedAddress.Longitude);
            }
            else if (!App.Locator.LocationHelper.lat.Equals(0.0))
            {
                MapView.InitialCameraUpdate = CameraUpdateFactory.NewPositionZoom(new Position(App.Locator.LocationHelper.lat, App.Locator.LocationHelper.lng), 20d);
            }

            if (PickerType == LocationPickerType.To)
            {
                TitleLabel.Text = AppResources.SelectToLocation;
            }
            else if (PickerType == LocationPickerType.From)
            {
                TitleLabel.Text = AppResources.SelectFromLocation;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        protected override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();

            if (fl && SelectedAddress==null)
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

            if(MapView!=null)
            {
                var place = new GooglePlace();
                place.Address = AddressLabel.Text;
                place.Latitude = MapView.CameraPosition.Target.Latitude;
                place.Longitude = MapView.CameraPosition.Target.Longitude;
                place.Country = addressData.Country;

                Navigation.PopAsync(false);

                eventHandler(this, new LocationPickerPageEventArgs(place));
            }
         
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

                        addressData = new AddressData();

                        if (address.SubThoroughfare != null)
                        {
                            formatedAddress = address.Thoroughfare + " " + address.SubThoroughfare + ", " + address.Locality;

                            addressData.Address = address.Thoroughfare + " " + address.SubThoroughfare;
                        }
                        else
                        {
                            formatedAddress = address.Thoroughfare + ", " + address.Locality;

                            addressData.Address = address.Thoroughfare;
                        }

                        addressData.Country = address.CountryCode;


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
