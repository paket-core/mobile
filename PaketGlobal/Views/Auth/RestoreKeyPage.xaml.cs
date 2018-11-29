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

         ///entrySecretKey.Text = "SBOLPN4HNTCLA3BMRS6QG62PXZUFOZ5RRMT6LPJHUPGQLBP5PZY4YFIT";
            // entrySecretKey.Text = "SB6MPNBY5C3POXIBYK25PLT6TLZ6SLDGRL4HJMNNPU7KPYOPZGKLI6OQ";
          // entrySecretKey.Text = "SAAU3WOGFO5R7KVUP52FSVIGPEDMMBN5HXZLCT3YIBQOUNY3CUOU2JMR";

            App.Locator.DeviceService.setStausBarBlack();

            EnableDisableButton();

#if __ANDROID__
            backButton.TranslationX = -30;
#elif __IOS___
#endif
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            Navigation.PopAsync(true);
        }


        private async void LoginClicked(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Unfocus();

                if (!App.Locator.FriendlyService.IsFundWorking)
                {
                    ShowErrorMessage(AppResources.RegistrationFundNotWorking);
                    return;
                }

                App.ShowLoading(true);

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

                    var result = await App.Locator.IdentityServiceClient.GetUser(kd.KeyPair.Address, null);

					if (result != null) {
						App.Locator.Profile.SetCredentials(result.UserDetails.PaketUser, kd.KeyPair.SecretSeed, kd.MnemonicString);

						var infosResult = await App.Locator.IdentityServiceClient.UserInfos();
						if (infosResult != null && !String.IsNullOrWhiteSpace(infosResult.UserDetails.PhoneNumber)) {

                            Application.Current.Properties[Constants.STORED_PHONE] = infosResult.UserDetails.PhoneNumber;

                            CheckActivation();
						} else {
							var page = new RegistrationPage(true, result.UserDetails);

							await Navigation.PushAsync(page, true);

							App.ShowLoading(false);
						}
					} else {
						App.Locator.Profile.SetCredentials(kd.KeyPair.SecretSeed, kd.MnemonicString);

						var page = new RegistrationPage(true);

						await Navigation.PushAsync(page, true);

						App.ShowLoading(false);
					}
                }
                catch (Exception ex)
                {
                    App.ShowLoading(false);

                    App.Locator.Profile.KeyPair = null;

                    if(!string.IsNullOrEmpty(entrySecretKey.Text))
                    {
                        ShowErrorMessage(AppResources.InvalidSecretKey);
                    }
                    else if (!string.IsNullOrEmpty(entryMnemonic.Text))
                    {
                        ShowErrorMessage(AppResources.InvalidMnemonic);
                    }
                    else{
                        ShowErrorMessage(ex.Message);
                    }
                }
            }
        }

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

                    //var userInfo = await App.Locator.IdentityServiceClient.UserInfos();
                    //if (userInfo != null)
                    //{
                    //    App.Locator.Profile.SetCredentials(App.Locator.Profile.UserName,
                    //                                       userInfo.UserDetails.FullName, userInfo.UserDetails.PhoneNumber, userInfo.UserDetails.Address,
                    //                                       App.Locator.Profile.KeyPair.SecretSeed, App.Locator.Profile.Mnemonic);

                    //}

                    var page = new WaitingAccountCreationPage(true,true);

                    await Navigation.PushAsync(page, true);

                    App.ShowLoading(false); 
                }
                else
                {
                    var page = new SMSVereficationPage();

                    await Navigation.PushAsync(page, true);

                    App.ShowLoading(false); 
                }
            }
            else
            {
                var page = new SMSVereficationPage();

                await Navigation.PushAsync(page, true);

                App.ShowLoading(false); 
            }
        }


        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateMnemonic(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text))
            {
                entryMnemonic.Focus();
                return false;
            }

            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            entryMnemonic.IsEnabled = enabled;
            entrySecretKey.IsEnabled = enabled;
        }

        private void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            EnableDisableButton();
        }

        private void EnableDisableButton()
        {
            if (!ValidationHelper.ValidateMnemonic(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text))
            {
                restoreButton.Disabled = true;
            }
            else{
                restoreButton.Disabled = false;
            }
        }
    }
}
