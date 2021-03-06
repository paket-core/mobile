﻿using System;
using System.Collections.Generic;
using System.IO;
using Acr.UserDialogs;
using Plugin.Geolocator;
using stellar_dotnetcore_sdk;
using Xamarin.Forms;
using libphonenumber;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public partial class LaunchPackagePage : BasePage
	{

        private string recipient = "";
        private byte[] PhotoSource = null;

		private Package ViewModel {
			get {
				return BindingContext as Package;
			}
		}

		public LaunchPackagePage(Package package)
		{
			InitializeComponent();

            AddressBookHelper.LoadCallSigns();

            BindingContext = package;

            //set launcher phone
            var profile = App.Locator.ProfileModel;
            var number = profile.StoredPhoneNumber;
      
            PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
            try
            {
                PhoneNumber numberProto = phoneUtil.Parse(number, "");
                package.LauncherPhoneCode = "+" + Convert.ToString(numberProto.CountryCode);
                package.LauncherPhoneNumber = number.Replace(package.LauncherPhoneCode, "");
            }
            catch (NumberParseException)
            {
                package.LauncherPhoneNumber = number;
            }

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
            EntryLauncherPhoneNumber.TranslationY = 3;
            EntryRecipientPhoneNumber.TranslationY = 3;
#endif

            var selectMyCountryCommand = new Command(() =>
            {
                var picker = new CountryPickerPage();
                picker.eventHandler = DidSelectMyCountryHandler;
                Navigation.PushAsync(picker, true);
            });

            var selectRecipientCountryCommand = new Command(() =>
            {
                var picker = new CountryPickerPage();
                picker.eventHandler = DidSelectRecipientCountryHandler;
                Navigation.PushAsync(picker, true);
            });

            var selectFromLocation = new Command(() =>
            {
                GooglePlace place = null;

                if (ViewModel.FromLocationGPS!=null)
                {
                    string location = ViewModel.FromLocationGPS;

                    place = new GooglePlace();
                    place.Address = ViewModel.FromLocationAddress;
                    place.Latitude = Convert.ToDouble(location.Split(',')[0]);
                    place.Longitude = Convert.ToDouble(location.Split(',')[1]);
                }

                var picker = new LocationPickerPage(LocationPickerType.From, place);
                picker.eventHandler = DidSelectLocationHandler;
                Navigation.PushAsync(picker, true);
            });

            var selectToLocation = new Command(() =>
            {
                GooglePlace place = null;

                if (ViewModel.ToLocationGPS != null)
                {
                    string location = ViewModel.ToLocationGPS;

                    place = new GooglePlace();
                    place.Address = ViewModel.ToLocationAddress;
                    place.Latitude = Convert.ToDouble(location.Split(',')[0]);
                    place.Longitude = Convert.ToDouble(location.Split(',')[1]);
                }

                var picker = new LocationPickerPage(LocationPickerType.To, place);
                picker.eventHandler = DidSelectLocationHandler;
                Navigation.PushAsync(picker, true);
            });

            var openFullScreeenPhotoCommand = new Command(() =>
            {
                OpenPhotoFullScreen();
            });
            XamEffects.Commands.SetTap(PhotoImage, openFullScreeenPhotoCommand);

            XamEffects.Commands.SetTap(LauncherCountryCodeLabel, selectMyCountryCommand);
            XamEffects.Commands.SetTap(RecipientCountryCodeLabel, selectRecipientCountryCommand);
            XamEffects.Commands.SetTap(FromLocationLabel, selectFromLocation);
          //  XamEffects.Commands.SetTap(FromLocationFrame, selectFromLocation);
            XamEffects.Commands.SetTap(FromLocationImage, selectFromLocation);
            XamEffects.Commands.SetTap(ToLocationLabel, selectToLocation);
           // XamEffects.Commands.SetTap(ToLocationFrame, selectToLocation);
            XamEffects.Commands.SetTap(ToLocationImage, selectToLocation);

            EntryRecepient.MakeCustomOffset();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.IsNeedAlertDialogToCloseLaunchPackage = true;

            App.Locator.DeviceService.setStausBarLight();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.Locator.DeviceService.IsNeedAlertDialogToCloseLaunchPackage = false;
        }

        private void DidSelectLocationHandler(object sender, LocationPickerPageEventArgs e)
        {
            var page = sender as LocationPickerPage;

            var place = e.Item;

            string location = place.Latitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture) + "," + place.Longitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture);
            string address = place.Address;

            if(page.PickerType == LocationPickerType.From)
            {
                ViewModel.FromLocationGPS = location;
                ViewModel.FromLocationAddress = address;
                ViewModel.FromCountry = place.Country;
            }
            else if (page.PickerType == LocationPickerType.To)
            {
                ViewModel.ToLocationGPS = location;
                ViewModel.ToLocationAddress = address;
                ViewModel.ToCountry = place.Country;
            }

            SetLocationImage(page.PickerType,place.Latitude,place.Longitude);
        }

        private async void SetLocationImage(LocationPickerType pickerType, double lat, double lng)
        {
            var size = 300;

#if __ANDROID__
            size = 340;
#endif

            var mapHelper = new MapHelper();

            var mapImage = await mapHelper.GetStaticMap(lat, lng, 18, size, size);

            MemoryStream stream = null;

            if (mapImage != null)
            {
                stream = new MemoryStream(mapImage);
            }

            if (pickerType == LocationPickerType.From)
            {
                if (stream != null)
                {
                    FromLocationImage.Source = ImageSource.FromStream(() => stream);
                }
                else
                {
                    FromLocationImage.Source = "map_black_icon.png";
                }
            }
            else if (pickerType == LocationPickerType.To)
            {
                if (stream != null)
                {
                    ToLocationImage.Source = ImageSource.FromStream(() => stream);
                }
                else
                {
                    ToLocationImage.Source = "map_black_icon.png";
                }
            } 
        }

        private void DidSelectRecipientCountryHandler(object sender, CountryPickerPageEventArgs e)
        {
            ViewModel.RecipientPhoneCode = e.Item.CallingCode;
        }

        private void DidSelectMyCountryHandler(object sender, CountryPickerPageEventArgs e)
        {
            ViewModel.LauncherPhoneCode = e.Item.CallingCode;
        }

        private void DidSelectRecipientPhoneHandler(object sender, ContactsBookPageEventArgs e)
        {
            var contact = e.Item;
           
            if(contact.CountryCode!=null)
            {
                ViewModel.RecipientPhoneCode = contact.CountryCode;
                ViewModel.RecipientPhoneNumber = contact.NationalPhone;
            }
            else{
                ViewModel.RecipientPhoneNumber = contact.SimplePhone;
            }
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            //.
            EventHandler handler = (se, ee) => {
                if (ee != null)
                {
                    Navigation.PopAsync();
                }
            };

            ShowErrorMessage(AppResources.LaunchLeaveMessage, false, handler, AppResources.Leave, AppResources.Cancel);

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

        private async void OnTakePhoto(object sender, System.EventArgs e)
        {
            await Plugin.Media.CrossMedia.Current.Initialize();

            try{
                var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
                {
                    PhotoSize = Plugin.Media.Abstractions.PhotoSize.Large,
                });

                if (photo != null)
                {
                    PhotoButton.Image = AppResources.ReTakePhoto;

                    PhotoSource = GetImageBytes(photo.GetStream());

                    PhotoImage.Source = ImageSource.FromStream(() => {
                        return photo.GetStream();
                    });

                    PhotoImage.IsVisible = true;
                }
            }
            catch(Exception ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private byte[] GetImageBytes(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        private async void CreateClicked(object sender, System.EventArgs e)
        {
        
            if (EntryRecepient.IsBusy)
            {
                return;
            }

            if (IsValid())
            {
                if(recipient.Length==0)
                {
                    if(recipient.Length == 0)
                    {
                        EntryRecepient.FocusField();
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

					if (hasPermission) {
						var locator = CrossGeolocator.Current;
                      //  locator.DesiredAccuracy = DesiredAccuracy.Value;

                        var position = await locator.GetPositionAsync();

						if (position != null) {
							location = position.Latitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString("F7", System.Globalization.CultureInfo.InvariantCulture);
						}
					}

                    var result = await StellarHelper.CreatePackage(escrowKP, recipient, ViewModel.LauncherFullPhoneNumber, ViewModel.RecipientFullPhoneNumber, EntryDescription.Text, ViewModel.FromLocationAddress, ViewModel.ToLocationAddress, vm.Deadline, payment, collateral, location, ViewModel.FromLocationGPS, ViewModel.ToLocationGPS, PhotoSource, LaunchPackageEvents);

                    if (result == StellarOperationResult.Success)
                    {
                        App.Locator.DeviceService.IsNeedAlertDialogToCloseLaunchPackage = false;

                        await System.Threading.Tasks.Task.Delay(2000);
                        await App.Locator.Packages.Load();

                        await Navigation.PopAsync();
                    }
                    else if (result==StellarOperationResult.LowBULsLauncher)
                    {
                        EventHandler handler = (se, ee) => {
                            if (ee != null)
                            {
                                ShowPurchaseBuls();
                            }
                        };

                        ShowErrorMessage(AppResources.PurchaseBULs, false, handler, AppResources.Purchase);
                    }
                    else
                    {
                        ShowError(result);
                    }
                }
                catch (Exception exc)
                {
                    EventHandler handler = (se, ee) => {
                        if (ee != null)
                        {
                            ShowPurchaseBuls();
                        }
                    };

                    if (exc.Message == AppResources.InsufficientBULs)
                    {
                        ShowErrorMessage(AppResources.PurchaseBULs, false, handler, AppResources.Purchase);
                    }
                    else
                    {
                        ShowErrorMessage(exc.Message);
                    }
                }

                ProgressView.IsVisible = false;

                App.Locator.Wallet.StartTimer();
                App.Locator.Packages.StartTimer();
            }
        }

        private void ShowPurchaseBuls()
        {
            WalletPage page = new WalletPage();
            page.ShowPurchaseBuls = true;
            this.Navigation.PushAsync(page);
        }

        private async void ContactsButtonClicked(object sender, EventArgs e)
        {
            this.Unfocus();

            try
            {
                var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Contacts);

                if (hasPermission)
                {
                    var result = await App.Locator.DeviceService.OpenAddressBook();
                    if(result!=null)
                    {
                        var contact = new BookContact("",result,"");

                        if (contact.CountryCode != null)
                        {
                            ViewModel.RecipientPhoneCode = contact.CountryCode;
                            ViewModel.RecipientPhoneNumber = contact.NationalPhone;
                        }
                        else
                        {
                            ViewModel.RecipientPhoneNumber = contact.SimplePhone;
                        }
                    }
                }
                else
                {
                    ShowMessage(AppResources.ContactsAccessNotGranted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            //ContactsBookPage page = new ContactsBookPage();
            //page.eventHandler = DidSelectRecipientPhoneHandler;
            //this.Navigation.PushAsync(page);
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

        private void OpenPhotoFullScreen()
        {
            string photoBase64 = Convert.ToBase64String(PhotoSource);
            var photoPage = new PhotoFullScreenPage(photoBase64, null);
            photoPage.BackgroundColor = Color.Black;

            var navigation = new NavigationPage(photoPage);
            navigation.BackgroundColor = Color.Black;
            navigation.BarTextColor = Color.White;
            navigation.BarBackgroundColor = Color.Black;

            Navigation.PushModalAsync(navigation);
        }

        private async void DidSelectItemHandler(object sender, AddressBookPageEventArgs e)
        {
            var page = sender as AddressBookPage;

            EntryRecepient.Text = e.Item;
            recipient = await EntryRecepient.CheckValidCallSignOrPubKey();
        }

        private async void FieldUnfocus(object sender, EventArgs e)
        {
            if (sender == EntryRecepient)
            {
                recipient = await EntryRecepient.CheckValidCallSignOrPubKey();
            }
        }

        private void FieldCompleted(object sender, EventArgs e)
        {
            if (sender == EntryRecepient)
            {
                if (!ValidationHelper.ValidateTextField(EntryRecipientPhoneNumber.Text))
                {
                    EntryRecipientPhoneNumber.Focus();
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

        public async void Scroll()
        {
            await Task.Delay(TimeSpan.FromSeconds(0.2f));
            await MainScrollView.ScrollToAsync(MainScrollView.ScrollX, MainScrollView.ScrollY - 40, false);
        }

        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateTextField(EntryRecepient.Text))
            {
                EntryRecepient.FocusField();
                Scroll();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.LauncherFullPhoneNumber))
            {
                EntryLauncherPhoneNumber.Focus();
                Scroll();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.RecipientFullPhoneNumber))
            {
                EntryRecipientPhoneNumber.Focus();
                Scroll();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.FromLocationGPS))
            {
                EventHandler handleHandler = (s, e) => {
                    var picker = new LocationPickerPage(LocationPickerType.From);
                    picker.eventHandler = DidSelectLocationHandler;
                    Navigation.PushAsync(picker, true);
                };

                ShowErrorMessage(AppResources.LocationsNotSet, false, handleHandler);

                return false;
            }
            else if (!ValidationHelper.ValidateTextField(ViewModel.ToLocationGPS))
            {
                EventHandler handleHandler = (s, e) => {
                    var picker = new LocationPickerPage(LocationPickerType.To);
                    picker.eventHandler = DidSelectLocationHandler;
                    Navigation.PushAsync(picker, true);
                };

                ShowErrorMessage(AppResources.LocationsNotSet, false, handleHandler);

                return false;
            }
            else if (PhotoSource==null)
            {
                EventHandler handleHandler = (s, e) => {
                    OnTakePhoto(PhotoButton, null);
                };

                ShowErrorMessage(AppResources.SelectPackagePhoto, false, handleHandler);

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
            else if (!ValidationHelper.ValidateTextField(EntryDescription.Text))
            {
                EntryDescription.Focus();
                Scroll();

                return false;
            }
      

            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            BackButton.IsEnabled = enabled;
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

        private void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (sender==EntryPayment)
            {
                try
                {
                    if (string.IsNullOrEmpty(EntryPayment.Text))
                    {
                        PaymentEuroLabel.IsVisible = false;
                    }
                    else
                    {
                        double payment = double.Parse(EntryPayment.Text);

                        double result = payment * App.Locator.Wallet.BUL_Ratio;

                        var euro = "€" + StellarConverter.ConvertEuroValueToString(result);

                        PaymentEuroLabel.Text = euro;

                        if (!PaymentEuroLabel.IsVisible)
                        {
                            PaymentEuroLabel.IsVisible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    PaymentEuroLabel.IsVisible = false;
                }
            }
            else if(sender==EntryCollateral){
                try
                {
                    if (string.IsNullOrEmpty(EntryCollateral.Text))
                    {
                        CollateralEuroLabel.IsVisible = false;
                    }
                    else
                    {
                        double payment = double.Parse(EntryCollateral.Text);

                        double result = payment * App.Locator.Wallet.BUL_Ratio;

                        var euro = "€" + StellarConverter.ConvertEuroValueToString(result);

                        CollateralEuroLabel.Text = euro;

                        if (!CollateralEuroLabel.IsVisible)
                        {
                            CollateralEuroLabel.IsVisible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    CollateralEuroLabel.IsVisible = false;
                }
            }
        }
	}
}
