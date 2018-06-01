using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class AcceptPackagePage : BasePage
	{
		bool cleaned;
		bool scanned;
		BarcodePackageData data;

		private Package ViewModel {
			get {
				return BindingContext as Package;
			}
		}

		public AcceptPackagePage()
		{
			InitializeComponent();

			Title = "Accept Package";

			SetupUserInterface();

			ConfigureScanner();
		}

		protected override void SetupUserInterface()
		{
			base.SetupUserInterface();

			RelativeLayout.SetXConstraint(labelBarcode, Constraint.RelativeToParent(p => (p.Width - labelBarcode.Measure(p.Width, p.Height).Request.Width) / 2f));
			RelativeLayout.SetYConstraint(labelBarcode, Constraint.RelativeToParent(p => (p.Height - labelBarcode.Measure(p.Width, p.Height).Request.Height) / 2f));
		}

		protected override bool OnBackButtonPressed()
		{
			CleanUp();
			return base.OnBackButtonPressed();
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (!scanned) StartScanning();
		}

		protected override void OnDisappearing()
		{
			if (!scanned) StopScanning();
			base.OnDisappearing();
		}

		private void ConfigureScanner()
		{
			barcodeScaner.Options.UseFrontCameraIfAvailable = false;
			barcodeScaner.OnScanResult += (result) => {
				Device.BeginInvokeOnMainThread(async () => {
					try {
						data = JsonConvert.DeserializeObject<BarcodePackageData>(result.Text);
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
					}

					if (data != null && data.EscrowAddress != null && data.PaymentTransaction != null) {
						App.ShowLoading(true);

						StopScanning();
						scanned = true;

						var package = await App.Locator.ServiceClient.Package(data.EscrowAddress);
						if (package != null && package.Package != null) {
							var myPubkey = App.Locator.Profile.Pubkey;
							if (myPubkey == package.Package.RecipientPubkey) {
								//you are a recepient
								package.Package.MyRole = PaketRole.Recipient;
								Title = "Accept as a Recipient";
							} else {/*if (myPubkey == package.CourierPubkey) {*/
								//you are a courier
								package.Package.MyRole = PaketRole.Courier;
								Title = "Accept as a Courier";
							} /*else {
								//you are nothing
								ShowError("You are not participating in this delivery");
								StartScanning();
								return;
							}*/

							BindingContext = package.Package;
							await ViewHelper.ToggleViews(layoutAccept, layoutBarcode);
						} else {
							ShowError("Invalid package identifier");
							StartScanning();
						}

						App.ShowLoading(false);
					} else {
						ShowError("Invalid barcode");
						StartScanning();
					}
				});
			};
		}

		async void AcceptClicked(object sender, System.EventArgs e)
		{
			var myPubkey = App.Locator.Profile.Pubkey;
			if (myPubkey == ViewModel.RecipientPubkey) {
				//I'm a recipient
				App.ShowLoading(true);

				var result = await StellarHelper.AcceptPackageAsRecipient(data.EscrowAddress, data.PaymentTransaction);
				if (result == StellarOperationResult.Success) {
					await System.Threading.Tasks.Task.Delay(2000);
					await App.Locator.Packages.Load();
					ShowError("Package accepted successfully");
					App.Locator.NavigationService.GoBack();
				} else {
					ShowError(result);
				}

				App.ShowLoading(false);
			} else {
				//I'm a courier
				App.ShowLoading(true);

				var result = await StellarHelper.AcceptPackageAsCourier(data.EscrowAddress, ViewModel.Collateral, data.PaymentTransaction);
				if (result == StellarOperationResult.Success) {
					await System.Threading.Tasks.Task.Delay(2000);
					await App.Locator.Packages.Load();
					ShowError("Package accepted successfully");
					App.Locator.NavigationService.GoBack();
				} else {
					ShowError(result);
				}

				App.ShowLoading(false);
			}
		}

		public void StartScanning()
		{
			if (!cleaned && barcodeScaner != null) {
				barcodeScaner.IsAnalyzing = true;
				barcodeScaner.IsScanning = true;
			}
		}

		public void StopScanning()
		{
			if (!cleaned && barcodeScaner != null) {
				barcodeScaner.IsAnalyzing = false;
				barcodeScaner.IsScanning = false;
			}
		}

		protected override void CleanUp()
		{
			StopScanning();
			cleaned = true;
		}
	}
}
