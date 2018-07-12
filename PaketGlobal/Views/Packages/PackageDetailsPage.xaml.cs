using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class PackageDetailsPage : BasePage
	{
		private Package ViewModel { get { return BindingContext as Package; } }
       
        private Command BarcodeTapCommand;

		public PackageDetailsPage(Package package)
		{
			InitializeComponent();

            BindingContext = package;

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
            BackButton.TranslationX = -25;
#endif

            var data = new BarcodePackageData {
              EscrowAddress = package.PaketId
            };
            BarcodeImage.BarcodeOptions.Width = 300;
            BarcodeImage.BarcodeOptions.Height = 300;
            BarcodeImage.BarcodeOptions.Margin = 1;
            BarcodeImage.BarcodeValue = JsonConvert.SerializeObject(data);


            BarcodeTapCommand = new Command(() =>
            {
                BarcodeImage.IsVisible = !BarcodeImage.IsVisible;
            });

            XamEffects.Commands.SetTap(BarcodeView, BarcodeTapCommand);
		}

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

		private async void RefundClicked(object sender, System.EventArgs e)
		{
			App.ShowLoading(true);

			var transData = App.Locator.Profile.GetTransaction(ViewModel.PaketId);
			if (transData != null) {
				var result = await StellarHelper.RefundEscrow(transData.RefundTransaction, transData.MergeTransaction);
				if (result) {
				//	stackRefund.IsVisible = false;
				//	lblStatus.Text = "Closed";
					ShowMessage("Refunding successfull");
				} else {
					ShowMessage("Error during refunding");
				}
			} else {
				ShowMessage("Transcations data is missing");
			}

			App.ShowLoading(false);
		}

		private async void ReclaimClicked(object sender, System.EventArgs e)
		{
			App.ShowLoading(true);

			var transData = App.Locator.Profile.GetTransaction(ViewModel.PaketId);
			if (transData != null) {
				var result = await StellarHelper.ReclaimEscrow(transData.MergeTransaction);
				if (result) {
				//	stackReclaim.IsVisible = false;
				//	lblStatus.Text = "Closed";
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
