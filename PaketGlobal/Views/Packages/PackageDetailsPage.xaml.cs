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
