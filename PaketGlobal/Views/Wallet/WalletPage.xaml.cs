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
				await LoadPackages();
			}
			base.OnAppearing();
		}

		private async System.Threading.Tasks.Task LoadPackages()
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
			Unfocus();

			App.ShowLoading(true, false);

			var result = await App.Locator.ServiceClient.SendBuls(entryRecepient.Text, long.Parse(entryAmount.Text));
			if (result != null) {
				ShowError("Sent successfully");
			} else {
				ShowError("Error occured");
			}

			App.ShowLoading(false, false);
		}

		void PubkeyCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Focus();
		}

		void BULsCompleted(object sender, System.EventArgs e)
		{
			entryAmount.Unfocus();
		}
	}
}
