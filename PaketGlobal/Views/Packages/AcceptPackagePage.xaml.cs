using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class AcceptPackagePage : BasePage
	{
		bool cleaned;
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

            App.Locator.DeviceService.setStausBarLight();
		}

        private void OnBack(object sender, System.EventArgs e)
        {
            CleanUp();

            Navigation.PopToRootAsync();
        }


		protected override void OnAppearing()
		{
			base.OnAppearing();

            if (barcodeScaner.IsScanning==false) 
                StartScanning();

            App.Locator.DeviceService.setStausBarLight();
		}

		protected override void OnDisappearing()
		{
            if (barcodeScaner.IsScanning==true) 
                StopScanning();
            
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

						var package = await App.Locator.RouteServiceClient.Package(data.EscrowAddress);
                        if (package != null && package.Package != null && package.Package.PaymentTransaction!=null) {
							var myPubkey = App.Locator.Profile.Pubkey;
							if (myPubkey == package.Package.RecipientPubkey) {
                                //you are a recepient //Title = "Accept as a Recipient";
								package.Package.MyRole = PaketRole.Recipient;
							} 
                            else {
                                //you are a courier //Title = "Accept as a Courier";
								package.Package.MyRole = PaketRole.Courier;
							}

							BindingContext = package.Package;

                            var packagePage = new NewPackageDetailPage(package.Package, true, data);
                            await Navigation.PushAsync(packagePage);
						} 
                        else {
                            ShowMessage(AppResources.InvalidPackageId);
							StartScanning();
						}

						App.ShowLoading(false);
					} 
                    else {
                        ShowMessage(AppResources.InvalidBarcode);
					}
				});
			};
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
