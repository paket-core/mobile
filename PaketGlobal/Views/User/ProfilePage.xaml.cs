using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ProfilePage : BasePage,ISwipeCallBack
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


            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;

                await LoadProfile();

                PullToRefresh.IsRefreshing = false;
            });

            PullToRefresh.RefreshCommand = refreshCommand;


            SwipeListener swipeListener = new SwipeListener(this.Content, this);


		}

        public void onBottomSwipe(View view)
        {
            if (view == ProfileView)
            {
            }
        }

        public void onLeftSwipe(View view)
        {
            if (view == ProfileView)
            {
            }
        }

        public void onNothingSwiped(View view)
        {
            if (view == ProfileView)
            {
            }
        }

        public void onRightSwipe(View view)
        {
            if (view == ProfileView)
            {
            }
        }

        public void onTopSwipe(View view)
        {
            if (view == ProfileView)
            {
            }
        }

		protected async override void OnAppearing()
		{
            App.Locator.DeviceService.setStausBarLight();

            App.Locator.DeviceService.IsNeedAlertDialogToClose = true;

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

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.Locator.DeviceService.IsNeedAlertDialogToClose = false;
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
                if(result)
                {
                    ShowMessage(AppResources.ProfileSaved);
                }
                else{
                    ShowErrorMessage(AppResources.ProfileNotSaved);
                }

				App.ShowLoading(false);
			}
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
            ShowMessage(AppResources.Copied);
        }

        private void MnemonicCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(App.Locator.Profile.Mnemonic);
            ShowMessage(AppResources.Copied);        
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
