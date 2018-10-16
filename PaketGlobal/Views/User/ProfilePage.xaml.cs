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
            else{
                var array = mnemonic.Split(' ');

                mnemonicLabel_1.Text = array[0];
                mnemonicLabel_2.Text = array[1];
                mnemonicLabel_3.Text = array[2];
                mnemonicLabel_4.Text = array[3];
                mnemonicLabel_5.Text = array[4];
                mnemonicLabel_6.Text = array[5];
                mnemonicLabel_7.Text = array[6];
                mnemonicLabel_8.Text = array[7];
                mnemonicLabel_9.Text = array[8];
                mnemonicLabel_10.Text = array[9];
                mnemonicLabel_11.Text = array[10];
                mnemonicLabel_12.Text = array[11];
            }


            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;

                await LoadProfile();

                PullToRefresh.IsRefreshing = false;
            });

            PullToRefresh.RefreshCommand = refreshCommand;

            SwipeListener swipeListener = new SwipeListener(this.Content, this);

            PubkeyLabel.Text = App.Locator.Profile.Pubkey;

            var whyTapCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(Constants.WHY_BLOCKED));
            });

            XamEffects.Commands.SetTap(whyButton, whyTapCommand);

            if (ViewModel.Address!=null)
            {
                if (ViewModel.Address.ToLower() == "united states of america" || ViewModel.Address.ToLower() == "usa")
                {
                    ErrorView.IsVisible = true;
                }
            }

            UpdateButton.Disabled = true;
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

            if (ViewModel.Address != null)
            {
                if (ViewModel.Address.ToLower() == "united states of america" || ViewModel.Address.ToLower() == "usa")
                {
                    ErrorView.IsVisible = true;
                }
                else{
                    ErrorView.IsVisible = false;
                }
            }
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
                var mnemonic = App.Locator.Profile.Mnemonic;
                if (!String.IsNullOrWhiteSpace(mnemonic))
                {
                    MnemonicStack.IsVisible = false;
                    MnemonicFrame.IsVisible = true;
                }

                SecretLabel.Text = App.Locator.Profile.Seed;
                KeysButton.Image = "swift_on.png";
            }
            else{
                SecretLabel.Text = "•••••••••";
                KeysButton.Image = "swift_off.png";

                var mnemonic = App.Locator.Profile.Mnemonic;
                if (!String.IsNullOrWhiteSpace(mnemonic))
                {
                    MnemonicStack.IsVisible = true;
                    MnemonicFrame.IsVisible = false;
                }
            }

        }

        private void PubkeyCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(App.Locator.Profile.Pubkey);
            ShowMessage(AppResources.Copied);
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
            	return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.PhoneNumber)) {
            	return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.Address)) {
            	return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.Address))
            {
                return false;
            }
            else if(ErrorView.IsVisible){
                return false;
            }

            return true;
		}

        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            AddressEntry.Unfocus();

            var picker = new AddressPickerPage();
            picker.eventHandler = DidSelectAddressHandler;
            Navigation.PushAsync(picker, true);
        }

        private void ReadClicked(object sender, EventArgs e)
        {
            Device.OpenUri(new Uri(Constants.WHY_BLOCKED));
        }

        private void DidSelectAddressHandler(object sender, CountryPickerPageEventArgs e)
        {
            ViewModel.Address = e.Item.Name;

            if (ViewModel.Address != null)
            {
                if (ViewModel.Address.ToLower() == "united states of america" || ViewModel.Address.ToLower() == "usa")
                {
                    ErrorView.IsVisible = true;
                }
                else
                {
                    ErrorView.IsVisible = false;
                }
            }

            EnableDisableButton();
        }

        private void Handle_TextChanged(object sender, TextChangedEventArgs e)
        {
            EnableDisableButton();
        }

        private void EnableDisableButton()
        {
            if (!IsValid())
            {
                UpdateButton.Disabled = true;
            }
            else
            {
                UpdateButton.Disabled = false;
            }
        }

    }
}
