using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class TransactionsPage : BasePage
    {
        public TransactionsPage()
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

            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;

                BindingContext = await PackageHelper.GetPackageDetails(ViewModel.PaketId);

                PullToRefresh.IsRefreshing = false;
            });

            PullToRefresh.RefreshCommand = refreshCommand;
        }

        private async void OnBack(object sender, System.EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }
    }
}
