using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ActivationPage : BasePage
    {
        //private Command MnemonicTapCommand;
        //private Command AddressTapCommand;

        public ActivationPage()
        {
            InitializeComponent();
            AddLongTaps();

            mnemonicLabel.Text = App.Locator.Profile.Mnemonic;
        }

        private void AddLongTaps()
        {
            //MnemonicTapCommand = new Command(() =>
            //{
            //    App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            //});

            //XamEffects.Commands.SetTap(mnemonicStackView, MnemonicTapCommand);

            //AddressTapCommand = new Command(() =>
            //{
            //    App.Locator.ClipboardService.SendTextToClipboard(addressLabel.Text);
            //});

            //XamEffects.Commands.SetTap(btcStackView, AddressTapCommand);
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
                        Application.Current.MainPage = new MainPage();
                    }
                    else
                    {
                        var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);
                        if (added)
                        {
                            App.Locator.Profile.Activated = true;
                            Application.Current.MainPage = new MainPage();
                        }
                        else
                        {
                            ShowMessage("Error adding trust token");
                        }
                    }
                }
            });
        }

        void OnCopyMnemonic(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            ShowMessage("Copied to clipboard");

        }

        void OnCopyAddress(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(addressLabel.Text);
            ShowMessage("Copied to clipboard");
        }

        #endregion

        protected override void ToggleLayout(bool enabled)
        {
            App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
        }
    }
}
