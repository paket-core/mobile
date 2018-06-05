using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using stellar_dotnetcore_sdk;
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

			ToolbarItems.Add(new ToolbarItem("Launch", null, OnSaveClicked));
		}

		async void OnSaveClicked()
		{
			Unfocus();

			App.ShowLoading(true);

			var vm = ViewModel;
			var escrowKP = KeyPair.Random();
			var result = await StellarHelper.LaunchPackage(escrowKP, vm.RecipientPubkey, vm.Deadline, vm.CourierPubkey, vm.Payment, vm.Collateral);
			if (result == StellarOperationResult.Success) {
				await System.Threading.Tasks.Task.Delay(2000);
				await App.Locator.Packages.Load();

				var package = await PackageHelper.GetPackageDetails(escrowKP.Address);

				ShowMessage("Package created successfully");
				App.Locator.NavigationService.GoBack();

				if (package != null) {
					App.Locator.NavigationService.NavigateTo(Locator.PackageDetailsPage, package);
				} else {
					ShowMessage("Error retrieving package details");
				}
			} else {
				ShowError(result);
			}

			App.ShowLoading(false);
		}

		void DeadlineTapped(object sender, System.EventArgs e)
		{
			var dpc = new DatePromptConfig();
			dpc.OkText = "OK";
			dpc.CancelText = "Cancel";
			dpc.IsCancellable = true;
			dpc.MinimumDate = DateTime.Today.AddDays(1);
			dpc.SelectedDate = ViewModel.DeadlineDT.Date;
			dpc.Title = "Please select a Deadline Date";
			dpc.OnAction = dateResult =>
			{
				if (dateResult.Ok) {
					var date = dateResult.SelectedDate.Date;
					date = date.AddSeconds(86399);//23:59.59 of selected day
					ViewModel.Deadline = DateTimeHelper.ToUnixTime(date.ToUniversalTime());
					entryDeadline.Text = ViewModel.DeadlineString;
				}
			};

			UserDialogs.Instance.DatePrompt(dpc);
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
				entryCollateral.Unfocus();
			}
		}
	}
}
