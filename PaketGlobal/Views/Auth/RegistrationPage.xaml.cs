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
                pickerCurrency.IsVisible = false;
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

            App.Locator.DeviceService.setStausBarBlack();

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                botBg.TranslationY = botBg.TranslationY - 40;
                botStack.TranslationY = botStack.TranslationY - 40;
            }
#endif
        }

        public RegistrationPage()
        {
            InitializeComponent();

            BindingContext = new RegisterViewModel();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarBlack();
        }

        #region Button Actions

        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            App.Locator.NavigationService.GoBack();
        }

        private async void CreateAccountClicked(object sender, EventArgs e)
        {
            if (IsValid())
            {
                Unfocus();

                App.ShowLoading(true);

                if (IsAddedInfo == false && IsFinishActivation == true)
                {
                    var updateResult = await App.Locator.IdentityServiceClient.UserInfos(ViewModel.FullName, ViewModel.PhoneNumber, ViewModel.Address);

                    var createResult = await App.Locator.IdentityServiceClient.PurchaseXLMs(5, ViewModel.PaymentCurrency.Value);

                    if (createResult != null)
                    {
                        App.Locator.AccountService.ActivationAddress = createResult.PaymentAddress;

                        App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);

                        App.ShowLoading(false);
                    }
                    else
                    {
                        App.ShowLoading(false);

                        App.Locator.Profile.KeyPair = null;
                    }
                }
                else if (IsAddedInfo)
                {
                    try
                    {
                        //Retrievee private key
                        var kd = App.Locator.Profile.TryGetKeyData();

                        var result = await App.Locator.IdentityServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
                                                                                      ViewModel.PhoneNumber, ViewModel.Address, kd.KeyPair.Address);
                        if (result != null)
                        {
                            App.Locator.Profile.SetCredentials(ViewModel.UserName,
                                                               ViewModel.FullName,
                                                               ViewModel.PhoneNumber,
                                                               kd.KeyPair.SecretSeed,
                                                               kd.MnemonicString);
                            CheckActivation();
                        }
                        else
                        {
                            App.ShowLoading(false);
                        }
                    }
                    catch (Exception ex)
                    {
                        App.ShowLoading(false);

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
                                                                                      ViewModel.PhoneNumber, ViewModel.Address, kd.KeyPair.Address);
                        if (result != null)
                        {
                            App.Locator.Profile.SetCredentials(ViewModel.UserName,
                                                               ViewModel.FullName,
                                                               ViewModel.PhoneNumber,
                                                               kd.KeyPair.SecretSeed,
                                                               kd.MnemonicString);


                            var createResult = await App.Locator.IdentityServiceClient.PurchaseXLMs(5, ViewModel.PaymentCurrency.Value);

                            if (createResult != null)
                            {
                                App.Locator.AccountService.ActivationAddress = createResult.PaymentAddress;

                                App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);

                                App.ShowLoading(false);
                            }
                            else
                            {
                                App.ShowLoading(false);

                                App.Locator.Profile.KeyPair = null;
                            }
                        }
                        else
                        {
                            App.ShowLoading(false);

                            App.Locator.Profile.KeyPair = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        App.ShowLoading(false);

                        System.Diagnostics.Debug.WriteLine(ex);

                        App.Locator.Profile.KeyPair = null;

                        ShowErrorMessage(ex.Message);
                    }
                }
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

        #endregion

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
            else if (sender == entryUserAddress)
            {
                if (pickerCurrency.SelectedItem == null && IsAddedInfo == false)
                {
                    pickerCurrency.Focus();
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
            else if (pickerCurrency.SelectedItem == null && IsAddedInfo == false)
            {
                EventHandler handleCurrencyHandler = (s, e) => {
                    pickerCurrency.Focus();
                };

                ShowErrorMessage(AppResources.PleaseSelectPaymentCurrency, false, handleCurrencyHandler);

                return false;
            }

            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            backButton.IsEnabled = enabled;
            entryUserName.IsEnabled = enabled;
            entryFullName.IsEnabled = enabled;
            entryUserAddress.IsEnabled = enabled;
            entryPhoneNumber.IsEnabled = enabled;
            pickerCurrency.IsEnabled = enabled;
        }


    }
}
