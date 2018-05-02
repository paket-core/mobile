using System;
using System.Collections.Generic;
using Acr.UserDialogs;
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
				App.Locator.Profile.AddTransaction(result.EscrowAddress, result.PaymentTransaction);//save payment transaction data

				var trans = await App.Locator.ServiceClient.PrepareSendBuls(result.EscrowAddress, vm.Payment);
				if (trans != null) {
					var signed = App.Locator.Profile.SignData(trans.Transaction);
					var paymentResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
					if (paymentResult != null) {
						await System.Threading.Tasks.Task.Delay(2000);
						await App.Locator.Packages.Load();
						ShowError("Package created successfully");
						App.Locator.NavigationService.GoBack();
					} else {
						ShowError("Error sending payment");
					}
				} else {
					ShowError("Error sending payment");
				}
			} else {
				ShowError("Error during package creation");
			}

			App.ShowLoading(false, false);
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
					ViewModel.Deadline = DateTimeHelper.ToUnixTime(date);
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
