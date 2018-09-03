using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ConfirmMnemonicPage : BasePage
    {
        public ConfirmMnemonicPage()
        {
            InitializeComponent();

            App.Locator.DeviceService.setStausBarBlack();

#if __ANDROID__
            backButton.TranslationX = -25;
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        private void OnBack(object sender, EventArgs e)
        {
            App.Locator.NavigationService.GoBack();
        }


        private void OnNext(object sender, EventArgs e)
        {
            var page = new ActivationPage();
            Navigation.PushAsync(page, false);
        }
    }
}
