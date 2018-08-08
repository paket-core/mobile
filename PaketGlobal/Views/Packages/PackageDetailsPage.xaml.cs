using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PackageDetailsPage : BasePage
    {
        private Package ViewModel { get { return BindingContext as Package; } }

        private Command BarcodeTapCommand;

        private bool CanAcceptPackage = false;
        private BarcodePackageData BarcodeData;
        public bool ShouldDismiss = false;

        public PackageDetailsPage(Package package, bool canAcceptPackage = false, BarcodePackageData barcodePackageData = null)
        {
            InitializeComponent();

            BindingContext = package;

            CanAcceptPackage = canAcceptPackage;
            BarcodeData = barcodePackageData;

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

            //TODO: temp
            BottomBarcodeView.IsVisible = false;

            var data = new BarcodePackageData
            {
                EscrowAddress = package.PaketId
            };
            BarcodeImage.BarcodeOptions.Width = 300;
            BarcodeImage.BarcodeOptions.Height = 300;
            BarcodeImage.BarcodeOptions.Margin = 1;
            BarcodeImage.BarcodeValue = JsonConvert.SerializeObject(data);
            BarcodeTapCommand = new Command(() =>
            {
                BarcodeImage.IsVisible = !BarcodeImage.IsVisible;

                if(BarcodeImage.IsVisible)
                {
                    BarcodeArrow.Source = "dropdown_arrow_top.png";
                }
                else{
                    BarcodeArrow.Source = "dropdown_arrow.png";
                }
            });

            XamEffects.Commands.SetTap(BarcodeView, BarcodeTapCommand);

            App.Locator.DeviceService.setStausBarLight();

            if(CanAcceptPackage)
            {
                AcceptButton.IsVisible = true;
                PaymentInfoViewFrame.IsVisible = false;
                EventsInfoViewFrame.IsVisible = false;
                FundInfoViewFrame.VerticalOptions = LayoutOptions.FillAndExpand;
            }
            else{
                AddEvents();

                EventsInfoViewFrame.IsVisible = true;

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
            });

            PullToRefresh.RefreshCommand = refreshCommand;
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
                        CornerRadius = 5,
                        HeightRequest = 16
                    };

                    var eventTypeLabel = new Label()
                    {
                        Text = ev.EventType.ToUpper(),
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
                        Text = String.Format("{0:MM.dd.yyyy HH:mm}", ev.TimestampDT),
                        TextColor = Color.FromHex("#A7A7A7"),
                        FontSize = 10
                    };
                    dateLabel.SetDynamicResource(Label.FontFamilyProperty, "MediumFont");

                    stack.Children.Add(dateLabel);

                    var userLabel = new Label()
                    {
                        Text = ev.PaketUser,
                        TextColor = Color.FromHex("#555555"),
                        FontSize = 12,
                    };
                    userLabel.LineBreakMode = LineBreakMode.CharacterWrap;
                    userLabel.WidthRequest = 140;
                    userLabel.SetDynamicResource(Label.FontFamilyProperty, "MediumFont");

                    stack.Children.Add(userLabel);


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
