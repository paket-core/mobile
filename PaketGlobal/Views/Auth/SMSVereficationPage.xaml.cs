using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class SMSVereficationPage : BasePage
    {
        public SMSVereficationPage()
        {
            InitializeComponent();

#if __ANDROID__
            backButton.TranslationX = -30;
#else
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                backButton.TranslationY = 30;
                titleLabel.TranslationY = 30;
            }
#endif
        }

		protected async override void OnAppearing()
		{
			var fl = firstLoad;

			base.OnAppearing();

			if (fl) {
				await App.Locator.IdentityServiceClient.SendVerification();
			}
		}

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            App.Locator.Workspace.Logout();
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
                        var trusted = await StellarHelper.CheckTokenTrusted();
                        if (trusted)
                        {
                            OpenMainPage();
                        }
                        else{
                            var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);

                            if (added)
                            {
                                OpenMainPage();
                            }
                            else
                            {
                                ShowErrorMessage(AppResources.ErrorAddTrustToken);
                                App.ShowLoading(false);
                            }
                        }
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

        private void OpenMainPage()
        {
            App.Locator.Profile.Activated = true;

            var navigationPage = new NavigationPage(new MainPage());

            Application.Current.MainPage = navigationPage;

            App.ShowLoading(false);  
        }
    }
}
