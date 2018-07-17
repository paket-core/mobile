﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PackageDetailsPage : BasePage
    {
        private Package ViewModel { get { return BindingContext as Package; } }

        private Command BarcodeTapCommand;

        private bool CanAcceptPackage = false;
        private BarcodePackageData BarcodeData;

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
            });

            XamEffects.Commands.SetTap(BarcodeView, BarcodeTapCommand);

            App.Locator.DeviceService.setStausBarLight();

            if(CanAcceptPackage)
            {
                AcceptButton.IsVisible = true;
                PaymentInfoViewFrame.IsVisible = false;
                EventsInfoViewFrame.IsVisible = false;
            }
            else{
                AddEvents();
            }
        }

        private async void OnBack(object sender, System.EventArgs e)
        {
            if(CanAcceptPackage)
            {
                MessagingCenter.Send<PackageDetailsPage, bool>(this, "AcceptPackage", true);  
            }
            else{
                await Navigation.PopAsync();
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

                //  lblStatus.Text = "Closed";
                ShowMessage("Refunding successfull");
            }
            else
            {
                ShowMessage("Error during refunding");
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

                //  lblStatus.Text = "Closed";
                ShowMessage("Reclaiming successfull");
            }
            else
            {
                ShowMessage("Error during reclaiming");
            }

            App.ShowLoading(false);
        }

        private async void AcceptClicked(object sender, System.EventArgs e)
        {
            var myPubkey = App.Locator.Profile.Pubkey;
            if (myPubkey == ViewModel.RecipientPubkey)
            {
                //I'm a recipient
                App.ShowLoading(true);

                var result = await StellarHelper.AcceptPackageAsRecipient(BarcodeData.EscrowAddress, ViewModel.PaymentTransaction);
                if (result == StellarOperationResult.Success)
                {
                    await System.Threading.Tasks.Task.Delay(2000);

                    await App.Locator.Packages.Load();

                    ShowMessage("Package accepted successfully");

                    MessagingCenter.Send<PackageDetailsPage, bool>(this, "AcceptPackage", true);
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
                App.ShowLoading(true);

                var result = await StellarHelper.AcceptPackageAsCourier(BarcodeData.EscrowAddress, ViewModel.Collateral, ViewModel.PaymentTransaction);
                if (result == StellarOperationResult.Success)
                {
                    await System.Threading.Tasks.Task.Delay(2000);

                    await App.Locator.Packages.Load();

                    ShowMessage("Package accepted successfully");

                    MessagingCenter.Send<PackageDetailsPage, bool>(this, "AcceptPackage", true);
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
                    Text = String.Format("{0:MM.dd.yyyy}", ev.TimestampDT),
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
  
                if(lastStackView==null)
                {
                    relativeLayout.Children.Add(stack,
                                        Constraint.RelativeToParent((parent) => { return 0; }));

                    relativeLayout.Children.Add(line,
                            Constraint.RelativeToView(stack, (parent, view) => { return view.X + 5; }),
                            Constraint.RelativeToParent((parent) => { return 85; }),
                            Constraint.RelativeToView(stack, (parent, view) => { return view.Width + 15; }),
                            Constraint.Constant(0.5f));


                    relativeLayout.Children.Add(progressImage,
                                      Constraint.RelativeToParent((parent) => { return 0; }),
                                      Constraint.RelativeToParent((parent) =>{return 78;}));
                }
                else{
                    relativeLayout.Children.Add(stack,
                                   Constraint.RelativeToView(lastStackView, (parent, view) =>{
                                        return view.X + view.Width + 20; 
                                   }));

                    if(i != (package.Events.Count - 1))
                    {
                        relativeLayout.Children.Add(line,
                                Constraint.RelativeToView(stack, (parent, view) => { return view.X + 5; }),
                                Constraint.RelativeToParent((parent) => { return 85; }),
                                Constraint.RelativeToView(stack, (parent, view) => { return view.Width + 15; }),
                                Constraint.Constant(0.5f)); 
                    }

             


                    relativeLayout.Children.Add(progressImage,
                                   Constraint.RelativeToView(lastStackView, (parent, view) => {
                                       return view.X + view.Width + 20;
                                   }),
                                   Constraint.RelativeToParent((parent) => { return 78; }));
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
