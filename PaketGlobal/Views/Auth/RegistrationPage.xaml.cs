using System;
using System.Globalization;
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
            entryPhoneNumber.TranslationY = 3;
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
				ViewModel.UserName = userData.PaketUser;
				ViewModel.FullName = userData.FullName;
				ViewModel.Address = userData.Address;
                if(ViewModel.Address==null)
                {
                    ViewModel.Address = ISO3166.GetCurrentCountryName();
                }
            }
            else{
                ViewModel.Address = ISO3166.GetCurrentCountryName();
            }

            if (IsAddedInfo)
            {
                GenerateButton.Text = AppResources.CompleteRegistration;
                titleLabel.Text = AppResources.AddInfo;
            }
            else if (isAddedInfo == false && IsFinishActivation == true)
            {
                GenerateButton.Text = AppResources.CompleteRegistration;
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

            var whyTapCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Constants.WHY_BLOCKED));
            });

            XamEffects.Commands.SetTap(whyButton, whyTapCommand);


            App.Locator.DeviceService.setStausBarLight();

            if (ViewModel.Address.ToLower() == "united states of america" || ViewModel.Address.ToLower() == "usa")
            {
                ErrorView.IsVisible = true;
            }

            EnableDisableButton();
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

        private void DidSelectAddressHandler(object sender, CountryPickerPageEventArgs e)
        {
            ViewModel.Address = e.Item.Name;

            if (ViewModel.Address.ToLower() == "united states of america" || ViewModel.Address.ToLower() == "usa")
            {
                ErrorView.IsVisible = true;
            }
            else{
                ErrorView.IsVisible = false;
            }

            EnableDisableButton();
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

                if (IsFinishActivation == true)
                {
					//Retrievee private key
					var kd = App.Locator.Profile.TryGetKeyData();

					var updateResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.FullPhoneNumber, ViewModel.Address);

					if (updateResult != null) {
						App.Locator.Profile.SetCredentials(ViewModel.UserName,
														   ViewModel.FullName,
														   ViewModel.FullPhoneNumber,
						                                   ViewModel.Address,
														   kd.KeyPair.SecretSeed,
														   kd.MnemonicString);

                        Application.Current.Properties[Constants.STORED_PHONE] = ViewModel.FullPhoneNumber;

                        if (App.Locator.Profile.MnemonicGenerated) {
							var page = new ViewMnemonicPage();
							await Navigation.PushAsync(page, true);
						} else {
							var page = new SMSVereficationPage();
							await Navigation.PushAsync(page, true);
						}
					}
                }
                else if (IsAddedInfo)
                {
                    try
                    {
                        //Retrievee private key
                        var kd = App.Locator.Profile.TryGetKeyData();

                        var result = await App.Locator.IdentityServiceClient.RegisterUser(ViewModel.UserName, kd.KeyPair.Address);
                        if (result != null)
                        {
							IsFinishActivation = true;

							App.Locator.Profile.SetCredentials(ViewModel.UserName, kd.KeyPair.SecretSeed, kd.MnemonicString);

							var infosResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.FullPhoneNumber, ViewModel.Address, kd.KeyPair.Address);

							if (infosResult != null) {
								App.Locator.Profile.SetCredentials(ViewModel.UserName,
																   ViewModel.FullName,
																   ViewModel.FullPhoneNumber,
								                                   ViewModel.Address,
																   kd.KeyPair.SecretSeed,
																   kd.MnemonicString);

                                Application.Current.Properties[Constants.STORED_PHONE] = ViewModel.FullPhoneNumber;

                                if (App.Locator.Profile.MnemonicGenerated) {
									var page = new ViewMnemonicPage();
									await Navigation.PushAsync(page, true);
								} else {
									var page = new SMSVereficationPage();
									await Navigation.PushAsync(page, true);
								}
							}
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

                        var result = await App.Locator.IdentityServiceClient.RegisterUser(ViewModel.UserName, kd.KeyPair.Address);
						if (result != null)
                        {
							IsFinishActivation = true;
							App.Locator.Profile.SetCredentials(ViewModel.UserName, kd.KeyPair.SecretSeed, kd.MnemonicString);
							App.Locator.Profile.MnemonicGenerated = true;

							var infosResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.FullPhoneNumber, ViewModel.Address, kd.KeyPair.Address);

							if (infosResult != null) {
								App.Locator.Profile.SetCredentials(ViewModel.UserName,
																   ViewModel.FullName,
																   ViewModel.FullPhoneNumber,
								                                   ViewModel.Address,
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
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryFullName.Text))
            {
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryPhoneNumber.Text))
            {
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(entryUserAddress.Text))
            {
                return false;
            }
            else if (entryUserAddress.Text.ToLower() == "united states of america" || entryUserAddress.Text.ToLower() == "usa")
            {
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

        private void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            EnableDisableButton();
        }

        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            entryUserAddress.Unfocus();

            var picker = new AddressPickerPage();
            picker.eventHandler = DidSelectAddressHandler;
            Navigation.PushAsync(picker, true);
        }

        private void ReadClicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(Constants.WHY_BLOCKED));
        }

        private void EnableDisableButton()
        {
            if (!IsValid())
            {
                GenerateButton.Disabled = true;
            }
            else
            {
                GenerateButton.Disabled = false;
            }
        }
    }
}
