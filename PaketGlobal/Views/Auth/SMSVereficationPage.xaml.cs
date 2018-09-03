﻿using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class SMSVereficationPage : BasePage
    {
        public SMSVereficationPage()
        {
            InitializeComponent();

#if __ANDROID___
            backButton.TranslationX = -25;
#else
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                backButton.TranslationY = 30;
                titleLabel.TranslationY = 30;
            }
#endif
        }

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            Navigation.PopToRootAsync(true);
        }

        private async void OnVerify(object sender, EventArgs e)
        {
            if (!ValidationHelper.ValidateTextField(entryCode.Text))
            {
                entryCode.Focus();
            }
            else
            {
                Unfocus();

                App.ShowLoading(true);

                try
                {
                    var result = await App.Locator.IdentityServiceClient.VerifyCode(entryCode.Text);
                    if (result != null)
                    {
                        App.Locator.Profile.Activated = true;

                        var navigationPage = new NavigationPage(new MainPage());

                        Application.Current.MainPage = navigationPage;

                        App.ShowLoading(false);
                    }
                    else
                    {
                        App.ShowLoading(false);
                    }
                }
                catch (Exception ex)
                {
                    App.ShowLoading(false);

                    ShowErrorMessage(ex.Message);
                }
            }
        }
    }
}
