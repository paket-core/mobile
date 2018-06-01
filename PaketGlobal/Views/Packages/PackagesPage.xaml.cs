using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class PackagesPage : BasePage
	{
		private PackagesModel ViewModel {
			get {
				return BindingContext as PackagesModel;
			}
		}

		ICommand RefreshListCommand {
			get {
				return new Command(async () => {
					await LoadPackages();
					packagesList.IsRefreshing = false;
				});
			}
		}

		public PackagesPage()
		{
			Title = "Packages";

			ToolbarItems.Add(new ToolbarItem("Launch Package", "ic_add_circle_white_24dp.png", LaunchPackageClicked));
			ToolbarItems.Add(new ToolbarItem("Accept Package", "ic_check_circle_white_24dp.png", AcceptPackageClicked));

			InitializeComponent();

			BindingContext = App.Locator.Packages;
			packagesList.RefreshCommand = RefreshListCommand;
		}

		void LaunchPackageClicked()
		{
			App.Locator.NavigationService.NavigateTo(Locator.LaunchPackagePage, new Package() {
				Deadline = DateTimeHelper.ToUnixTime(DateTime.Now.AddDays(1)),
				RecipientPubkey = "GDEO6AUQ3OIIHL2R2IBAWXWJR6NQ5YSCSLJOKHHJUQWRNDFIWO67VCLW",//TODO remove this
				CourierPubkey = "GD6UGA2SMQWHCCAUS2WIH4IYBCYKVXCLAZBMMRWSCQZJOF7QNZKBFWKA",//TODO remove this
				Payment = 20,//TODO remove this
				Collateral = 30//TODO remove this
			});
		}

		void AcceptPackageClicked()
		{
			App.Locator.NavigationService.NavigateTo(Locator.AcceptPackagePage);
		}

		protected async override void OnAppearing()
		{
			var fl = firstLoad;

			base.OnAppearing();

			if (fl) {
				await LoadPackages();
			}
		}

		private async System.Threading.Tasks.Task LoadPackages()
 		{
			placholderLabel.IsVisible = false;
			layoutPlacholderLabel.IsVisible = false;

			activityIndicator.IsRunning = true;

			await ViewModel.Load();

			if (ViewModel.PackagesList == null) {
				placholderLabel.IsVisible = true;
				layoutPlacholderLabel.IsVisible = true;
			} else if (ViewModel.PackagesList.Count == 0) {
				placholderLabel.IsVisible = true;
				layoutPlacholderLabel.IsVisible = true;
			}

			await layoutActivity.FadeTo(0);
			await packagesList.FadeTo(1);

			layoutActivity.IsVisible = false;
		}

		async void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				App.ShowLoading(true);

				var pkgData = (Package)e.SelectedItem;
				var packageData = await App.Locator.ServiceClient.Package(pkgData.PaketId);
				if (packageData != null) {
					var balanceData = await App.Locator.ServiceClient.Balance(pkgData.PaketId);

					packageData.Package.DeliveryStatus = balanceData != null && balanceData.BalanceBUL == 0 ?
						DeliveryStatus.Delivered :
						(balanceData == null ? DeliveryStatus.Closed : (DateTime.Now > packageData.Package.DeadlineDT ? DeliveryStatus.DeadlineExpired : DeliveryStatus.InTransit));

					var myPubkey = App.Locator.Profile.Pubkey;
					packageData.Package.MyRole = myPubkey == packageData.Package.LauncherPubkey ? PaketRole.Launcher :
						(myPubkey == packageData.Package.RecipientPubkey ? PaketRole.Recipient : PaketRole.Courier);
					
					App.Locator.NavigationService.NavigateTo(Locator.PackageDetailsPage, packageData.Package);
				} else {
					ShowError("Error retrieving package details");
				}
				packagesList.SelectedItem = null;

				App.ShowLoading(false);
			}
		}
	}
}
