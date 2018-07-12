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
            }
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

                await WithProgressButton(generateButton, async () =>
                {
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
                                ShowMessage("Error adding info");
                            }
                        }
                        catch (Exception ex)
                        {
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


                                var createResult = await App.Locator.FundServiceClient.CreateStellarAccount(ViewModel.PaymentCurrency.Value);
                                if (createResult != null)
                                {
                                    App.Locator.NavigationService.NavigateTo(Locator.ActivationPage);
                                }
                                else
                                {
                                    ShowMessage("Error creating Stellar account");

                                    App.Locator.Profile.KeyPair = null;
                                }
                            }
                            else
                            {
                                ShowMessage("Error registering user");

                                App.Locator.Profile.KeyPair = null;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine(ex);

                            App.Locator.Profile.KeyPair = null;

                            ShowMessage(ex.Message);
                        }
                    }
                });
            }
        }

        public async void CheckActivation()
        {
            await WithProgressButton(generateButton, async () => {
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
                    ShowMessage("Stellar account isn't created yet");
                }
            });
        }

#endregion

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
            else if (pickerCurrency.SelectedItem==null)
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
