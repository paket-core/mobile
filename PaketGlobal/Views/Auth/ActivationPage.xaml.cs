using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ActivationPage : BasePage
    {
        public ActivationPage()
        {
            InitializeComponent();

            mnemonicLabel.Text = App.Locator.Profile.Mnemonic;

            App.Locator.DeviceService.setStausBarBlack();

#if __ANDROID__
            backButton.TranslationX = -25;
#endif
        }


        #region Button Actions

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            App.Locator.NavigationService.GoBack();
        }

        private async void CheckActivation(object sender, EventArgs e)
        {
            Unfocus();

            await WithProgressButton(checkButton, async () =>
            {
                var created = await StellarHelper.CheckAccountCreated(App.Locator.Profile.KeyPair);
                if (created)
                {
                    var trusted = await StellarHelper.CheckTokenTrusted();

                    if (trusted)
                    {
                        App.Locator.Profile.Activated = true;

                        var navigationPage = new NavigationPage(new MainPage()); 

                        Application.Current.MainPage = navigationPage;
                    }
                    else
                    {
                        var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);
                        if (added)
                        {
                            App.Locator.Profile.Activated = true;

                            var navigationPage = new NavigationPage(new MainPage()); 

                            Application.Current.MainPage = navigationPage;
                        }
                        else
                        {
                            ShowMessage("Error adding trust token");
                        }
                    }
                }
            });
        }

        void OnCopyMnemonic(object sender, EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            ShowMessage("Copied to clipboard");
        }

        void OnCopyAddress(object sender, EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(addressLabel.Text);
            ShowMessage("Copied to clipboard");
        }

        #endregion

        protected override void ToggleLayout(bool enabled)
        {
            backButton.IsEnabled = enabled;
        }
    }
}
