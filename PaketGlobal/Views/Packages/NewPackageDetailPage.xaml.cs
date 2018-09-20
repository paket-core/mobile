using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class NewPackageDetailPage : BasePage
    {
        private Package ViewModel { get { return BindingContext as Package; } }
        private string photoBase64 = null;
        private BarcodePackageData BarcodeData;

        private bool CanAcceptPackage = false;
        private bool CanAssignPackage = false;

        public bool ShouldDismiss = false;

        public NewPackageDetailPage(Package package, bool canAcceptPackage = false, BarcodePackageData barcodePackageData = null)
        {
            InitializeComponent();

            BarcodeData = barcodePackageData;

            CanAcceptPackage = canAcceptPackage;

            if (package.CourierPubkey == null && package.MyRole == PaketRole.Courier && CanAcceptPackage == false)
            {
                CanAssignPackage = true;
            }

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif

            InfoPacketView.WidthRequest = App.Locator.DeviceService.ScreenWidth();

            BindingContext = package;
          
            AddCommands();
            LoadPhoto();
            LoadBarcode();
            CheckVisiblePayments();
            AddEvents();

            MessagingCenter.Subscribe<PackagesModel, Package>(this, Constants.DISPLAY_PACKAGE_CHANGED, (sender, arg) =>
            {
                var _package = BindingContext as Package;
                _package.Status = arg.Status;

                RefreshDetails();
            });

            if(CanAssignPackage)
            {
                AssignLabel.IsVisible = true;
                AssignButton.IsVisible = true;
                EmptyBox.IsVisible = true;
            }
            else if(CanAcceptPackage)
            {
                EmptyBox.IsVisible = true;
                AcceptButton.IsVisible = true;
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            MessagingCenter.Unsubscribe<PackagesModel, Package>(this, Constants.DISPLAY_PACKAGE_CHANGED);
        }

        private async void RefreshDetails()
        {
            PullToRefresh.IsRefreshing = true;

            BindingContext = await PackageHelper.GetPackageDetails(ViewModel.PaketId);
            CheckVisiblePayments();

            PullToRefresh.IsRefreshing = false;
        }

        private void AddCommands()
        {
            //pull to refresh
            var refreshCommand = new Command(() =>
            {
                RefreshDetails();
            });
            PullToRefresh.RefreshCommand = refreshCommand;

            //location
            var locationTapCommand = new Command(async () =>
            {
                bool isVisible = !LocationDetailsStackView.IsVisible;
                if (isVisible)
                {
                    LocationDetailsStackView.IsVisible = true;
                    await LocationDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await LocationDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    LocationDetailsStackView.IsVisible = false;
                }

                LocationDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(LocationTopStackView, locationTapCommand);

            //info
            var infoTapCommand = new Command(async () =>
            {
                bool isVisible = !InfoDetailsStackView.IsVisible;
                if (isVisible)
                {
                    InfoDetailsStackView.IsVisible = true;
                    await InfoDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await InfoDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    InfoDetailsStackView.IsVisible = false;
                }

                InfoDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(InfoTopStackView, infoTapCommand);

            var openPhotoCommand = new Command(() =>
            {
                var photoPage = new PhotoFullScreenPage(photoBase64, null);
                photoPage.BackgroundColor = Color.Black;

                var navigation = new NavigationPage(photoPage);
                navigation.BackgroundColor = Color.Black;
                navigation.BarTextColor = Color.White;
                navigation.BarBackgroundColor = Color.Black;

                Navigation.PushModalAsync(navigation);
            });
            XamEffects.Commands.SetTap(PhotoImage, openPhotoCommand);

            var openBarcodeCommand = new Command(() =>
            {
                var photoPage = new PhotoFullScreenPage(null, BarcodeImage.BarcodeValue);
                photoPage.BackgroundColor = Color.Black;

                var navigation = new NavigationPage(photoPage);
                navigation.BackgroundColor = Color.Black;
                navigation.BarTextColor = Color.White;
                navigation.BarBackgroundColor = Color.Black;

                Navigation.PushModalAsync(navigation);
            });
            XamEffects.Commands.SetTap(BarcodeImage, openBarcodeCommand);

            var blockChainLinkCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(ViewModel.BlockchainUrl));
            });
            XamEffects.Commands.SetTap(BlockChainLinkLabel, blockChainLinkCommand);

            var packageLinkCommand = new Command(() =>
            {
                Device.OpenUri(new Uri(ViewModel.PaketUrl));
            });
            XamEffects.Commands.SetTap(PackageLinkLabel, packageLinkCommand);


            //Funds
            var fundsTapCommand = new Command(async () =>
            {
                bool isVisible = !FundsDetailsStackView.IsVisible;
                if (isVisible)
                {
                    FundsDetailsStackView.IsVisible = true;
                    await FundsDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await FundsDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    FundsDetailsStackView.IsVisible = false;
                }

                FundsDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(FundsTopStackView, fundsTapCommand);

            //Payemnt
            var paymentTapCommand = new Command(async () =>
            {
                bool isVisible = !PaymentDetailsStackView.IsVisible;
                if (isVisible)
                {
                    PaymentDetailsStackView.IsVisible = true;
                    await PaymentDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await PaymentDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    PaymentDetailsStackView.IsVisible = false;
                }

                PaymentDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(PaymentTopStackView, paymentTapCommand);

            //Users
            var usersTapCommand = new Command(async () =>
            {
                bool isVisible = !UsersDetailsStackView.IsVisible;
                if (isVisible)
                {
                    UsersDetailsStackView.IsVisible = true;
                    await UsersDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await UsersDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    UsersDetailsStackView.IsVisible = false;
                }

                UsersDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(UsersTopStackView, usersTapCommand);

            //Events
            var eventsTapCommand = new Command(async () =>
            {
                bool isVisible = !EventsDetailsStackView.IsVisible;
                if (isVisible)
                {
                    EventsDetailsStackView.IsVisible = true;
                    await EventsDetailsStackView.FadeTo(1, 500, Easing.SinIn);
                }
                else
                {
                    await EventsDetailsStackView.FadeTo(0, 250, Easing.SinOut);
                    EventsDetailsStackView.IsVisible = false;
                }

                EventsDetailsStackView.IsVisible = isVisible;
            });
            XamEffects.Commands.SetTap(EventsTopStackView, eventsTapCommand);
        }

        private void LoadBarcode()
        {
            var data = new BarcodePackageData
            {
                EscrowAddress = ViewModel.PaketId
            };
            BarcodeImage.BarcodeOptions.Width = 300;
            BarcodeImage.BarcodeOptions.Height = 300;
            BarcodeImage.BarcodeOptions.Margin = 1;
            BarcodeImage.BarcodeValue = JsonConvert.SerializeObject(data);
        }

        private async void LoadPhoto()
        {
            try
            {
                var result = await App.Locator.RouteServiceClient.GetPackagePhoto(ViewModel.PaketId);

                if (result != null)
                {
                    if (result.PackagePhoto != null)
                    {
                        var photo = result.PackagePhoto;

                        if (photo.Photo != null)
                        {
                            photoBase64 = photo.Photo;

                            PhotoImage.Source = ImageSource.FromStream(
                                () => new MemoryStream(Convert.FromBase64String(photoBase64)));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void CheckVisiblePayments()
        {
            if (ViewModel.LauncherName != null)
            {
                LauncherCallSignView.IsVisible = true;
            }

            if (ViewModel.RecipientName != null)
            {
                RecipientCallSignView.IsVisible = true;
            }

            if (ViewModel.MyRole == PaketRole.Courier)
            {
                if (ViewModel.CourierPubkey != null && ViewModel.PaymentTransaction == null)
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingLauncherMakeDeposit;

                    DepositButton.IsVisible = false;

                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;
                }

                if (ViewModel.CourierPubkey == null)
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingCourierAssign;

                    DepositButton.IsVisible = false;

                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;

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

                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;

                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingCourierAccept;
                }
                else
                {
                    LauncherPhoneButton.IsVisible = true;
                    LauncherContactLabel.Text = ViewModel.LauncherContact;

                    RecipientPhoneButton.IsVisible = true;
                    RecipientContactLabel.Text = ViewModel.RecipientContact;
                }
            }
            else if (ViewModel.MyRole == PaketRole.Launcher)
            {

                if (ViewModel.CourierPubkey == null)
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingAssignCourierToPackage;
                    DepositButton.IsVisible = true;

                    DepositButton.IsEnabled = false;
                    DepositButton.ButtonBackground = "#A7A7A7";
                }
                else
                {
                    WaitingStackView.IsVisible = true;
                    WaitingAssignLabel.IsVisible = true;
                    WaitingAssignLabel.Text = AppResources.WaitingMakeDepositPackage;
                    DepositButton.IsVisible = true;

                    if (!ViewModel.IsExpiredInList && ViewModel.IsExpired)
                    {
                        DepositButton.IsEnabled = false;
                        DepositButton.ButtonBackground = "#A7A7A7";
                        WaitingAssignLabel.Text = AppResources.WaitingExpiredDepositPackage;
                    }
                    else
                    {
                        DepositButton.IsEnabled = true;
                        DepositButton.ButtonBackground = "#4D64E8";
                    }
                }

                if (ViewModel.PaymentTransaction == null)
                {
                    BarcodeArrow.IsVisible = false;
                    BarcodeImage.IsVisible = false;
                }
                else
                {
                    WaitingStackView.IsVisible = false;

                    BarcodeImage.IsVisible = true;
                    BarcodeArrow.IsVisible = true;
                }
            }
        }

        private async void OnBack(object sender, System.EventArgs e)
        {
            if (ShouldDismiss)
            {
                await this.Navigation.PopModalAsync();
            }
            else
            {
                await Navigation.PopToRootAsync();
            }
        }

        private void CopyClicked(object sender, System.EventArgs e)
        {
            if (sender == EscrowPubKeyButton)
            {
                App.Locator.ClipboardService.SendTextToClipboard(ViewModel.PaketId);
            }

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
                else
                {
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
            var result = await StellarHelper.AssignPackage(ViewModel.PaketId, ViewModel.Collateral, location, FinalizePackageEvents);
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

                var result = await StellarHelper.AcceptPackageAsRecipient(BarcodeData.EscrowAddress, ViewModel.PaymentTransaction, location);
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

                    await Navigation.PopToRootAsync();
                }
                else
                {
                    ShowError(result);
                }

                App.ShowLoading(false);
            }
        }

        private void FinalizePackageEvents(object sender, LaunchPackageEventArgs e)
        {
            ProgressBar.AnimationEasing = Easing.SinIn;
            ProgressBar.AnimationLength = 3000;
            ProgressBar.AnimatedProgress = e.Progress;
            ProgressLabel.Text = e.Message;
        }

        #region Events

        private void AddEvents()
        {
            var package = ViewModel;

            if (package.Events == null)
            {
                EventsFrameView.IsVisible = false;
            }
            else
            {
                StackLayout lastStackView = null;

                RelativeLayout relativeLayout = new RelativeLayout();

                for (int i = 0; i < package.Events.Count; i++)
                {
                    int leftTiknesss = 0;
                    var ev = package.Events[i];

                    var stack = new StackLayout();

                    if(i==0)
                    {
                        leftTiknesss = 20;
                        stack.Padding = new Thickness(leftTiknesss, 0, 0, 0);
                    }

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

                        if (package.Events.Count > 1)
                        {
                            relativeLayout.Children.Add(line,
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.X + 5 + leftTiknesss; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Height + 14; }),
                                    Constraint.RelativeToView(stack, (parent, view) => { return view.Width + 15; }),
                                    Constraint.Constant(0.5f));

                            relativeLayout.Children.Add(progressImage,
                                                        Constraint.RelativeToParent((parent) => { return 17; }),
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
                                           return view.X + view.Width + 17;
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
                    EventsFrameView.IsVisible = false;
                }
            }
        }
        #endregion

    }

}
