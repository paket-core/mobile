using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class WellcomePage : BasePage
    {
        public WellcomePage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        private void OnRegistrationClicked(object sender, System.EventArgs e)
        {
            var page = new RegistrationPage(false);
            Navigation.PushAsync(page, true);
        }

        private void OnLogInClicked(object sender, System.EventArgs e)
        {
            var page = new RestoreKeyPage();
            Navigation.PushAsync(page, true);
        }
    }
}
