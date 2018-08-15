﻿using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using Plugin.Geolocator;
using stellar_dotnetcore_sdk;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class LaunchPackagePage : BasePage
	{

        private string recipient = "";
        private string courier = "";

		private Package ViewModel {
			get {
				return BindingContext as Package;
			}
		}

		public LaunchPackagePage(Package package)
		{
			InitializeComponent();

            BindingContext = package;

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else{
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif

        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void PickerFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            EntryDeadline.Unfocus();
            Unfocus();

            var dpc = new DatePromptConfig();
            dpc.OkText = AppResources.OK;
            dpc.CancelText = AppResources.Cancel;
            dpc.IsCancellable = true;
            dpc.MinimumDate = DateTime.Today.AddDays(1);
            dpc.SelectedDate = ViewModel.DeadlineDT.Date;
            dpc.Title = AppResources.PickDeadlineDate;
            dpc.OnAction = dateResult =>
            {
                if (dateResult.Ok)
                {
                    var date = dateResult.SelectedDate.Date;
                    date = date.AddSeconds(86399);//23:59.59 of selected day                    
                    ViewModel.Deadline = DateTimeHelper.ToUnixTime(date.ToUniversalTime());

                    EntryDeadline.Text = ViewModel.DeadlineString;

                    SelectButton(CustomDateButton);
                }
            };

            UserDialogs.Instance.DatePrompt(dpc);
        }

        private void DateButtonClicked(object sender, System.EventArgs e)
        {
            int addedDays = 0;

            if(sender == CustomDateButton) {
                PickerFocused(null, null);
            }
            else if (sender==DayButton){
                addedDays = 1;
            }
            else if (sender==OneWeekButton){
                addedDays = 7;
            }
            else if (sender == TwoWeekButton)
            {
                addedDays = 14;
            }

            if (addedDays>0){
                SelectButton(sender as Button);

                var date = DateTime.Today.Date.AddDays(addedDays);
                date = date.AddSeconds(86399);//23:59.59 of selected day

                ViewModel.Deadline = DateTimeHelper.ToUnixTime(date.ToUniversalTime());

                EntryDeadline.Text = ViewModel.DeadlineString;
            }
        }

        private void SelectButton(Button btn) {
            OneWeekButton.BackgroundColor = Color.White;
            TwoWeekButton.BackgroundColor = Color.White;
            CustomDateButton.BackgroundColor = Color.White;
            DayButton.BackgroundColor = Color.White;

            OneWeekButton.TextColor = Color.FromHex("#4D64E8");
            TwoWeekButton.TextColor = Color.FromHex("#4D64E8");
            CustomDateButton.TextColor = Color.FromHex("#4D64E8");
            DayButton.TextColor = Color.FromHex("#4D64E8");

            btn.BackgroundColor = Color.FromHex("#4D64E8");
            btn.TextColor = Color.White;
        }

        private async void CreateClicked(object sender, System.EventArgs e)
        {
            if(EntryCourier.IsBusy || EntryRecepient.IsBusy)
            {
                return;
            }

            if (IsValid())
            {
                if(recipient.Length==0 || courier.Length==0)
                {
                    if(recipient.Length == 0)
                    {
                        EntryRecepient.FocusField();
                    }
                    else{
                        EntryCourier.FocusField(); 
                    }
                    return;
                }

                Unfocus();

                ProgressBar.Progress = 0;
                ProgressLabel.Text = AppResources.LaunchPackageStep0;
                ProgressView.BackgroundColor = new Color(0, 0, 0, 0.7);
                ProgressView.IsVisible = true;

                var vm = ViewModel;

                var escrowKP = KeyPair.Random();
            
                try{
                    App.Locator.Wallet.StopTimer();
                    App.Locator.Packages.StopTimer();

                    double payment = double.Parse(EntryPayment.Text);
                    double collateral = double.Parse(EntryCollateral.Text);

                    string location = null;

                    var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

                    if(hasPermission)
                    {
                        var locator = CrossGeolocator.Current;

                        var position = await locator.GetPositionAsync();

                        if(position!=null)
                        {
                            location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                        
                            if(location.Length>24)
                            {
                                location = location.Substring(0, 24);
                            }
                        }
                    }

                    var result = await StellarHelper.LaunchPackage(escrowKP, recipient, vm.Deadline, courier, payment, collateral, location, LaunchPackageEvents);

                    if (result == StellarOperationResult.Success)
                    {
                        await System.Threading.Tasks.Task.Delay(2000);
                        await App.Locator.Packages.Load();

                        OnBack(BackButton, null);
                    }
                    else
                    {
                        ShowError(result);
                    }
                }
                catch (Exception exc)
                {
                    ShowErrorMessage(exc.Message);
                }

                ProgressView.IsVisible = false;

                App.Locator.Wallet.StartTimer();
                App.Locator.Packages.StartTimer();
            }
        }

        private void AddressButtonClicked(object sender, EventArgs e)
        {
            this.Unfocus();

            AddressBookPage page;

            if(sender==EntryRecepient){
                page = new AddressBookPage(false);
            }
            else{
                page = new AddressBookPage(true);
            }

            page.eventHandler = DidSelectItemHandler;

            this.Navigation.PushAsync(page);

        }

        private async void DidSelectItemHandler(object sender, AddressBookPageEventArgs e)
        {
            var page = sender as AddressBookPage;

            if(page.IsCourierSelect)
            {
                EntryCourier.Text = e.Item;
                courier = await EntryCourier.CheckValidCallSignOrPubKey();
            }
            else{
                EntryRecepient.Text = e.Item;
                recipient = await EntryRecepient.CheckValidCallSignOrPubKey();
            }
        }

        private async void FieldUnfocus(object sender, EventArgs e)
        {
            if (sender == EntryRecepient)
            {
                recipient = await EntryRecepient.CheckValidCallSignOrPubKey();
            }
            else if (sender == EntryCourier)
            {
                courier = await EntryCourier.CheckValidCallSignOrPubKey();
            } 
        }

        private void FieldCompleted(object sender, EventArgs e)
        {
            if (sender == EntryRecepient)
            {
                if (!ValidationHelper.ValidateTextField(EntryCourier.Text))
                {
                    EntryCourier.FocusField();
                }
            }
            else if (sender == EntryCourier)
            {
                if (!ValidationHelper.ValidateTextField(EntryRecepient.Text))
                {
                    EntryRecepient.FocusField();
                }
            }
            else if (sender == EntryPayment)
            {
                if (!ValidationHelper.ValidateNumber(EntryCollateral.Text))
                {
                    EntryCollateral.Focus();
                }
            }
            else if (sender == EntryCollateral)
            {
                if (!ValidationHelper.ValidateNumber(EntryPayment.Text))
                {
                    EntryPayment.Focus();
                }
            }
        }

        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateTextField(EntryRecepient.Text))
            {
                EntryRecepient.FocusField();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(EntryCourier.Text))
            {
                EntryCourier.FocusField();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(EntryDeadline.Text))
            {
                EventHandler handleHandler = (s, e) => {                     EntryDeadline.Focus();                 };                  ShowErrorMessage(AppResources.SelectDeadlineDate, false, handleHandler); 
                return false;
            }
            else if (!ValidationHelper.ValidateNumber(EntryPayment.Text))
            {
                EntryPayment.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateNumber(EntryCollateral.Text))
            {
                EntryCollateral.Focus();
                return false;
            }
    
            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            BackButton.IsEnabled = enabled;
            EntryCourier.IsEnabled = enabled;
            EntryPayment.IsEnabled = enabled;
            EntryDeadline.IsEnabled = enabled;
            EntryCollateral.IsEnabled = enabled;
            EntryRecepient.IsEnabled = enabled;
        }

        void LaunchPackageEvents(object sender, LaunchPackageEventArgs e)
        {
            ProgressBar.AnimationEasing = Easing.SinIn;
            ProgressBar.AnimationLength = 3000;
            ProgressBar.AnimatedProgress = e.Progress;

            ProgressLabel.Text = e.Message;
        }

	}
}
