using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace PaketGlobal
{
    public class CountryPickerPageEventArgs : EventArgs
    {
        private readonly ISO3166Country item;

        public CountryPickerPageEventArgs(ISO3166Country item)
        {
            this.item = item;
        }

        public ISO3166Country Item
        {
            get { return this.item; }
        }
    }

    public partial class CountryPickerPage : BasePage
    {
        private List<ISO3166Country> Items;
        public delegate void CountryPickerEventHandler(object sender, CountryPickerPageEventArgs args);
        public CountryPickerEventHandler eventHandler;

        public CountryPickerPage()
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

            Items = ISO3166.GetCollection().ToList();
            ItemsListView.ItemsSource = Items;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            await Navigation.PopAsync();

            eventHandler(this, new CountryPickerPageEventArgs(e.SelectedItem as ISO3166Country));
        }
    }
}
