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
				Title = "Activate Account";
				ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
				layoutLogin.IsVisible = false;
				layoutRegistration.IsVisible = false;
				layoutFundPrompt.IsVisible = true;
				entryMnemonicPrompt.Text = App.Locator.Profile.Mnemonic;
				CheckActivation();
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

						var result = await App.Locator.ServiceClient.RecoverUser(kd.KeyPair.Address);
						if (result != null) {
							App.Locator.Profile.SetCredentials(result?.UserDetails?.PaketUser,
															   result?.UserDetails?.FullName,
															   result?.UserDetails?.PhoneNumber,
															   kd.KeyPair.SecretSeed,
															   kd.MnemonicString);

							CheckActivation();
						} else {
							App.Locator.Profile.KeyPair = null;
							Application.Current.MainPage = new MainPage();
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
						App.Locator.Profile.KeyPair = null;
						ShowError(ex is OutOfMemoryException ? "Secret key is invalid" : ex.Message);
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

						var result = await App.Locator.ServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
																				  ViewModel.PhoneNumber, kd.KeyPair.Address);
						if (result != null) {
							App.Locator.Profile.SetCredentials(ViewModel.UserName,
															   ViewModel.FullName,
															   ViewModel.PhoneNumber,
							                                   kd.KeyPair.SecretSeed,
															   kd.MnemonicString);
							Title = "Activate Account";
							entryMnemonicPrompt.Text = kd.MnemonicString;
							ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
							await ViewHelper.ToggleViews(layoutFundPrompt, layoutRegistration);
						} else {
							ShowError("Error registering user");
							App.Locator.Profile.KeyPair = null;
						}
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
						App.Locator.Profile.KeyPair = null;
						ShowError(ex.Message);
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
						var added = await StellarHelper.AddTrustToken(App.Locator.Profile.KeyPair, "SC2PO5YMP7VISFX75OH2DWETTEZ4HVZOECMDXOZIP3NBU3OFISSQXAEP");
						if (added) {
							App.Locator.Profile.Activated = true;
							Application.Current.MainPage = new MainPage();
						} else {
							ShowError("Error adding trust token");
						}
					}
				} else {
					ShowError("Account isn't created yet");
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
			} else {
				if (!String.IsNullOrWhiteSpace(entryMnemonic.Text) && !String.IsNullOrWhiteSpace(entrySecretKey.Text)) {
					ShowError("Please fill only one field");
					return false;
				}

				if (!ValidationHelper.ValidateTextField(entryMnemonic.Text) && !ValidationHelper.ValidateTextField(entrySecretKey.Text)) {
					//Workspace.OnValidationError(ValidationError.Login);
					entryMnemonic.Focus();
					return false;
				}
			}

			return true;
		}

		protected override void ToggleLayout(bool enabled)
		{
			if (layoutRegistration.IsVisible) {
				entryUserName.InputEnabled = enabled;
				entryFullName.InputEnabled = enabled;
				entryPhoneNumber.InputEnabled = enabled;
				btnCreateAccount.IsEnabled = enabled;
				btnAlreadyRegistered.IsEnabled = enabled;
			} else if (layoutLogin.IsVisible) {
				entryMnemonic.InputEnabled = enabled;
				btnLogin.IsEnabled = enabled;
				btnGotoReg.IsEnabled = enabled;
			} else if (layoutFundPrompt.IsVisible) {
				btnCheck.IsEnabled = enabled;
			}
		}
	}
}
