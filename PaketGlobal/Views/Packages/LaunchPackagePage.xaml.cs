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

			if (IsValid()) {
				App.ShowLoading(true);

				var vm = ViewModel;
				
                var escrowKP = KeyPair.Random();

                var recipientPubkey = vm.RecipientPubkey;
                var courierPubkey = vm.CourierPubkey;

                //get recipient pubkey if user entered callsign
                if (recipientPubkey.Length != 56)
                {
                    var recipientResult = await App.Locator.FundServiceClient.GetUser(null, recipientPubkey);
                    if (recipientResult == null) {
                        App.ShowLoading(false);
                        ShowMessage("Recipient not found");
                        return;
                    }
                    else{
                        recipientPubkey = recipientResult.UserDetails.Pubkey;
                    }
                }

                //get courier pubkey if user entered callsign
                if (courierPubkey.Length != 56)
                {
                    var courierResult = await App.Locator.FundServiceClient.GetUser(null, courierPubkey);
                    if (courierResult == null)
                    {
                        App.ShowLoading(false);
                        ShowMessage("Courier not found");
                        return;
                    }
                    else
                    {
                        courierPubkey = courierResult.UserDetails.Pubkey;
                    }
                }

                var result = await StellarHelper.LaunchPackage(escrowKP, recipientPubkey, vm.Deadline, courierPubkey, vm.Payment, vm.Collateral);

       
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
		}

		void DeadlineTapped(object sender, System.EventArgs e)
		{
            entryDeadline.Unfocus();

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

		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateTextField(entryCourier.Text)) {
				entryCourier.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateTextField(entryRecepient.Text)) {
				entryRecepient.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateNumber(entryPayment.Text)) {
				entryPayment.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateNumber(entryCollateral.Text)) {
				entryCollateral.Focus();
				return false;
			}

			return true;
		}
	}
}
