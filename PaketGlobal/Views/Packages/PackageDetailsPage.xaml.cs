using System;
using System.Collections.Generic;
using PaketGlobal.ClientService;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class PackageDetailsPage : BasePage
	{
		public PackageDetailsPage(Package package)
		{
			InitializeComponent();

			barcodeImage.BarcodeOptions.Width = 320;
			barcodeImage.BarcodeOptions.Height = 320;
			barcodeImage.BarcodeOptions.Margin = 10;

			Title = "Package Details";

			BindingContext = package;
		}

		void EventItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				eventsList.SelectedItem = null;
			}
		}
	}
}
