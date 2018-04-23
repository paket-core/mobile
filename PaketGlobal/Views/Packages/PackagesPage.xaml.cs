using System;
using System.Collections.Generic;
using PaketGlobal.ClientService;
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

		public PackagesPage()
		{
			Title = "Packages";

			ToolbarItems.Add(new ToolbarItem("Launch Package", "ic_add_circle_white_24dp.png", LaunchPackageClicked));

			InitializeComponent();

			BindingContext = App.Locator.Packages;
		}

		void LaunchPackageClicked()
		{
			App.Locator.NavigationService.NavigateTo(Locator.LaunchPackagePage, new Package());
		}

		protected async override void OnAppearing()
		{
			if (firstLoad) {
				await LoadPackages();
			}
			base.OnAppearing();
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

		void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
		{
			if (e.SelectedItem != null) {
				App.Locator.NavigationService.NavigateTo(Locator.PackageDetailsPage, ((ClientService.Package)e.SelectedItem));
				packagesList.SelectedItem = null;
			}
		}
	}
}
