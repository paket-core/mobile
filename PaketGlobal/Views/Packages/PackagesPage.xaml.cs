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
			await ViewModel.Load();

			activityIndicator.IsRunning = false;
			layoutActivity.IsVisible = false;

			placholderLabel.IsVisible = ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0;

			await packagesList.FadeTo(0);
			await packagesList.FadeTo(1);
		}

		async void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				App.ShowLoading(true);

				var pkgData = (Package)e.SelectedItem;
				var package = await PackageHelper.GetPackageDetails(pkgData.PaketId);
				if (package != null) {
					App.Locator.NavigationService.NavigateTo(Locator.PackageDetailsPage, package);
				} else {
					ShowMessage("Error retrieving package details");
				}

				packagesList.SelectedItem = null;

				App.ShowLoading(false);
			}
		}
	}
}
