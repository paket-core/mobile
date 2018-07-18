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

		void PubkeyCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Focus();
		}

		void BULsCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Unfocus();
		}

		private async void SendClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				Unfocus();

				App.ShowLoading(true);

                var recipientPubkey = entryRecepient.Text;                  //get recipient pubkey if user entered callsign                 if (recipientPubkey.Length != 56)                 {                     var recipientResult = await App.Locator.FundServiceClient.GetUser(null, recipientPubkey);                     if (recipientResult == null)                     {                         ShowMessage("Recipient not found");                          App.ShowLoading(false);                          return;                     }                     else                     {                         recipientPubkey = recipientResult.UserDetails.Pubkey;                     }                 }

                var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, recipientPubkey, long.Parse(entryAmount.Text));
				if (trans != null) {
					var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
					var result = await App.Locator.ServiceClient.SubmitTransaction(signed);
					if (result != null) {
						await ViewModel.Load();
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

		async void BuyBULClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.BUL)) {
				App.ShowLoading(true);

				var currency = (PaymentCurrency)pickerBULCurrency.SelectedItem;
                var amount = long.Parse(entryAmountForBUL.Text) * 100;
				var result = await App.Locator.FundServiceClient.PurchaseBULs(amount, currency);

				App.ShowLoading(false);

				if (result != null) {
					labelAmountForBUL.Text = String.Format("Please send your {0} to this address to purchase your BULs", currency);
					entryBULAddress.Text = result.PaymentAddress;
					await ViewHelper.ToggleViews(stackBULResult, stackBULPurchase);
				} else {
					ShowMessage("Error purchasing BULs");
				}
			}
		}

		async void BuyXLMClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.XLM)) {
				App.ShowLoading(true);

				var currency = (PaymentCurrency)pickerXLMCurrency.SelectedItem;
                var amount = long.Parse(entryAmountForXLM.Text) * 100;
				var result = await App.Locator.FundServiceClient.PurchaseXLMs(amount, currency);

				App.ShowLoading(false);

				if (result != null) {
					labelAmountForXLM.Text = String.Format("Please send your {0} to this address to purchase your XLMs", currency);
					entryXLMAddress.Text = result.PaymentAddress;
					await ViewHelper.ToggleViews(stackXLMResult, stackXLMPurchase);
				} else {
					ShowMessage("Error purchasing XLMs");
				}
			}
		}

		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateTextField(entryRecepient.Text)) {
				//Workspace.OnValidationError(ValidationError.Password);
				entryRecepient.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateNumber(entryAmount.Text)) {
				//Workspace.OnValidationError(ValidationError.PasswordConfirmation);
				entryAmount.Focus();
				return false;
			}

			return true;
		}

		private bool IsValid(SpendCurrency spendCurrency)
		{
			if (spendCurrency == SpendCurrency.BUL) {
				if (pickerBULCurrency.SelectedItem == null) {
					ShowMessage("Please select payment currency");
					return false;
				}
				if (!ValidationHelper.ValidateNumber(entryAmountForBUL.Text)) {
					entryAmountForBUL.Focus();
					return false;
				}
			} else {
				if (pickerXLMCurrency.SelectedItem == null) {
					ShowMessage("Please select payment currency");
					return false;
				}
				if (!ValidationHelper.ValidateNumber(entryAmountForXLM.Text)) {
					entryAmountForXLM.Focus();
					return false;
				}
			}

			return true;
		}
	}
}
