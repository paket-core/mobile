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

        private bool IsShowSecret = false;

		public ProfilePage()
		{
			InitializeComponent();

            App.Locator.DeviceService.setStausBarLight();

			BindingContext = new ProfileModel();

            #if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 25;
                LogoutButton.TranslationY = 8;
                LogoutButton.TranslationX = 22;
            }
            else
            {
                TitleLabel.TranslationY = 24;
                LogoutButton.TranslationY = 10;
                LogoutButton.TranslationX = 25;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            LogoutButton.TranslationY = -8;
            LogoutButton.TranslationX = 2;
#endif

            var mnemonic = App.Locator.Profile.Mnemonic;
            if (String.IsNullOrWhiteSpace(mnemonic)) {
                MnemonicView.IsVisible = false;
			}

            if(App.Locator.AccountService.ShowNotifications)
            {
                NotificationsButton.Image = "swift_on.png";
            }
            else{
                NotificationsButton.Image = "swift_off.png";
            }


            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;

                await LoadProfile();

                PullToRefresh.IsRefreshing = false;
            });

            PullToRefresh.RefreshCommand = refreshCommand;
		}

		protected async override void OnAppearing()
		{
            App.Locator.DeviceService.setStausBarLight();

            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                ActivityIndicator.IsVisible = true;
                ActivityIndicator.IsRunning = true;
                MainScrollView.IsVisible = false;
                LogoutButton.IsVisible = false;

                await LoadProfile();
            }
		}

		private async System.Threading.Tasks.Task LoadProfile()
		{
            await ViewModel.Load();

            ActivityIndicator.IsVisible = false;
            ActivityIndicator.IsRunning = false;
            MainScrollView.IsVisible = true;
            LogoutButton.IsVisible = true;
		}

        private void OnLogoutClicked(object sender, System.EventArgs e)
		{
			App.Locator.Workspace.Logout();
		}

        private async void SaveClicked(object sender, System.EventArgs e)
		{
			if (IsValid()) {
				App.ShowLoading(true);

				var result = await ViewModel.Save();
				ShowMessage(result ? "Profile info saved" : "Error saving profile info");

				App.ShowLoading(false);
			}
		}

        private void NotificationsClicked(object sender, System.EventArgs e)
        {
            bool enabled = !App.Locator.AccountService.ShowNotifications;

            if (enabled)
            {
                NotificationsButton.Image = "swift_on.png";
            }
            else
            {
                NotificationsButton.Image = "swift_off.png";
            }

            App.Locator.AccountService.ShowNotifications = enabled;
        }

        private void ShowClicked(object sender, System.EventArgs e)
        {
            IsShowSecret = !IsShowSecret;

            if(IsShowSecret)
            {
                SecretLabel.Text = App.Locator.Profile.Seed;
                MnemonicLabel.Text = App.Locator.Profile.Mnemonic;
                KeysButton.Image = "swift_on.png";
            }
            else{
                SecretLabel.Text = "•••••••••";
                MnemonicLabel.Text = "•••••••••";
                KeysButton.Image = "swift_off.png";
            }

        }

        private void SecretCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(App.Locator.Profile.Seed);
            ShowMessage("Copied to clipboard");
        }

        private void MnemonicCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(App.Locator.Profile.Mnemonic);
            ShowMessage("Copied to clipboard");        
        }

		protected override bool IsValid()
		{
            if (!ValidationHelper.ValidateTextField(ViewModel.FullName)) {
                EntryFullName.Focus();
            	return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.PhoneNumber)) {
                PhoneEntry.Focus();
            	return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.Address)) {
            	AddressEntry.Focus();
            	return false;
            }

            return true;
		}

	}
}
