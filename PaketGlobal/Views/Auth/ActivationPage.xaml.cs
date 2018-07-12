using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ActivationPage : BasePage
    {
        private Command LongMnemonicTapCommand;
        private Command LongAddressTapCommand;

        public ActivationPage()
        {
            InitializeComponent();
            AddLongTaps();

            mnemonicLabel.Text = App.Locator.Profile.Mnemonic;
        }

        private void AddLongTaps()
        {
            LongMnemonicTapCommand = new Command(() =>
            {
                App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            });

            XamEffects.Commands.SetLongTap(mnemonicLabel, LongMnemonicTapCommand);

            LongAddressTapCommand = new Command(() =>
            {
                App.Locator.ClipboardService.SendTextToClipboard(addressLabel.Text);
            });

            XamEffects.Commands.SetLongTap(addressLabel, LongAddressTapCommand);
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

        #endregion

        protected override void ToggleLayout(bool enabled)
        {
            backButton.IsEnabled = enabled;
        }
    }
}
