using System;
using stellar_dotnetcore_sdk;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class LoginPage : BasePage
	{
		public RegisterViewModel ViewModel {
			get { return BindingContext as RegisterViewModel; }
		}

		public LoginPage()
		{
			InitializeComponent();

			Title = "Restore Private Key";

			BindingContext = new RegisterViewModel();

			if (!String.IsNullOrWhiteSpace(App.Locator.Profile.Pubkey)) {
				if (App.Locator.Profile.UserName != null) {
					Title = "Activate Account";
					ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
					layoutLogin.IsVisible = false;
					layoutRegistration.IsVisible = false;
					layoutFundPrompt.IsVisible = true;
					layoutProvideInfo.IsVisible = false;
					entryMnemonicPrompt.Text = App.Locator.Profile.Mnemonic;
					CheckActivation();
				} else {
					Title = "Add Info";
					ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
					layoutLogin.IsVisible = false;
					layoutRegistration.IsVisible = false;
					layoutFundPrompt.IsVisible = false;
					layoutProvideInfo.IsVisible = true;
				}
			}
		}

		protected override bool OnBackButtonPressed()
		{
			if (layoutRegistration.IsVisible) {
				AlreadyRegisteredClicked(null, EventArgs.Empty);
			}

			return true;
		}

		#region Entry Handlers

		void LoginUserNameCompleted(object sender, EventArgs e)
		{
			LoginClicked(null, EventArgs.Empty);
		}

		void LoginSecretCompleted(object sender, System.EventArgs e)
		{
			LoginClicked(null, EventArgs.Empty);
		}

		void UserNameCompleted(object sender, EventArgs e)
		{
			entryFullName.Focus();
		}

		void FullNameCompleted(object sender, EventArgs e)
		{
			entryPhoneNumber.Focus();
		}

		void PhoneNumberCompleted(object sender, EventArgs e)
		{
			entryPhoneNumber.Unfocus();
			CreateAccountClicked(null, EventArgs.Empty);
		}

		void UserNameInfoCompleted(object sender, EventArgs e)
		{
			entryFullNameInfo.Focus();
		}

		void FullNameInfoCompleted(object sender, EventArgs e)
		{
			entryPhoneNumberInfo.Focus();
		}

		void PhoneNumberInfoCompleted(object sender, EventArgs e)
		{
			entryPhoneNumberInfo.Unfocus();
			ContinueClicked(null, EventArgs.Empty);
		}

		#endregion Entry Handlers

		#region Button Handlers

		void OnLogoutClicked()
		{
			App.Locator.Workspace.Logout();
		}

		private async void LoginClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				await WithProgress(activityIndicator, async () => {
					try {
						Profile.KeyData kd;

						if (!String.IsNullOrWhiteSpace(entrySecretKey.Text)) {
							kd = Profile.GenerateKeyPairFromSeed(entrySecretKey.Text);
						} else {
							//Generate private key
							kd = Profile.GenerateKeyPairFromMnemonic(entryMnemonic.Text);
						}

						App.Locator.Profile.KeyPair = kd.KeyPair;

                        var result = await App.Locator.FundServiceClient.GetUser(kd.KeyPair.Address,null);

						App.Locator.Profile.SetCredentials(result?.UserDetails?.PaketUser,
															   result?.UserDetails?.FullName,
															   result?.UserDetails?.PhoneNumber,
															   kd.KeyPair.SecretSeed,
															   kd.MnemonicString);
						
						if (result != null) {
							CheckActivation();
						} else {
							Title = "Add Info";
							ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
							await ViewHelper.ToggleViews(layoutProvideInfo, layoutLogin);
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
						App.Locator.Profile.KeyPair = null;
						ShowMessage(ex is OutOfMemoryException ? "Secret key is invalid" : ex.Message);
					}
				});
			}
		}

		private async void CreateAccountClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				await WithProgress(activityIndicator, async () => {
					try {
						//Create new private key
						var kd = Profile.GenerateKeyPairFromMnemonic();
						App.Locator.Profile.KeyPair = kd.KeyPair;

						var result = await App.Locator.FundServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
																				  ViewModel.PhoneNumber, kd.KeyPair.Address);
						if (result != null) {
							App.Locator.Profile.SetCredentials(ViewModel.UserName,
															   ViewModel.FullName,
															   ViewModel.PhoneNumber,
							                                   kd.KeyPair.SecretSeed,
															   kd.MnemonicString);

                            //don't remove this line!
                             var tempResult = await App.Locator.FundServiceClient.UserInfos("Full Name 1", "1231231", "Address 1");

							var createResult = await App.Locator.FundServiceClient.CreateStellarAccount(ViewModel.PaymentCurrency.Value);
							if (createResult != null) {

                                var updateResult = await App.Locator.FundServiceClient.UserInfos(ViewModel.FullName, ViewModel.PhoneNumber, "");

								Title = "Activate Account";
								entryMnemonicPrompt.Text = kd.MnemonicString;
								entryAddress.Text = createResult.PaymentAddress;
								ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
								await ViewHelper.ToggleViews(layoutFundPrompt, layoutRegistration);
							} else {
								ShowMessage("Error creating Stellar account");
								App.Locator.Profile.KeyPair = null;
							}
						} else {
							ShowMessage("Error registering user");
							App.Locator.Profile.KeyPair = null;
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
						App.Locator.Profile.KeyPair = null;
						ShowMessage(ex.Message);
					}
				});
			}
		}

		private async void ContinueClicked(object sender, System.EventArgs e)
		{
			if (IsValid()) {
				await WithProgress(activityIndicator, async () => {
					try {
						//Retrievee private key
						var kd = App.Locator.Profile.TryGetKeyData();

						var result = await App.Locator.FundServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
																				  ViewModel.PhoneNumber, kd.KeyPair.Address);
						if (result != null) {
                            var updateResult = await App.Locator.FundServiceClient.UserInfos(ViewModel.FullName, ViewModel.PhoneNumber, "");

							App.Locator.Profile.SetCredentials(ViewModel.UserName,
															   ViewModel.FullName,
															   ViewModel.PhoneNumber,
															   kd.KeyPair.SecretSeed,
															   kd.MnemonicString);
							CheckActivation();
						} else {
							ShowMessage("Error adding info");
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
						ShowMessage(ex.Message);
					}
				});
			}
		}

		private async void AlreadyRegisteredClicked(object sender, EventArgs e)
		{
			Title = "Restore Private Key";
			await ViewHelper.ToggleViews(layoutLogin, layoutRegistration);
			ViewModel.Reset();
		}

		private async void GoToRegistrationClicked(object sender, EventArgs e)
		{
			Title = "Create New Private Key";
			await ViewHelper.ToggleViews(layoutRegistration, layoutLogin);
			ViewModel.Reset();
		}

		void CheckActivationClicked(object sender, System.EventArgs e)
		{
			CheckActivation();
		}

		#endregion Button Handlers

		public async void CheckActivation()
		{
			await WithProgress(activityIndicator, async () => {
				var created = await StellarHelper.CheckAccountCreated(App.Locator.Profile.KeyPair);
				if (created) {
					var trusted = await StellarHelper.CheckTokenTrusted();
					if (trusted) {
						App.Locator.Profile.Activated = true;
						Application.Current.MainPage = new MainPage();
					} else {
						var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair);
						if (added) {
							App.Locator.Profile.Activated = true;
							Application.Current.MainPage = new MainPage();
						} else {
							ShowMessage("Error adding trust token");
						}
					}
				} else {
					ShowMessage("Stellar account isn't created yet");
				}
			});
		}

		protected override bool IsValid()
		{
			if (layoutRegistration.IsVisible) {
				if (!ValidationHelper.ValidateTextField(entryUserName.Text)) {
					//Workspace.OnValidationError(ValidationError.Password);
					entryUserName.Focus();
					return false;
				}
				if (!ValidationHelper.ValidateTextField(entryFullName.Text)) {
					//Workspace.OnValidationError(ValidationError.PasswordConfirmation);
					entryFullName.Focus();
					return false;
				}
				if (!ValidationHelper.ValidateTextField(entryPhoneNumber.Text)) {
					//Workspace.OnValidationError(ValidationError.Email);
					entryPhoneNumber.Focus();
					return false;
				}

				if (pickerCurrency.SelectedItem == null) {
					ShowMessage("Please select payment currency");
					return false;
				}
			} else if (layoutLogin.IsVisible) {
				if (!String.IsNullOrWhiteSpace(entryMnemonic.Text) && !String.IsNullOrWhiteSpace(entrySecretKey.Text)) {
					ShowMessage("Please fill only one field");
					return false;
				}

				if (!ValidationHelper.ValidateTextField(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text)) {
					//Workspace.OnValidationError(ValidationError.Login);
					entryMnemonic.Focus();
					return false;
				}
			} else if (layoutProvideInfo.IsVisible) {
				if (!ValidationHelper.ValidateTextField(entryUserNameInfo.Text)) {
					//Workspace.OnValidationError(ValidationError.Password);
					entryUserNameInfo.Focus();
					return false;
				}
				if (!ValidationHelper.ValidateTextField(entryFullNameInfo.Text)) {
					//Workspace.OnValidationError(ValidationError.PasswordConfirmation);
					entryFullNameInfo.Focus();
					return false;
				}
				if (!ValidationHelper.ValidateTextField(entryPhoneNumberInfo.Text)) {
					//Workspace.OnValidationError(ValidationError.Email);
					entryPhoneNumberInfo.Focus();
					return false;
				}
			}

			return true;
		}

		protected override void ToggleLayout(bool enabled)
		{
			if (layoutRegistration.IsVisible) {
                entryUserName.IsEnabled = enabled;
                entryFullName.IsEnabled = enabled;
                entryPhoneNumber.IsEnabled = enabled;
				btnCreateAccount.IsEnabled = enabled;
				btnAlreadyRegistered.IsEnabled = enabled;
			} else if (layoutLogin.IsVisible) {
                entryMnemonic.IsEnabled = enabled;
				btnLogin.IsEnabled = enabled;
				btnGotoReg.IsEnabled = enabled;
			} else if (layoutFundPrompt.IsVisible) {
				btnCheck.IsEnabled = enabled;
			}
		}
	}
}
