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
            addressLabel.Text = App.Locator.AccountService.ActivationAddress;

            if (addressLabel.Text != null)
            {
                if (addressLabel.Text.Length > 2)
                {
                    if (addressLabel.Text.Substring(0, 2) == "0x")
                    {
                        sendLabel.Text = AppResources.ActivationEth;
                    }
                }
                else{
                    LoadActivationAdderss();   
                }
            }

            App.Locator.DeviceService.setStausBarBlack();

#if __ANDROID__
            backButton.TranslationX = -25;
#else
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                mnemonicBg.TranslationY = mnemonicBg.TranslationY - 35;
                mnemonicStack.TranslationY = mnemonicStack.TranslationY - 10;
            }
            else{
                if (App.Locator.DeviceService.IsIphonePlus() == true)
                {
                    mnemonicStack.TranslationY = mnemonicStack.TranslationY+5;
                }
                else{
                    mnemonicStack.TranslationY = mnemonicStack.TranslationY - 10;
                }
                mnemonicBg.TranslationY = mnemonicBg.TranslationY - 10;
            }
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        private void LoadActivationAdderss()
        {
            
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
                            ShowErrorMessage(AppResources.ErrorAddTrustToken);
                        }
                    }
                }
            });
        }

        void OnCopyMnemonic(object sender, EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            ShowMessage(AppResources.Copied);
        }

        void OnCopyAddress(object sender, EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(addressLabel.Text);
            ShowMessage(AppResources.Copied);
        }

        #endregion

        protected override void ToggleLayout(bool enabled)
        {
            backButton.IsEnabled = enabled;
        }
    }
}
