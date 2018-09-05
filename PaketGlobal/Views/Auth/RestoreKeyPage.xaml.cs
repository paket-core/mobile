﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RestoreKeyPage : BasePage
    {
        public RestoreKeyPage()
        {
            InitializeComponent();

            //entrySecretKey.Text = "SDAORBDDZ2ZZQZKZWY73YF3RZU6ZS4DGHOL5VUGOM56GWXUV5GZKVBGF";

            App.Locator.DeviceService.setStausBarBlack();

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
                    App.Locator.AccountService.ActivationAddress = "";

                    var result = await App.Locator.IdentityServiceClient.GetUser(kd.KeyPair.Address, null);

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

                    App.Locator.Profile.KeyPair = null;

                    ShowErrorMessage(ex is OutOfMemoryException ? "Secret key is invalid" : ex.Message);
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

                    var navigationPage = new NavigationPage(new MainPage());

                    Application.Current.MainPage = navigationPage;

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
    }
}
