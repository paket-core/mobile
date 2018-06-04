using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class PackageDetailsPage : BasePage
	{
		private Package ViewModel { get { return BindingContext as Package; } }

		public PackageDetailsPage(Package package)
		{
			InitializeComponent();

			barcodeImage.BarcodeOptions.Width = 320;
			barcodeImage.BarcodeOptions.Height = 320;
			barcodeImage.BarcodeOptions.Margin = 10;

			Title = "Package Details";

			BindingContext = package;

			var transData = App.Locator.Profile.GetTransaction(package.PaketId);

			var data = new BarcodePackageData {
				EscrowAddress = package.PaketId,
				PaymentTransaction = transData?.PaymentTransaction
			};

			barcodeImage.BarcodeValue = JsonConvert.SerializeObject(data);
		}

		void EventItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				eventsList.SelectedItem = null;
			}
		}

		async void RefundClicked(object sender, System.EventArgs e)
		{
			App.ShowLoading(true);

			var transData = App.Locator.Profile.GetTransaction(ViewModel.PaketId);
			if (transData != null) {
				var result = await StellarHelper.RefundEscrow(transData.RefundTransaction, transData.MergeTransaction);
				if (result) {
					stackRefund.IsVisible = false;
					lblStatus.Text = "Closed";
					ShowMessage("Refunding successfull");
				} else {
					ShowMessage("Error during refunding");
				}
			} else {
				ShowMessage("Transcations data is missing");
			}

			App.ShowLoading(false);
		}

		async void ReclaimClicked(object sender, System.EventArgs e)
		{
			App.ShowLoading(true);

			var transData = App.Locator.Profile.GetTransaction(ViewModel.PaketId);
			if (transData != null) {
				var result = await StellarHelper.ReclaimEscrow(transData.MergeTransaction);
				if (result) {
					stackReclaim.IsVisible = false;
					lblStatus.Text = "Closed";
					ShowMessage("Reclaiming successfull");
				} else {
					ShowMessage("Error during reclaiming");
				}
			} else {
				ShowMessage("Transcations data is missing");
			}

			App.ShowLoading(false);
		}
	}
}
