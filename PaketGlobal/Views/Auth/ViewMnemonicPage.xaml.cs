using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ViewMnemonicPage : BasePage
    {
        public ViewMnemonicPage()
        {
            InitializeComponent();

            App.Locator.DeviceService.setStausBarBlack();

#if __ANDROID__
            backButton.TranslationX = -30;
#endif
            mnemonicLabel.Text = App.Locator.Profile.Mnemonic;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        private void OnBack(object sender, EventArgs e)
        {
            Navigation.PopAsync(true);
        }

        private void OnCopyMnemonic(object sender, EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            ShowMessage(AppResources.Copied);
        }

        private void OnNext(object sender, EventArgs e)
        {
            var page = new ConfirmMnemonicPage();
            Navigation.PushAsync(page, false);
        }

    }
}
