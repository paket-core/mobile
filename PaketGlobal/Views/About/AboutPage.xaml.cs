using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class AboutPage : BasePage
    {
        public AboutPage()
        {
            InitializeComponent();

            App.Locator.DeviceService.setStausBarLight();

            VersionLabel.Text = App.Locator.AppInfoService.AppVersion;
            GitLabel.Text = App.Locator.AppInfoService.GitCommit;

            #if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 25;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
#endif

            if (App.Locator.AccountService.ShowNotifications)
            {
                NotificationsButton.Image = "swift_on.png";
            }
            else
            {
                NotificationsButton.Image = "swift_off.png";
            }

            var appLinkCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Constants.APP_URL));
            });
            XamEffects.Commands.SetTap(AppUrlView, appLinkCommand);

            var paketLinkCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Constants.PAKET_URL));
            });
            XamEffects.Commands.SetTap(PaketUrlView, paketLinkCommand);

            var languageCommand = new Command(async () =>
            {
                var page = new LanguagePage();

                var mainPage = App.Current.MainPage;

                await mainPage.Navigation.PushAsync(page);
            });
            XamEffects.Commands.SetTap(LanguageView, languageCommand);
        }

        protected override void OnAppearing()
        {
            App.Locator.DeviceService.setStausBarLight();

            base.OnAppearing();
        }

        private void NotificationsClicked(object sender, System.EventArgs e)
        {
            bool enabled = !App.Locator.AccountService.ShowNotifications;

            if (enabled)
            {
                NotificationsButton.Image = "swift_on.png";
            }
            else
            {
                NotificationsButton.Image = "swift_off.png";
            }

            App.Locator.AccountService.ShowNotifications = enabled;
        }
    }
}
