using System;
using System.Collections.Generic;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PaketGlobal
{
    public enum LocationPickerType { From, To }

    public class LocationPickerPageEventArgs : EventArgs
    {
        private readonly GooglePlace item;

        public LocationPickerPageEventArgs(GooglePlace item)
        {
            this.item = item;
        }

        public GooglePlace Item
        {
            get { return this.item; }
        }
    }

    public partial class LocationPickerPage : BasePage
    {
        public LocationPickerType PickerType;
        private int minimumSearchText = 2;
        private GooglePlacesHelper GooglePlaces = new GooglePlacesHelper();
        private List<AutoCompletePrediction> AutoCompletePlaces;

        public delegate void LocationPickerPageEventHandler(object sender, LocationPickerPageEventArgs args);
        public LocationPickerPageEventHandler eventHandler;

        private string Location = null;

        public LocationPickerPage(LocationPickerType pickerType)
        {
            InitializeComponent();

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

            if (PickerType == LocationPickerType.To)
            {
                SearchField.Placeholder = AppResources.LocationPickerTypeTo;
            }
            else if (PickerType == LocationPickerType.From)
            {
                SearchField.Placeholder = AppResources.LocationPickerTypeFrom;
            }

            PlacesRetrieved(new AutoCompleteResult());
        }

        protected override async void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                if (Location == null)
                {
                    var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

                    if (hasPermission)
                    {
                        var locator = CrossGeolocator.Current;

                        var position = await locator.GetPositionAsync();

                        if (position != null)
                        {
                            Location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        }
                    }
                }
            }
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.NewTextValue) && e.NewTextValue.Length >= minimumSearchText)
            {
                var predictions = await GooglePlaces.GetPlaces(e.NewTextValue,Location);
                if (predictions != null)
                    PlacesRetrieved(predictions);
                else
                    PlacesRetrieved(new AutoCompleteResult());
            }
            else
            {
                PlacesRetrieved(new AutoCompleteResult());
            }
        }

        private void PlacesRetrieved(AutoCompleteResult result)
        {
            var mapItem = new AutoCompletePrediction();
            mapItem.Structured = new StructuredFormatting();
            mapItem.Structured.MainText = AppResources.Map;
            mapItem.Structured.SecondText = AppResources.SelectOnMap;

            if(result.AutoCompletePlaces==null)
            {
                result.AutoCompletePlaces = new List<AutoCompletePrediction>();            
                result.AutoCompletePlaces.Add(mapItem);
            }
            else{
                result.AutoCompletePlaces.Insert(0, mapItem);
            }

            AutoCompletePlaces = result.AutoCompletePlaces;
            ItemsListView.ItemsSource = AutoCompletePlaces;
        }
 

        private async void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            Unfocus();


            var prediction = (AutoCompletePrediction)e.SelectedItem;

            if(prediction.Place_ID != null)
            {
                App.ShowLoading(true);

                var place = await this.GooglePlaces.GetPlace(prediction.Place_ID);
                place.Address = prediction.Description;
                if (place != null)
                {
                    eventHandler(this, new LocationPickerPageEventArgs(place));

                    await Navigation.PopAsync();
                }
                else
                {
                    ShowErrorMessage(AppResources.CantGetGooglePlace);
                }

                App.ShowLoading(false);
            }
 
        }
    }
}
