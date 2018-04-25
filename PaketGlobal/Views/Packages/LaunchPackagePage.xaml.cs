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
				System.Threading.Thread.Sleep(1000);

				await App.Locator.Packages.Load();
				App.Locator.NavigationService.GoBack();
			}

			App.ShowLoading(false, false);
		}

		void DeadlineTapped(object sender, System.EventArgs e)
		{
			var dpc = new DatePromptConfig();
			dpc.OkText = "OK";
			dpc.CancelText = "Cancel";
			dpc.IsCancellable = true;
			dpc.MinimumDate = DateTime.Today;
			dpc.Title = "Please select a Deadline Date";
			dpc.OnAction = dateResult =>
			{
				if (dateResult.Ok) {
					var deadlineSecs = DateTimeHelper.ToUnixTime(dateResult.SelectedDate);

					var ptc = new TimePromptConfig();
					ptc.OkText = "OK";
					ptc.CancelText = "Cancel";
					ptc.IsCancellable = true;
					ptc.SelectedTime = new TimeSpan(14, 0, 0);
					ptc.Title = "Please select a Deadline Time";
					ptc.OnAction = timeResult => {
						if (timeResult.Ok) {
							ViewModel.Deadline = deadlineSecs + (long)timeResult.SelectedTime.TotalSeconds;
							entryDeadline.Text = ViewModel.DeadlineString;
						}
					};

					UserDialogs.Instance.TimePrompt(ptc);
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
