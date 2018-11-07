using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class UnderConstructionPage : ContentPage
    {
        private bool IsInternetError;

        public UnderConstructionPage(bool isInternetError = false)
        {
            InitializeComponent();

            IsInternetError = isInternetError;

            if(IsInternetError)
            {
                DetailLabel.Text = AppResources.NoInternetConnection;
            }

            App.Locator.FriendlyService.Pause();
        }

        private async void RefreshInfo()
        {
            await App.Locator.Packages.Load();
            await App.Locator.Wallet.Load();
            await App.Locator.ProfileModel.Load();
        }

        private async void Handle_Clicked(object sender, System.EventArgs e)
        {
            if(IsInternetError)
            {
                if(App.Locator.FriendlyService.IsConnected())
                {
                    RefreshInfo();

                    await Navigation.PopModalAsync(true);

                    App.IsShowedFriendlyScreen = false;
                }
            }
            else{
                App.ShowLoading(true);

                var working = await App.Locator.FriendlyService.CheckServers();

                App.ShowLoading(false);

                if (working)
                {
                    App.Locator.FriendlyService.Resume();

                    await Navigation.PopModalAsync(true);

                    App.IsShowedFriendlyScreen = false;
                }
            }
        }
    }
}
