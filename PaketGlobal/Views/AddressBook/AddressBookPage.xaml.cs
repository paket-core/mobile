using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public class AddressBookPageEventArgs : EventArgs
    {
        private readonly string item;
        private readonly double progress;

        public AddressBookPageEventArgs(string item)
        {
            this.item = item;
        }

        public string Item
        {
            get { return this.item; }
        }
    }

    public partial class AddressBookPage : BasePage
    {
        private List<string> Items;
        public bool IsCourierSelect = false;
               
        public delegate void AddressBookPageEventHandler(object sender, AddressBookPageEventArgs args);
        public AddressBookPageEventHandler eventHandler;

        public AddressBookPage(bool isCouirierSelect)
        {
            InitializeComponent();

            IsCourierSelect = isCouirierSelect;

            #if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else{
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif

            Items = AddressBookHelper.GetItems();

            ItemsListView.ItemsSource = Items;

            if(isCouirierSelect)
            {
                TitleLabel.Text = AppResources.SelectCourier;
            }
            else{
                TitleLabel.Text = AppResources.SelectRecipient;
            }
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

            eventHandler(this, new AddressBookPageEventArgs(e.SelectedItem as string));
        }
    }
}
