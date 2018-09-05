using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RegistrationPage : BasePage
    {
        private Boolean IsAddedInfo = false;
        private Boolean IsFinishActivation = false;
        private UserDetails UserData;

        public RegisterViewModel ViewModel
        {
            get { return BindingContext as RegisterViewModel; }
        }

        public RegistrationPage(bool isAddedInfo, UserDetails userData = null)
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

            BindingContext = new RegisterViewModel();

            IsAddedInfo = isAddedInfo;

            if (userData != null)
            {
                IsFinishActivation = true;
                UserData = userData;
            }

            if (IsAddedInfo)
            {
                generateButton.Text = AppResources.CompleteRegistration;
                titleLabel.Text = AppResources.AddInfo;
            }
            else if (isAddedInfo == false && IsFinishActivation == true)
            {
                generateButton.Text = AppResources.CompleteRegistration;
                titleLabel.Text = AppResources.AddInfo;

                entryFullName.Text = userData.FullName;
                entryUserName.Text = userData.PaketUser;
                entryPhoneNumber.Text = userData.PhoneNumber;
                entryUserAddress.Text = userData.Address;

                if (userData.PaketUser != null)
                {
                    if (userData.PaketUser.Length > 0)
                    {
                        entryUserName.IsEnabled = false;
                    }
                }
            }

            var selectCountryCommand  = new Command(() =>
            {
                var picker = new CountryPickerPage();
                picker.eventHandler = DidSelectCountryHandler;
                Navigation.PushAsync(picker, true);
            });

            XamEffects.Commands.SetTap(countryCodeLabel, selectCountryCommand);

            var selectTermsCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Constants.PAKET_PRIVACY_URL));
            });

            XamEffects.Commands.SetTap(termsOfServiceLabel, selectTermsCommand);

            App.Locator.DeviceService.setStausBarLight();
        }

        public RegistrationPage()
        {
            InitializeComponent();

            BindingContext = new RegisterViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();
        }

        private void DidSelectCountryHandler(object sender, CountryPickerPageEventArgs e)
        {
            ViewModel.PhoneCode = e.Item.CallingCode;
        }

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            Navigation.PopAsync(true);
        }

        private async void CreateAccountClicked(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Unfocus();

                App.ShowLoading(true);

                if (IsAddedInfo == false && IsFinishActivation == true)
                {
                    var updateResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.FullPhoneNumber, ViewModel.Address);

					App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);
                }
                else if (IsAddedInfo)
                {
                    try
                    {
                        //Retrievee private key
                        var kd = App.Locator.Profile.TryGetKeyData();

                        var result = await App.Locator.IdentityServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
                                                                                          ViewModel.FullPhoneNumber, ViewModel.Address, kd.KeyPair.Address);
                        if (result != null)
                        {
                            App.Locator.Profile.SetCredentials(ViewModel.UserName,
                                                               ViewModel.FullName,
                                                               ViewModel.FullPhoneNumber,
                                                               kd.KeyPair.SecretSeed,
                                                               kd.MnemonicString);
                          
                            var page = new SMSVereficationPage();
                            await Navigation.PushAsync(page, true);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);

                        ShowErrorMessage(ex.Message);
                    }
                }
                else
                {
                    try
                    {
                        var kd = Profile.GenerateKeyPairFromMnemonic();
                        App.Locator.Profile.KeyPair = kd.KeyPair;

                        var result = await App.Locator.IdentityServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
                                                                                          ViewModel.FullPhoneNumber, ViewModel.Address, kd.KeyPair.Address);
                        if (result != null)
                        {
							var infosResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.FullPhoneNumber, ViewModel.Address, kd.KeyPair.Address);

							if (infosResult != null) {

								App.Locator.Profile.SetCredentials(ViewModel.UserName,
																   ViewModel.FullName,
																   ViewModel.FullPhoneNumber,
																   kd.KeyPair.SecretSeed,
																   kd.MnemonicString);

								var page = new ViewMnemonicPage();
								await Navigation.PushAsync(page, true);
							}
                        }
                        else
                        {
                            App.Locator.Profile.KeyPair = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex);

                        App.Locator.Profile.KeyPair = null;

                        ShowErrorMessage(ex.Message);
                    }
                }

				App.ShowLoading(false);
            }
        }

        public async void CheckActivation()
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
                        App.ShowLoading(false);

                        ShowErrorMessage(AppResources.ErrorAddTrustToken);
                    }
                }
            }
            else
            {
                App.ShowLoading(false);

                ShowErrorMessage(AppResources.StellarAccountNotCreated);
            }
        }

        private void FieldCompleted(object sender, EventArgs e)
        {
            if (sender == entryUserName)
            {
                if (!ValidationHelper.ValidateTextField(entryFullName.Text))
                {
                    entryFullName.Focus();
                }
            }
            else if (sender == entryFullName)
            {
                if (!ValidationHelper.ValidateTextField(entryPhoneNumber.Text))
                {
                    entryPhoneNumber.Focus();
                }
            }
            else if (sender == entryPhoneNumber)
            {
                if (!ValidationHelper.ValidateTextField(entryUserAddress.Text))
                {
                    entryUserAddress.Focus();
                }
            }
        }

        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateTextField(entryUserName.Text))
            {
                entryUserName.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryFullName.Text))
            {
                entryFullName.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryPhoneNumber.Text))
            {
                entryPhoneNumber.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryUserAddress.Text))
            {
                entryUserAddress.Focus();
                return false;
            }

            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            entryUserName.IsEnabled = enabled;
            entryFullName.IsEnabled = enabled;
            entryUserAddress.IsEnabled = enabled;
            entryPhoneNumber.IsEnabled = enabled;
        }

    }
}
