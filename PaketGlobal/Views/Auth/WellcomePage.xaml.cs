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

#if __IOS__
            SettingsButton.TranslationY = 20;
#endif

        }

        protected async override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            App.Locator.Packages.AvailablePackagesList = new List<AvaiablePackage>();
            App.Locator.Packages.PackagesList = new List<Package>();

            if (fl) {
                if (!String.IsNullOrEmpty(App.Locator.Profile.UserName)) {
                    if (!String.IsNullOrEmpty(App.Locator.Profile.PhoneNumber)) {
                        if (App.Locator.Profile.MnemonicGenerated) {
                            var page = new ViewMnemonicPage();
                            await Navigation.PushAsync(page, true);
                        } else {
                            var page = new SMSVereficationPage();
                            await Navigation.PushAsync(page, true);
                        }

                        App.ShowLoading(false);
                    } else {
                        var userDetails = new UserDetails {
                            PaketUser = App.Locator.Profile.UserName,
                            FullName = App.Locator.Profile.FullName,
                            Address = App.Locator.Profile.Address
                        };
                        var page = new RegistrationPage(true, userDetails);

                        await Navigation.PushAsync(page, true);
                    }
                }
            } else {
                App.Locator.Profile.DeleteCredentials();
            }

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

        private void SettingsClicked(object sender, System.EventArgs e)
        {
            var page = new SettingsPage();
            Navigation.PushAsync(page);
        }
    }
}
