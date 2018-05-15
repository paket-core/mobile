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
					StopScanning();

					try {
						data = JsonConvert.DeserializeObject<BarcodePackageData>(result.Text);
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
					}

					if (data != null && data.EscrowAddress != null && data.PaymentTransaction != null) {
						scanned = true;

						var package = await App.Locator.ServiceClient.Package(data.EscrowAddress);
						if (package != null && package.Package != null) {
							var myPubkey = App.Locator.Profile.Pubkey;
							if (myPubkey == package.Package.RecipientPubkey) {
								//you are a recepient
								Title = "Accept as a Recipient";
							} else {/*if (myPubkey == package.CourierPubkey) {*/
								//you are a courier
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
				App.ShowLoading(true, false);

				var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, data.PaymentTransaction);//sign the payment transaction
				var submitResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
				if (submitResult != null) {
					var result = await App.Locator.ServiceClient.AcceptPackage(data.EscrowAddress, data.PaymentTransaction);//accept the package
					if (result != null) {
						await System.Threading.Tasks.Task.Delay(2000);
						await App.Locator.Packages.Load();
						ShowError("Package accepted successfully");
						App.Locator.NavigationService.GoBack();
					} else {
						ShowError("Error accepting the package");
					}
				} else {
					ShowError("Error accepting the package");
				}

				App.ShowLoading(false, false);
			} else {
				//I'm a courier
				App.ShowLoading(true, false);

				var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, data.EscrowAddress, ViewModel.Collateral);
				if (trans != null) {
					var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
					var paymentResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
					if (paymentResult != null) {
						var acceptResult = await App.Locator.ServiceClient.AcceptPackage(data.EscrowAddress);
						if (acceptResult != null) {
							App.Locator.Profile.AddTransaction(data.EscrowAddress, data.PaymentTransaction);
							await System.Threading.Tasks.Task.Delay(2000);
							await App.Locator.Packages.Load();
							ShowError("Package accepted successfully");
							App.Locator.NavigationService.GoBack();
						} else {
							ShowError("Error accepting the package");
						}
					} else {
						ShowError("Error sending collateral");
					}
				} else {
					ShowError("Error sending collateral");
				}

				App.ShowLoading(false, false);
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
