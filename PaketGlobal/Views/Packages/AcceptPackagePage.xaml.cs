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

            #if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif

			ConfigureScanner();
		}

        private void OnBack(object sender, System.EventArgs e)
        {
            CleanUp();
            Navigation.PopModalAsync();
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

		void ScannerOverlayTapped(object sender, EventArgs e)
		{
			barcodeScaner?.AutoFocus();
		}

		private void ConfigureScanner()
		{
			overlayBarcode.BindingContext = overlayBarcode;

			barcodeScaner.Options.UseFrontCameraIfAvailable = false;

            barcodeScaner?.AutoFocus();

			barcodeScaner.OnScanResult += (result) => {
				Device.BeginInvokeOnMainThread(async () => {
					try {
						data = JsonConvert.DeserializeObject<BarcodePackageData>(result.Text);
					} catch (Exception ex) {
						System.Diagnostics.Debug.WriteLine(ex);
					}

					if (data != null && data.EscrowAddress != null) {
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
							} else {
								//you are a courier
								package.Package.MyRole = PaketRole.Courier;
								Title = "Accept as a Courier";
							}

							BindingContext = package.Package;
							await ViewHelper.ToggleViews(layoutAccept, layoutBarcode);
						} else {
							ShowMessage("Invalid package identifier");
							StartScanning();
						}

						App.ShowLoading(false);
					} else {
						ShowMessage("Invalid barcode");
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

				var result = await StellarHelper.AcceptPackageAsRecipient(data.EscrowAddress, ViewModel.PaymentTransaction);
				if (result == StellarOperationResult.Success) {
					await System.Threading.Tasks.Task.Delay(2000);
					await App.Locator.Packages.Load();
					ShowMessage("Package accepted successfully");
					App.Locator.NavigationService.GoBack();
				} else {
					ShowError(result);
				}

				App.ShowLoading(false);
			} else {
				//I'm a courier
				App.ShowLoading(true);

				var result = await StellarHelper.AcceptPackageAsCourier(data.EscrowAddress, ViewModel.Collateral, ViewModel.PaymentTransaction);
				if (result == StellarOperationResult.Success) {
					await System.Threading.Tasks.Task.Delay(2000);
					await App.Locator.Packages.Load();
					ShowMessage("Package accepted successfully");
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
