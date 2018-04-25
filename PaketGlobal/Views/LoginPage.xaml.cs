using System;
using System.Collections.Generic;
using System.Text;
using NBitcoin;
using NBitcoin.DataEncoders;
using Stellar;
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
		}

		protected override bool OnBackButtonPressed()
		{
			if (layoutRegistration.IsVisible) {
				AlreadyRegisteredClicked(null, EventArgs.Empty);
			}
			//MessagingCenter.Unsubscribe<object> (this, MessagingCenterConstants.OnRegistrationConfirmedMessage);
			return true;
		}

		#region Entry Handlers

		void LoginUserNameCompleted(object sender, EventArgs e)
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

		private async void LoginClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				await WithProgress(activityIndicator, async () => {
					try {
						//Generate private key
						var kd = Profile.GenerateKeyPair(entryMnemonic.Text);
						App.Locator.Profile.KeyPair = kd.KeyPair;

						var result = await App.Locator.ServiceClient.RecoverUser(kd.KeyPair.Address);
						if (result != null) {
							App.Locator.Profile.SetCredentials(result.UserDetails.PaketUser, result.UserDetails.FullName,
															   result.UserDetails.PhoneNumber, result.UserDetails.Pubkey, kd.MnemonicString);

							MessagingCenter.Unsubscribe<object>(this, MessagingCenterConstants.OnRegistrationConfirmedMessage);

							Application.Current.MainPage = new MainPage();
						} else {
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

		private async void CreateAccountClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				await WithProgress(activityIndicator, async () => {
					try {
						//Create new private key
						var kd = Profile.GenerateKeyPair();
						App.Locator.Profile.KeyPair = kd.KeyPair;

						var result = await App.Locator.ServiceClient.RegisterUser(ViewModel.UserName, ViewModel.FullName,
																				  ViewModel.PhoneNumber, kd.KeyPair.Address);
						if (result != null) {
							App.Locator.Profile.SetCredentials(result.UserDetails.PaketUser, result.UserDetails.FullName,
															   result.UserDetails.PhoneNumber, result.UserDetails.Pubkey, kd.MnemonicString);

							Application.Current.MainPage = new MainPage();
						} else {
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

		#endregion Button Handlers

		protected override bool IsValid()
		{
			//return true;

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
				if (!ValidationHelper.ValidateTextField(entryMnemonic.Text)) {
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
			} else {
				entryMnemonic.InputEnabled = enabled;
				btnLogin.IsEnabled = enabled;
				btnGotoReg.IsEnabled = enabled;
			}
		}
	}
}
