using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class SettingsPage : BasePage
    {
        public SettingsPage()
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


            BridgeEntry.Text = Config.BridgeServerUrl;
            FundEntry.Text = Config.IdentityServerUrl;
            RouteEntry.Text = Config.RouteServerUrl;

        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void SaveClicked(object sender, System.EventArgs e)
        {
            Unfocus();

            if (Uri.IsWellFormedUriString(BridgeEntry.Text, UriKind.Absolute) && 
                Uri.IsWellFormedUriString(RouteEntry.Text, UriKind.Absolute) &&
                Uri.IsWellFormedUriString(FundEntry.Text, UriKind.Absolute))
            {
                Config.BridgeServerUrl = BridgeEntry.Text;
                Config.RouteServerUrl = RouteEntry.Text;
                Config.IdentityServerUrl = FundEntry.Text;

                Application.Current.Properties[Config.BridgeService] = Config.BridgeServerUrl;
                Application.Current.Properties[Config.IdentityService] = Config.IdentityServerUrl;
                Application.Current.Properties[Config.RouteService] = Config.RouteServerUrl;    
                Application.Current.SavePropertiesAsync();

                ShowErrorMessage(AppResources.SettingsChanged);
            }
            else{
                EventHandler handleHandler = (sv, ev) => {
                    if (!Uri.IsWellFormedUriString(BridgeEntry.Text, UriKind.Absolute))
                    {
                        BridgeEntry.Focus();
                    }
                    else if (!Uri.IsWellFormedUriString(RouteEntry.Text, UriKind.Absolute))
                    {
                        RouteEntry.Focus();
                    }
                    else if (!Uri.IsWellFormedUriString(FundEntry.Text, UriKind.Absolute))
                    {
                        FundEntry.Focus();
                    }
                };

                ShowErrorMessage(AppResources.InvalidUrl,false,handleHandler);
            }

         
        }
    }
}
