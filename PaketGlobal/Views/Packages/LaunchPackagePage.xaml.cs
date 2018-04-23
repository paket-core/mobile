using System;
using System.Collections.Generic;
using PaketGlobal.ClientService;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class LaunchPackagePage : BasePage
	{
		private Package ViewModel {
			get {
				return BindingContext as Package;
			}
		}

		public LaunchPackagePage(Package package)
		{
			InitializeComponent();

			BindingContext = package;

			Title = (ViewModel == null) ? "Launch New Package" : "Edit Package";

			ToolbarItems.Add(new ToolbarItem("Save", null, OnSaveClicked));
		}

		async void OnSaveClicked()
		{
			Unfocus();

			App.ShowLoading(true, false);

			var vm = ViewModel;
			var result = await App.Locator.ServiceClient.LaunchPackage(vm.RecipientPubkey, vm.Deadline, vm.CourierPubkey, vm.Payment, vm.Collateral);
			if (result != null) {
				System.Threading.Thread.Sleep(1000);

				await App.Locator.Packages.Load();
				App.Locator.NavigationService.GoBack();
			}

			App.ShowLoading(false, false);
		}

		void HandleCompleted(object sender, System.EventArgs e)
		{
			if (sender == entryRecepient) {
				entryCourier.Focus();
			} else if (sender == entryCourier) {
				entryDeadline.Focus();
			} else if (sender == entryDeadline) {
				entryPayment.Focus();
			} else if (sender == entryPayment) {
				entryCollateral.Focus();
			} else if (sender == entryCollateral) {
				OnSaveClicked();
			}
		}
	}
}
