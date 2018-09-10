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

            App.Locator.DeviceService.setStausBarLight();

            if (fl)
            {
                if (Location == null)
                {
                    Location = await App.Locator.LocationHelper.GetStringLocation(true);
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
                ProgressIndicator.IsRunning = true;

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

            ProgressIndicator.IsRunning = false;
        }

        private void PlacesRetrieved(AutoCompleteResult result)
        {
            var mapItem = new AutoCompletePrediction();
            mapItem.Structured = new StructuredFormatting();
            mapItem.Structured.MainText = AppResources.Map;
            mapItem.Structured.SecondText = AppResources.SelectOnMap;
            mapItem.Structured.Icon = "red_pin.png";

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
                    ItemsListView.SelectedItem = null;

                    ShowErrorMessage(AppResources.CantGetGooglePlace);
                }

                App.ShowLoading(false);
            }
            else{
                ItemsListView.SelectedItem = null;

                var page = new MapPickerPage();
                page.eventHandler = DidSelectLocationHandler;
                await Navigation.PushAsync(page, true);
            }
        }

        private void DidSelectLocationHandler(object sender, LocationPickerPageEventArgs e)
        {
            eventHandler(this, new LocationPickerPageEventArgs(e.Item));

            Navigation.PopAsync();
        }
    }
}
