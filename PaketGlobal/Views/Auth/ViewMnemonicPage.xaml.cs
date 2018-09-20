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

            var mnemonic = App.Locator.Profile.Mnemonic;
            var array = mnemonic.Split(' ');

            mnemonicLabel_1.Text = array[0];
            mnemonicLabel_2.Text = array[1];
            mnemonicLabel_3.Text = array[2];
            mnemonicLabel_4.Text = array[3];
            mnemonicLabel_5.Text = array[4];
            mnemonicLabel_6.Text = array[5];
            mnemonicLabel_7.Text = array[6];
            mnemonicLabel_8.Text = array[7];
            mnemonicLabel_9.Text = array[8];
            mnemonicLabel_10.Text = array[9];
            mnemonicLabel_11.Text = array[10];
            mnemonicLabel_12.Text = array[11];


#if __ANDROID__
            backButton.TranslationX = -30;
#endif
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
            App.Locator.ClipboardService.SendTextToClipboard(App.Locator.Profile.Mnemonic);
            ShowMessage(AppResources.Copied);
        }

        private void OnNext(object sender, EventArgs e)
        {
            var page = new ActivationPage();
            Navigation.PushAsync(page, true);
        }

    }
}
