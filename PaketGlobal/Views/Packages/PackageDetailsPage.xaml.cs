using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Plugin.Geolocator;
using stellar_dotnetcore_sdk;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PackageDetailsPage : BasePage
    {
        private Package ViewModel { get { return BindingContext as Package; } }

        private bool CanAcceptPackage = false;
        private bool CanAssignPackage = false;

        private BarcodePackageData BarcodeData;

        public bool ShouldDismiss = false;

        public PackageDetailsPage(Package package, bool canAcceptPackage = false, BarcodePackageData barcodePackageData = null)
        {
            InitializeComponent();

            BindingContext = package;

            CanAcceptPackage = canAcceptPackage;
            BarcodeData = barcodePackageData;

            if(package.CourierPubkey==null && package.MyRole==PaketRole.Courier && CanAcceptPackage==false)
            {
                CanAssignPackage = true;
            }

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
                ShareButton.TranslationY = 11;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
            ShareButton.TranslationY = -18;
            ShareButton.TranslationX = -35;
#endif


            var data = new BarcodePackageData
            {
                EscrowAddress = package.PaketId
            };
            BarcodeImage.BarcodeOptions.Width = 300;
            BarcodeImage.BarcodeOptions.Height = 300;
            BarcodeImage.BarcodeOptions.Margin = 1;
            BarcodeImage.BarcodeValue = JsonConvert.SerializeObject(data);

            var barcodeTapCommand = new Command(() =>
            {
                if(!WaitingStackView.IsVisible)
                {
                    BarcodeImage.IsVisible = !BarcodeImage.IsVisible;

                    if (BarcodeImage.IsVisible)
                    {
                        BarcodeArrow.Source = "dropdown_arrow_top.png";
                    }
                    else
                    {
                        BarcodeArrow.Source = "dropdown_arrow.png";
                    }
                }
          
            });

            XamEffects.Commands.SetTap(BarcodeView, barcodeTapCommand);

            App.Locator.DeviceService.setStausBarLight();

            if(CanAssignPackage)
            {
                EmptyBox.IsVisible = true;
                AssignLabel.IsVisible = true;
                AssignButton.IsVisible = true;
                PaymentInfoViewFrame.IsVisible = false;
                EventsInfoViewFrame.IsVisible = false;
                FundInfoViewFrame.VerticalOptions = LayoutOptions.FillAndExpand;

                LauncherPhoneButton.IsVisible = false;
                RecipientPhoneButton.IsVisible = false;

                LauncherContactLabel.Text = AppResources.CourierNotContactVisible;
                RecipientContactLabel.Text = AppResources.CourierNotContactVisible;
            }
            else if(CanAcceptPackage)
            {
                EmptyBox.IsVisible = true;
                AcceptButton.IsVisible = true;
                PaymentInfoViewFrame.IsVisible = false;
                EventsInfoViewFrame.IsVisible = false;
                FundInfoViewFrame.VerticalOptions = LayoutOptions.FillAndExpand;

                CheckVisiblePayments();
            }
            else{
                AddEvents();

                EventsInfoViewFrame.IsVisible = true;

                CheckVisiblePayments();

                MessagingCenter.Subscribe<PackagesModel, Package>(this, Constants.DISPLAY_PACKAGE_CHANGED, (sender, arg) =>
                {
                    var _package = BindingContext as Package;
                    _package.Status = arg.Status;
                });
            }

            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;

                BindingContext = await PackageHelper.GetPackageDetails(ViewModel.PaketId);

                PullToRefresh.IsRefreshing = false;

                if (ViewModel.LauncherName != null)
                {
                    LauncherCallSignView.IsVisible = true;
                }

                if (ViewModel.RecipientName != null)
                {
                    RecipientCallSignView.IsVisible = true;
                }

                CheckVisiblePayments();
            });

            PullToRefresh.RefreshCommand = refreshCommand;


            if(ViewModel.LauncherName != null)
            {
                LauncherCallSignView.IsVisible = true;
            }

            if (ViewModel.RecipientName != null)
            {
                RecipientCallSignView.IsVisible = true;
            }


           var blockChainLinkCommand =  new Command(() =>
            {
                Device.OpenUri(new Uri(ViewModel.BlockchainUrl));
            });
            XamEffects.Commands.SetTap(BlockChainLinkLabel, blockChainLinkCommand);

            var packageLinkCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(ViewModel.PaketUrl));
            });
            XamEffects.Commands.SetTap(PackageLinkLabel, packageLinkCommand);


            LinksFrameView.WidthRequest = App.Locator.DeviceService.ScreenWidth();
            UsersFrameView.WidthRequest = App.Locator.DeviceService.ScreenWidth();

            LoadPhoto();
        }

        private async void LoadPhoto()
        {
            try{
                var result = await App.Locator.RouteServiceClient.GetPackagePhoto(ViewModel.PaketId);

                if (result != null)
                {
                    if (result.PackagePhoto != null)
                    {
                        var photo = result.PackagePhoto;

                        if (photo.Photo != null)
                        {
                            PhotoImage.Source = ImageSource.FromStream(
                                () => new MemoryStream(Convert.FromBase64String(photo.Photo)));
                            PhotoImage.IsVisible = true;
                        }
                    }
                }
            }
            catch (Exception)
            {

            }
        }

        private void CheckVisiblePayments()
        {
            if (ViewModel.MyRole == PaketRole.Courier)
            {
                if (ViewModel.CourierPubkey != null && ViewModel.PaymentTransaction==null)
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingLauncherMakeDeposit;

                    DepositButton.IsVisible = false;

                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;
                }

                if(ViewModel.CourierPubkey==null)
                {
                    LauncherPhoneButton.IsVisible = false;
                    RecipientPhoneButton.IsVisible = false;

                    LauncherContactLabel.Text = AppResources.CourierNotContactVisible;
                    RecipientContactLabel.Text = AppResources.CourierNotContactVisible;
                }
                else if (ViewModel.Status == "waiting pickup")
                {
                    LauncherPhoneButton.IsVisible = true;
                    LauncherContactLabel.Text = ViewModel.LauncherContact;

                    RecipientPhoneButton.IsVisible = false;
                    RecipientContactLabel.Text = AppResources.CourierNotContactVisible;

                    EmptyBox.IsVisible = true;
                    PaymentInfoViewFrame.IsVisible = false;
                    EventsInfoViewFrame.IsVisible = false;
                    FundInfoViewFrame.VerticalOptions = LayoutOptions.FillAndExpand;
                }
                else{
                    LauncherPhoneButton.IsVisible = true;
                    LauncherContactLabel.Text = ViewModel.LauncherContact;

                    RecipientPhoneButton.IsVisible = true;
                    RecipientContactLabel.Text = ViewModel.RecipientContact;
                }
            }
            else if(ViewModel.MyRole==PaketRole.Launcher)
            {

                if(ViewModel.CourierPubkey==null)
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingAssignCourierToPackage;
                    DepositButton.IsVisible = true;

                    DepositButton.IsEnabled = false;
                    DepositButton.ButtonBackground = "#A7A7A7";             
                }
                else{
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingMakeDepositPackage;
                    DepositButton.IsVisible = true;

                    if (!ViewModel.IsExpiredInList)
                    {
                        DepositButton.IsEnabled = false;
                        DepositButton.ButtonBackground = "#A7A7A7";   
                        WaitingAssignLabel.Text = AppResources.WaitingExpiredDepositPackage;
                    }
                    else{
                        DepositButton.IsEnabled = true;
                        DepositButton.ButtonBackground = "#4D64E8";
                    }
                }

                if(ViewModel.PaymentTransaction==null)
                {
                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;
                }
                else{
                    WaitingStackView.IsVisible = false;

                    BarcodeImage.IsVisible = true;
                    BarcodeArrow.IsVisible = true;
                }
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            if (CanAcceptPackage == false)
            {
                MessagingCenter.Unsubscribe<PackagesModel, Package>(this, Constants.DISPLAY_PACKAGE_CHANGED);
            }
        }

        private async void OnBack(object sender, System.EventArgs e)
        {
            if(ShouldDismiss)
            {
                await this.Navigation.PopModalAsync();
            }
            else{
                await Navigation.PopToRootAsync();  
            }
        }


        private async void OnShareClicked(object sender, System.EventArgs e)
        {
    
        }

        private void EscrowPubKeyCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.PaketId);
            ShowMessage(AppResources.Copied);
        }

        private void CopyLauncherPubkeyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.LauncherPubkey);
            ShowMessage(AppResources.Copied);
        }

        private void CopyLauncherNameClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.LauncherName);
            ShowMessage(AppResources.Copied);
        }

        private void CopyRecipientPubkeyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.RecipientPubkey);
            ShowMessage(AppResources.Copied);
        }

        private void CopyRecipientNameClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.RecipientName);
            ShowMessage(AppResources.Copied);
        }

        private void PackageLinkCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.PaketUrl);
            ShowMessage(AppResources.Copied);
        }

        private void BlockchainLinkCopyClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.BlockchainUrl);
            ShowMessage(AppResources.Copied);
        }

        private void CopyRecipientContactClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.RecipientContact);
            ShowMessage(AppResources.Copied);
        }

        private void CopyLauncherContactClicked(object sender, System.EventArgs e)
        {
            App.Locator.ClipboardService.SendTextToClipboard(ViewModel.LauncherContact);
            ShowMessage(AppResources.Copied);
        }


        private async void RefundClicked(object sender, System.EventArgs e)
        {
            App.ShowLoading(true);

            var result = await StellarHelper.RefundEscrow(ViewModel.RefundTransaction, ViewModel.MergeTransaction);

            if (result)
            {
                RefundButton.IsVisible = false;
                RefundLabel.IsVisible = false;

                ShowMessage(AppResources.RefundingOK);
            }

            App.ShowLoading(false);
        }

        private async void ReclaimClicked(object sender, System.EventArgs e)
        {
            App.ShowLoading(true);

            var result = await StellarHelper.ReclaimEscrow(ViewModel.MergeTransaction);

            if (result)
            {
                ReclaimButton.IsVisible = false;
                ReclaimLabel.IsVisible = false;

                ShowMessage(AppResources.ReclaimingOK);
            }
    

            App.ShowLoading(false);
        }

        private async void DepositClicked(object sender, System.EventArgs e)
        {
            Unfocus();

            ProgressBar.Progress = 0;
            ProgressLabel.Text = "";
            ProgressView.BackgroundColor = new Color(0, 0, 0, 0.7);
            ProgressView.IsVisible = true;

            try
            {
                var result = await StellarHelper.LaunchPackage(ViewModel.PaketId, ViewModel.RecipientPubkey, ViewModel.Deadline, ViewModel.CourierPubkey, ViewModel.Payment, ViewModel.Collateral, FinalizePackageEvents);

                if (result != StellarOperationResult.Success)
                {
                    ShowError(result);
                }
                else{
                    BindingContext = await PackageHelper.GetPackageDetails(ViewModel.PaketId);
                    CheckVisiblePayments();
                }
            }
            catch (Exception exc)
            {
                ShowErrorMessage(exc.Message);
            }

            ProgressView.IsVisible = false;
        }


		void FinalizePackageEvents(object sender, LaunchPackageEventArgs e)
		{
			ProgressBar.AnimationEasing = Easing.SinIn;
			ProgressBar.AnimationLength = 3000;
			ProgressBar.AnimatedProgress = e.Progress;
			ProgressLabel.Text = e.Message;
		}

        private async void AssignClicked(object sender, System.EventArgs e)
        {
            Unfocus();

            App.Locator.Packages.StopTimer();

            ProgressBar.Progress = 0;
            ProgressLabel.Text = AppResources.LaunchPackageStep0;
            ProgressView.BackgroundColor = new Color(0, 0, 0, 0.7);
            ProgressView.IsVisible = true;

            var myPubkey = App.Locator.Profile.Pubkey;

            string location = null;

            var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

            if (hasPermission)
            {
                var locator = CrossGeolocator.Current;

                var position = await locator.GetPositionAsync();

                if (position != null)
                {
                    location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            //I'm a courier
            var result = await StellarHelper.AssignPackage(ViewModel.PaketId, ViewModel.Collateral, location,FinalizePackageEvents);
            if (result == StellarOperationResult.Success)
            {
                await System.Threading.Tasks.Task.Delay(2000);

                await App.Locator.Packages.Load();

                MessagingCenter.Send(this, Constants.PACKAGE_ASSIGN, ViewModel.PaketId);

                ShowMessage(AppResources.PackageAssigned);

                await Navigation.PopToRootAsync();
            }
            else
            {
                ShowError(result);
            }

            ProgressView.IsVisible = false;

            App.Locator.Packages.StartTimer();
        }

        private async void AcceptClicked(object sender, System.EventArgs e)
        {
            App.ShowLoading(true);

            var myPubkey = App.Locator.Profile.Pubkey;
         
            string location = null;

            var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

            if (hasPermission)
            {
                var locator = CrossGeolocator.Current;

                var position = await locator.GetPositionAsync();

                if (position != null)
                {
                    location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            if (myPubkey == ViewModel.RecipientPubkey)
            {
                //I'm a recipient

                var result = await StellarHelper.AcceptPackageAsRecipient(BarcodeData.EscrowAddress, ViewModel.PaymentTransaction,location);
                if (result == StellarOperationResult.Success)
                {
                    await System.Threading.Tasks.Task.Delay(2000);

                    await App.Locator.Packages.Load();

                    ShowMessage(AppResources.PackageAccepted);

                    await Navigation.PopToRootAsync();
                }
                else
                {
                    ShowError(result);
                }

                App.ShowLoading(false);
            }
            else
            {
                //I'm a courier
                var result = await StellarHelper.AcceptPackageAsCourier(BarcodeData.EscrowAddress, ViewModel.Collateral, ViewModel.PaymentTransaction, location);
                if (result == StellarOperationResult.Success)
                {
                    await System.Threading.Tasks.Task.Delay(2000);

                    await App.Locator.Packages.Load();

                    ShowMessage(AppResources.PackageAccepted);

                    await  Navigation.PopToRootAsync();
                }
                else
                {
                    ShowError(result);
                }

                App.ShowLoading(false);
            }
        }


        private void AddEvents()
        {
            var package = ViewModel;

            if(package.Events==null)
            {
                EventsInfoViewFrame.IsVisible = false;
            }
            else{
                StackLayout lastStackView = null;

                RelativeLayout relativeLayout = new RelativeLayout();

                for (int i = 0; i < package.Events.Count; i++)
                {
                    var ev = package.Events[i];

                    var stack = new StackLayout();

                    var frame = new Frame()
                    {
                        HorizontalOptions = LayoutOptions.Start,
                        Padding = 3,
                        HasShadow = false,
                        BackgroundColor = Color.FromHex("#A5A5A5"),
                        CornerRadius = 10,
                        HeightRequest = 16
                    };

                    var eventTypeLabel = new Label()
                    {
                        Text = " " + ev.EventType.ToUpper() + "   ",
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.White,
                        FontSize = 10,
                        HeightRequest = 16,
                    };
                    eventTypeLabel.SetDynamicResource(Label.FontFamilyProperty, "NormalFont");

                    frame.Content = eventTypeLabel;

                    stack.Children.Add(frame);

                    var dateLabel = new Label()
                    {
                        Text = String.Format("{0:dd.MM.yyyy HH:mm}", ev.TimestampDT),
                        TextColor = Color.FromHex("#A7A7A7"),
                        FontSize = 10
                    };
                    dateLabel.SetDynamicResource(Label.FontFamilyProperty, "MediumFont");

                    stack.Children.Add(dateLabel);

                    var keyLabel = new Label()
                    {
                        Text = ev.PaketUser,
                        TextColor = Color.FromHex("#555555"),
                        FontSize = 12,
                    };
                    keyLabel.LineBreakMode = LineBreakMode.CharacterWrap;
                    keyLabel.WidthRequest = 140;
                    keyLabel.SetDynamicResource(Label.FontFamilyProperty, "MediumFont");
                    var keyTapCommand = new Command(() =>
                    {
                        App.Locator.ClipboardService.SendTextToClipboard(keyLabel.Text);
                        ShowMessage(AppResources.Copied);
                    });
                    XamEffects.TouchEffect.SetColor(keyLabel, Color.LightGray);
                    XamEffects.Commands.SetTap(keyLabel, keyTapCommand);
                    stack.Children.Add(keyLabel);

                    //var userLabel = new Label()
                    //{
                    //    Text = package.NameFromKey(ev.UserPubKey),
                    //    TextColor = Color.FromHex("#555555"),
                    //    FontSize = 12,
                    //};
                    //userLabel.LineBreakMode = LineBreakMode.CharacterWrap;
                    //userLabel.WidthRequest = 140;
                    //userLabel.SetDynamicResource(Label.FontFamilyProperty, "MediumFont");
                    //var userTapCommand = new Command(() =>
                    //{
                    //    App.Locator.ClipboardService.SendTextToClipboard(userLabel.Text);
                    //    ShowMessage(AppResources.Copied);
                    //});
                    //XamEffects.TouchEffect.SetColor(userLabel, Color.LightGray);
                    //XamEffects.Commands.SetTap(userLabel, userTapCommand);
                    //stack.Children.Add(userLabel);


                    var progressImage = new Image()
                    {
                        Source = "point_2.png",
                        HorizontalOptions = LayoutOptions.Start
                    };

                    var line = new BoxView()
                    {
                        HeightRequest = 1,
                        BackgroundColor = Color.FromHex("#53C5C7"),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };

                    if (lastStackView == null)
                    {
                        relativeLayout.Children.Add(stack,
                                  Constraint.RelativeToParent((parent) => { return 0; }));
                        
                        if(package.Events.Count>1)
                        {
                            relativeLayout.Children.Add(line,
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.X + 5; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Height + 14; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Width + 15; }),
                                    Constraint.Constant(0.5f));

                            relativeLayout.Children.Add(progressImage,
                                              Constraint.RelativeToParent((parent) => { return 0; }),
                                              Constraint.RelativeToView(stack, (parent, view) => { return view.Y + view.Height + 7; }));
                        }
                    }
                    else
                    {
                        relativeLayout.Children.Add(stack,
                                       Constraint.RelativeToView(lastStackView, (parent, view) => {
                                           return view.X + view.Width + 20;
                                       }));

                        if (i != (package.Events.Count - 1))
                        {
                            relativeLayout.Children.Add(line,
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.X + 5; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Height + 14; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Width + 15; }),
                                    Constraint.Constant(0.5f));
                        }

                        relativeLayout.Children.Add(progressImage,
                                       Constraint.RelativeToView(lastStackView, (parent, view) => {
                                           return view.X + view.Width + 20;
                                       }),
                                        Constraint.RelativeToView(stack, (parent, view) => { return view.Y + view.Height + 7; }));
                    }

                    lastStackView = stack;
                }

                if (lastStackView != null)
                {
                    EventsScrollView.Content = relativeLayout;
                }
                else
                {
                    EventsInfoViewFrame.IsVisible = false;
                }
            }
        }
    }
}
