using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RegistrationPage : BasePage
    {
        private Boolean IsAddedInfo = false;

        public RegisterViewModel ViewModel
        {
            get { return BindingContext as RegisterViewModel; }
        }

        public RegistrationPage(bool isAddedInfo)
        {
            InitializeComponent();

            BindingContext = new RegisterViewModel();

            IsAddedInfo = isAddedInfo;

            if(IsAddedInfo){
                generateButton.Text = "Complete Registration";
                titleLabel.Text = "Add Info";
                pickerCurrency.IsVisible = false;
            }

            App.Locator.DeviceService.setStausBarBlack();
        }

        public RegistrationPage()
        {
            InitializeComponent();

            BindingContext = new RegisterViewModel();
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

              //  await WithProgressButton(generateButton, async () =>
              //  {
                    if (IsAddedInfo){
                        try
                        {
                            //Retrievee private key
                            var kd = App.Locator.Profile.TryGetKeyData();

                            var result = await App.Locator.FundServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
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
                                ShowMessage("Error adding info");
                            }
                        }
                        catch (Exception ex)
                        {
                            App.ShowLoading(false);
                            System.Diagnostics.Debug.WriteLine(ex);
                            ShowMessage(ex.Message);
                        }
                    }
                    else{
                        try
                        {
                            var kd = Profile.GenerateKeyPairFromMnemonic();
                            App.Locator.Profile.KeyPair = kd.KeyPair;

                            var result = await App.Locator.FundServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
                                                                                          ViewModel.PhoneNumber, ViewModel.Address, kd.KeyPair.Address);
                            if (result != null)
                            {
                                App.Locator.Profile.SetCredentials(ViewModel.UserName,
                                                                   ViewModel.FullName,
                                                                   ViewModel.PhoneNumber,
                                                                   kd.KeyPair.SecretSeed,
                                                                   kd.MnemonicString);


                               // var createResult = await App.Locator.FundServiceClient.PurchaseXLMs(5, ViewModel.PaymentCurrency.Value);
                                var createResult = await App.Locator.FundServiceClient.CreateStellarAccount(ViewModel.PaymentCurrency.Value);

                                if (createResult != null)
                                {
                                    App.ShowLoading(false);

                                    App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);
                                }
                                else
                                {
                                    App.ShowLoading(false);

                                    ShowMessage("Error creating Stellar account");

                                    App.Locator.Profile.KeyPair = null;
                                }
                            }
                            else
                            {
                                App.ShowLoading(false);

                                ShowMessage("Error registering user");

                                App.Locator.Profile.KeyPair = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            App.ShowLoading(false);

                            System.Diagnostics.Debug.WriteLine(ex);

                            App.Locator.Profile.KeyPair = null;

                            ShowMessage(ex.Message);
                        }
                    }
               // });
            }
        }

        public async void CheckActivation()
        {
         //   await WithProgressButton(generateButton, async () => {
                var created = await StellarHelper.CheckAccountCreated(App.Locator.Profile.KeyPair);
                if (created)
                {
                    var trusted = await StellarHelper.CheckTokenTrusted();
                    if (trusted)
                    {

                        App.ShowLoading(false);

                        App.Locator.Profile.Activated = true;

                        var navigationPage = new NavigationPage(new MainPage()); 

                        Application.Current.MainPage = navigationPage;
                    }
                    else
                    {
                        var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);
                        if (added)
                        {
                            App.ShowLoading(false);

                            App.Locator.Profile.Activated = true;

                            var navigationPage = new NavigationPage(new MainPage());

                            Application.Current.MainPage = navigationPage;
                        }
                        else
                        {
                            App.ShowLoading(false);
                            ShowMessage("Error adding trust token");
                        }
                    }
                }
                else
                {
                    App.ShowLoading(false);
                    ShowMessage("Stellar account isn't created yet");
                }
           // });
        }

#endregion

        private void FieldCompleted(object sender, EventArgs e)
        {
            if(sender == entryUserName){
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
                if(pickerCurrency.SelectedItem==null && IsAddedInfo==false)
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
            else if (pickerCurrency.SelectedItem==null && IsAddedInfo==false)
            {
                ShowMessage("Please select payment currency");
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
