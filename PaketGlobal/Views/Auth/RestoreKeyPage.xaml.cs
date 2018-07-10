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

            entrySecretKey.Text = "SDGCA3ZKGA55PAGIPDELHATVOUBYEVTU4A7NJVVQ5JKMEOQ3532TOWK6";

            if (!String.IsNullOrWhiteSpace(App.Locator.Profile.Pubkey))
            {
                if (App.Locator.Profile.UserName != null)
                {
                    restoreButton.IsBusy = true;

                    CheckActivation();
                }
            }
        }

#region Buttons Actions

        void OnGenerate(object sender, EventArgs e)
        {
            App.Locator.NavigationService.NavigateTo(Locator.RegistrationPage);
        }

        private async void LoginClicked(object sender, EventArgs e)
        {
            if (IsValid())
            {
                await WithProgressButton(restoreButton, async () =>
                {
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
                            App.Locator.NavigationService.NavigateTo(Locator.RegistrationPage,true);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);

                        App.Locator.Profile.KeyPair = null;

                        ShowMessage(ex is OutOfMemoryException ? "Secret key is invalid" : ex.Message);
                    }
                });
            }
        }
#endregion

        public async void CheckActivation()
        {
            await WithProgressButton(restoreButton, async () =>
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
                else
                {
                    App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);
                }
            });

        }


        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateTextField(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text))
            {
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