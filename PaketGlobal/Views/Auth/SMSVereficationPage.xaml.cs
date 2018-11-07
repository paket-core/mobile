using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

            verifyButton.Disabled = false;
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

                if (!App.Locator.FirendlyService.IsFundWorking)
                {
                    ShowErrorMessage(AppResources.RegistrationFundNotWorking);
                    return;
                }

                var page = new WaitingAccountCreationPage();
                await Navigation.PushAsync(page, false);
                await Task.Delay(1000);

                try
                {
                    var result = await App.Locator.IdentityServiceClient.VerifyCode(entryCode.Text);
                    if (result != null)
                    {
                        var trusted = await StellarHelper.CheckTokenTrusted();
                        if (trusted)
                        {
                            page.OpenMainPage();
                        }
                        else{
                            var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);

                            if (added)
                            {
                                page.OpenMainPage();
                            }
                            else
                            {
                                page.GoBack();

                                ShowErrorMessage(AppResources.ErrorAddTrustToken);
                            }
                        }
                    }
                    else
                    {
                        page.GoBack();
                    }
                }
                catch (Exception ex)
                {
                    page.GoBack();

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

        private void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (!ValidationHelper.ValidateTextField(entryCode.Text))
            {
                verifyButton.Disabled = true;
            }
            else{
                verifyButton.Disabled = false;
            }

        }
    }
}
