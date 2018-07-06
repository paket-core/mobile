using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class ProfilePage : BasePage
	{
		private ProfileModel ViewModel {
			get {
				return BindingContext as ProfileModel;
			}
		}

		public ProfilePage()
		{
			InitializeComponent();

			Title = "Profile";

			BindingContext = new ProfileModel();

			//Set up key pair info
			entrySeed.Text = App.Locator.Profile.Seed;
			var mnemonic = App.Locator.Profile.Mnemonic;
			if (String.IsNullOrWhiteSpace(mnemonic)) {
                cellMnemonic.IsVisible = false;
			} else {
				entryMnemonic.Text = mnemonic;
			}

			ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
		}

		protected async override void OnAppearing()
		{
			if (firstLoad) {
				await LoadProfile();
			}
			base.OnAppearing();
		}

		private async System.Threading.Tasks.Task LoadProfile()
		{
			layoutActivity.IsVisible = true;
			activityIndicator.IsRunning = true;

			await ViewModel.Load();

			await layoutActivity.FadeTo(0);
			await contentProfile.FadeTo(1);

			layoutActivity.IsVisible = false;
		}

		void OnLogoutClicked()
		{
			App.Locator.Workspace.Logout();
		}

		async void SaveClicked(object sender, System.EventArgs e)
		{
			if (IsValid()) {
				App.ShowLoading(true);

				var result = await ViewModel.Save();
				ShowMessage(result ? "Profile info saved" : "Error saving profile info");

				App.ShowLoading(false);
			}
		}

		void ShowClicked(object sender, System.EventArgs e)
		{
			btnShow.IsVisible = false;
			entryMnemonic.IsVisible = true;
		}

		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateTextField(ViewModel.FullName)) {
				entryFullName.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateTextField(ViewModel.PhoneNumber)) {
				entryPhoneNumber.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateTextField(ViewModel.Address)) {
				entryAddress.Focus();
				return false;
			}

			return true;
		}

	}
}
