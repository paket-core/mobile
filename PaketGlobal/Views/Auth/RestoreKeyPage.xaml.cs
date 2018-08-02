using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RestoreKeyPage : BasePage
    {
        public RestoreKeyPage()
        {
            InitializeComponent();

          //  entrySecretKey.Text = "SDN6PSEJGHJXYOIW4ZONE64KHOPPDNQ6QNIZDPPMC4B27RWFBNYAABED";

            if (!String.IsNullOrWhiteSpace(App.Locator.Profile.Pubkey))
            {
                if (App.Locator.Profile.UserName != null)
                {
                    CheckActivation();
                }
            }

            App.Locator.DeviceService.setStausBarBlack();

            #if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                botBg.TranslationY = botBg.TranslationY - 40;
                botStack.TranslationY = botStack.TranslationY - 40;
            }
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

#region Buttons Actions

        void OnGenerate(object sender, EventArgs e)
        {
            Unfocus();

            var page = new RegistrationPage(false);

            Navigation.PushAsync(page,true);
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Unfocus();

                App.ShowLoading(true);

              //  await WithProgressButton(restoreButton, async () =>
              //  {
                    try
                    {
                        Profile.KeyData kd;

                        if (!String.IsNullOrWhiteSpace(entrySecretKey.Text))
                        {
                            kd = Profile.GenerateKeyPairFromSeed(entrySecretKey.Text);
                        }
                        else
                        {
                            //Generate private key
                            kd = Profile.GenerateKeyPairFromMnemonic(entryMnemonic.Text);
                        }

                        App.Locator.Profile.KeyPair = kd.KeyPair;

                        var result = await App.Locator.FundServiceClient.GetUser(kd.KeyPair.Address, null);

                        App.Locator.Profile.SetCredentials(result?.UserDetails?.PaketUser,
                                                               result?.UserDetails?.FullName,
                                                               result?.UserDetails?.PhoneNumber,
                                                               kd.KeyPair.SecretSeed,
                                                               kd.MnemonicString);

                        if (result != null)
                        {
                            CheckActivation();
                        }
                        else
                        {

                            var page = new RegistrationPage(true);

                            await Navigation.PushAsync(page, true);

                            App.ShowLoading(false);

                        }
                    }
                    catch (Exception ex)
                    {

                         App.ShowLoading(false);

                        System.Diagnostics.Debug.WriteLine(ex);

                        App.Locator.Profile.KeyPair = null;

                        ShowMessage(ex is OutOfMemoryException ? "Secret key is invalid" : ex.Message);
                    }
               // });
            }
        }
#endregion

        public async void CheckActivation()
        {
            Unfocus();

            var created = await StellarHelper.CheckAccountCreated(App.Locator.Profile.KeyPair);

            if (created)
            {
                var trusted = await StellarHelper.CheckTokenTrusted();
                if (trusted)
                {
                    App.Locator.Profile.Activated = true;

                    var navigationPage = new NavigationPage(new MainPage());

                    Application.Current.MainPage = navigationPage;

                    App.ShowLoading(false);
                }
                else
                {
                    var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);

                    if (added)
                    {
                        App.Locator.Profile.Activated = true;

                        var navigationPage = new NavigationPage(new MainPage());

                        Application.Current.MainPage = navigationPage;

                        App.ShowLoading(false);
                    }
                    else
                    {
                        ShowMessage("Error adding trust token");
                        App.ShowLoading(false);
                    }
                }
            }
            else
            {
                App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);
                App.ShowLoading(false);
            }
        }


		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateMnemonic(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text)) {
				entryMnemonic.Focus();
				return false;
			}

			return true;
		}

        protected override void ToggleLayout(bool enabled)
        {
            generateButton.IsEnabled = enabled;
            entryMnemonic.IsEnabled = enabled;
            entrySecretKey.IsEnabled = enabled;
        }
    }
}
