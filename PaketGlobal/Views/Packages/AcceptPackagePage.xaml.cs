using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class AcceptPackagePage : BasePage
	{
		bool cleaned;
		bool scanned;

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

					//TODO check scanned barcode, do stuff

					if (true) {
						scanned = true;
						await ViewHelper.ToggleViews(layoutAccept, layoutBarcode);
					} else {
						StartScanning();
					}

					App.Locator.NotificationService.ShowMessage(String.Format("Scanned Barcode: {0}", result.Text));
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

		private void CleanUp()
		{
			StopScanning();
			cleaned = true;
		}
	}
}
