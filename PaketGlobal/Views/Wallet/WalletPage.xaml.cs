using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class WalletPage : BasePage
	{
		private WalletModel ViewModel {
			get {
				return BindingContext as WalletModel;
			}
		}

		public WalletPage()
		{
			InitializeComponent();

			Title = "Wallet";

			BindingContext = App.Locator.Wallet;
		}

		protected async override void OnAppearing()
		{
			if (firstLoad) {
				await LoadWallet();
			}
			base.OnAppearing();
		}

		private async System.Threading.Tasks.Task LoadWallet()
		{
			layoutActivity.IsVisible = true;
			activityIndicator.IsRunning = true;

			await ViewModel.Load();

			await layoutActivity.FadeTo(0);
			await contentWallet.FadeTo(1);

			layoutActivity.IsVisible = false;
		}

		private async void SendClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				Unfocus();

				App.ShowLoading(true);

				var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, entryRecepient.Text, long.Parse(entryAmount.Text));
				if (trans != null) {
					var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
					var result = await App.Locator.ServiceClient.SubmitTransaction(signed);
					if (result != null) {
						ShowMessage("Funds sent successfully");
					} else {
						ShowMessage("Error sending funds");
					}
				} else {
					ShowMessage("Error sending funds");
				}

				App.ShowLoading(false);
			}
		}

		void PubkeyCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Focus();
		}

		void BULsCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Unfocus();
		}

		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateTextField(entryRecepient.Text)) {
				//Workspace.OnValidationError(ValidationError.Password);
				entryRecepient.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateTextField(entryAmount.Text)) {
				//Workspace.OnValidationError(ValidationError.PasswordConfirmation);
				entryAmount.Focus();
				return false;
			}

			return true;
		}
	}
}
