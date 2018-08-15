using System;
using System.Collections.Generic;
using Plugin.DeviceInfo;

using Xamarin.Forms;
                 
namespace PaketGlobal
{
    public partial class WalletPage : BasePage
    {
        private WalletModel ViewModel
        {
            get
            {
                return BindingContext as WalletModel;
            }
        }

        private string recipient = "";

        private Command PurchaseXLMTapCommand;
        private Command PurchaseBULTapCommand;
        private Command SendBULTapCommand;

        private string PurchaseBullAddress = null;
        private string PurchaseXlmAddress = null;

        private bool IsAnimationEnabled = true;

        public WalletPage()
        {
            InitializeComponent();

            BindingContext = App.Locator.Wallet;

            App.Locator.DeviceService.setStausBarLight();

#if __ANDROID__
            HeaderView.TranslationY = -20;
            BULFrameView.WidthRequest = (double)App.Locator.DeviceService.ScreenWidth() - 150;
            XLMFrameView.WidthRequest = (double)App.Locator.DeviceService.ScreenWidth() - 150; 

            if(CrossDeviceInfo.Current.Model.ToLower().Contains("htc_m10h"))
            {
                IsAnimationEnabled = false;
            }
#endif

            AddCommands();
        }

        private void AddCommands()
        {
            PurchaseXLMTapCommand = new Command(() =>
            {
                var visible = !PurchaseXLMEntryViews.IsVisible;
                if (visible)
                {
                    ShowEntry(PurchaseXLMEntryViews);
                }
                else
                {
                    HideEntry(PurchaseXLMEntryViews);
                }
            });

            PurchaseBULTapCommand = new Command(() =>
            {
                var visible = !PurchaseBULEntryViews.IsVisible;
                if (visible)
                {
                    ShowEntry(PurchaseBULEntryViews);
                }
                else
                {
                    HideEntry(PurchaseBULEntryViews);
                }
            });

            SendBULTapCommand = new Command(() =>
            {
                var visible = !SendBULEntryViews.IsVisible;
                if (visible)
                {
                    ShowEntry(SendBULEntryViews);
                }
                else
                {
                    HideEntry(SendBULEntryViews);
                }

            });


            XamEffects.Commands.SetTap(PurchaseXLMStackView, PurchaseXLMTapCommand);
            XamEffects.Commands.SetTap(PurchaseBULStackView, PurchaseBULTapCommand);
            XamEffects.Commands.SetTap(SendBULStackView, SendBULTapCommand);


            var refreshCommand = new Command(async () =>
            {
                PullToRefresh.IsRefreshing = true;
                RefreshButton.IsVisible = false;

                await LoadWallet();

                PullToRefresh.IsRefreshing = false;
                RefreshButton.IsVisible = true;
            });

            PullToRefresh.RefreshCommand = refreshCommand;

        }

        private void ShowEntry(StackLayout stackLayout)
        {
            if (IsAnimationEnabled)
            {
                stackLayout.Opacity = 0;
                stackLayout.Scale = 0.8;
                stackLayout.IsVisible = true;
                stackLayout.FadeTo(1, 500, Easing.SinIn);
                stackLayout.ScaleTo(1, 250);
            }
            else{
                stackLayout.IsVisible = true;
            }

            if(stackLayout==PurchaseBULEntryViews)
            {
                var visible = SendBULEntryViews.IsVisible;
                if(visible)
                {
                    HideEntry(SendBULEntryViews);
                }
            }
            else if (stackLayout == SendBULEntryViews)
            {
                var visible = PurchaseBULEntryViews.IsVisible;
                if (visible)
                {
                    HideEntry(PurchaseBULEntryViews);
                }
            }
        }

        private void HideEntry(StackLayout stackLayout)
        {
            if(IsAnimationEnabled)
            {
                Action<double> callback = input => stackLayout.HeightRequest = input;

                double startingHeight = stackLayout.Height;
                double endingHeight = -30;
                uint rate = 16;
                uint length = 250;

                Easing easing = Easing.CubicOut;
#if __ANDROID__
                stackLayout.Opacity = 0;
#else
            stackLayout.FadeTo(0, length, easing);
#endif
                stackLayout.Animate("invis", callback, startingHeight, endingHeight, rate, length, easing, (double arg1, bool arg2) => {
                    stackLayout.IsVisible = false;
                    stackLayout.HeightRequest = startingHeight;
                });   
            }
            else{
                stackLayout.IsVisible = false;
            }
        }


		protected async override void OnAppearing()
		{
            App.Locator.DeviceService.setStausBarLight();

            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                await LoadWallet();
            }
		}

		private async System.Threading.Tasks.Task LoadWallet()
		{
			await ViewModel.Load();

            ActivityIndicatorBUL.IsRunning = false;
            ActivityIndicatorBUL.IsVisible = false;

            ActivityIndicatorXLM.IsRunning = false;
            ActivityIndicatorXLM.IsVisible = false;

            RefreshButton.IsVisible = true;

            TransactionsBULScrollView.IsVisible = true;
            TransactionsLabel.IsVisible = true;

            TopScrollView.IsEnabled = true;
		}


        private async void ReloadClicked(object sender, System.EventArgs e)
        {
            App.ShowLoading(true);

            await ViewModel.Load();

            App.ShowLoading(false);
        }

        private async void AddressButtonClicked(object sender, EventArgs e)
        {
            this.Unfocus();

            AddressBookPage page = new AddressBookPage(false);
            page.eventHandler = DidSelectItemHandler;

            var mainPage = App.Current.MainPage;

            await mainPage.Navigation.PushAsync(page);
        }

        private async void DidSelectItemHandler(object sender, AddressBookPageEventArgs e)
        {
            EntryRecepient.Text = e.Item;
            recipient = await EntryRecepient.CheckValidCallSignOrPubKey();
        }


        private void ShowXLMActivityClicked(object sender, EventArgs e)
        {
        }

        private void ShowBULActivityClicked(object sender, EventArgs e)
        {
        }

        private void DoneSendBULSClicked(object sender, EventArgs e)
        {
            EntryRecepient.Text = "";
            EntryAmount.Text = "";
            EntryRecepient.ToDefaultState();

            SendBULSSuccessView.IsVisible = false;
            SundBULSMainStackView.IsVisible = true;    

            HideEntry(SendBULEntryViews);
        }

        private void DonePurchaseXLMClicked(object sender, EventArgs e)
        {
            IconXLMCurrencyView.IsVisible = false;
            PickerXLMCurrency.SelectedItem = null;
            EntryAmountForXLM.Text = "";

            PurchaseXLMSuccessView.IsVisible = false;
            PurchaseXLMMainView.IsVisible = true;

            if (PurchaseXlmAddress != null)
            {
                App.Locator.ClipboardService.SendTextToClipboard(PurchaseXlmAddress);
                ShowMessage(AppResources.Copied);
            }

            HideEntry(PurchaseXLMEntryViews);
        }

        private void DonePurchaseBULSClicked(object sender, EventArgs e)
        {
            PickerBULCurrency.SelectedItem = null;
            EntryAmountForBUL.Text = "";
            IconBULCurrencyView.IsVisible = false;

            PurchaseBULSSuccessView.IsVisible = false;
            PurchaseBULMainView.IsVisible = true;

            if(PurchaseBullAddress != null)
            {
                App.Locator.ClipboardService.SendTextToClipboard(PurchaseBullAddress);
                ShowMessage(AppResources.Copied);
            }

            HideEntry(PurchaseBULEntryViews);
        }

        private async void SendClicked(object sender, EventArgs e)
		{
            if(EntryRecepient.IsBusy)
            {
                return;
            }
            else if(recipient.Length==0)
            {
                EntryRecepient.FocusField();
            }
			else if (IsValid())
            {
				Unfocus();

				App.ShowLoading(true);

                try{
                    double amount = double.Parse(EntryAmount.Text);

                    var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, recipient, amount);
                    if (trans != null)
                    {
                        var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
                        var result = await App.Locator.ServiceClient.SubmitTransaction(signed);

                        if (result != null)
                        {
                            await ViewModel.Load();

                            SundBULSMainStackView.IsVisible = false;
                            SendBULSSuccessView.IsVisible = true;
                        }
                    } 
                }
                catch (Exception ex)
                {
                    EventHandler handler = (se, ee) => {
                        EntryAmount.Focus();
                    };

                    ShowErrorMessage(ex.Message, false, handler);
                }

				App.ShowLoading(false);
		    }
		}

        private async void BuyBULClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.BUL)) {

                try{
                    App.ShowLoading(true);

                    var currency = (PaymentCurrency)PickerBULCurrency.SelectedItem;

                    double amount = double.Parse(EntryAmountForBUL.Text);

                    var result = await App.Locator.FundServiceClient.PurchaseBULs(amount, currency);

                    if (result != null)
                    {
                        PurchaseBullAddress = result.PaymentAddress;

                        var successString = String.Format("Please send your {0} to the address {1} to purchase your BULs", currency, result.PaymentAddress);
                        PurchaseBULSuccessLabel.Text = successString;

                        PurchaseBULMainView.IsVisible = false;
                        PurchaseBULSSuccessView.IsVisible = true;
                    }
 
                }
                catch(Exception)
                {
                }

                App.ShowLoading(false);
			}
		}

		private async void BuyXLMClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.XLM)) {
                try{
                    App.ShowLoading(true);

                    var currency = (PaymentCurrency)PickerXLMCurrency.SelectedItem;
                    var amount = double.Parse(EntryAmountForXLM.Text);
                    var result = await App.Locator.FundServiceClient.PurchaseXLMs(amount, currency);

                    if (result != null)
                    {
                        PurchaseXlmAddress = result.PaymentAddress;

                        var successString = String.Format("Please send your {0} to the address {1} to purchase your XLMs", currency, result.PaymentAddress);
                        PurchaseXLMSuccessLabel.Text = successString;

                        PurchaseXLMMainView.IsVisible = false;
                        PurchaseXLMSuccessView.IsVisible = true;
                    }
                }
                catch(Exception)
                {
                }

                App.ShowLoading(false);
			}
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
                if (!ValidationHelper.ValidateNumber(EntryAmount.Text))
                {
                    EntryAmount.Focus();
                }
            }
            else if (sender == EntryAmount)
            {
                if (!ValidationHelper.ValidateTextField(EntryRecepient.Text))
                {
                    EntryRecepient.FocusField();
                }
            }
            else if (sender == EntryAmountForBUL)
            {
                if (PickerBULCurrency.SelectedItem==null)
                {
                    PickerBULCurrency.Focus();
                }
            }
            else if (sender == EntryAmountForXLM)
            {
                if (PickerXLMCurrency.SelectedItem == null)
                {
                    PickerXLMCurrency.Focus();
                }
            }
        }

        private void PickerUnfocused(object sender, EventArgs e){
            if(sender==PickerBULCurrency)
            {
                if(PickerBULCurrency.SelectedItem!=null)
                {
                    IconBULCurrencyView.IsVisible = true;

                    var currency = (PaymentCurrency)PickerBULCurrency.SelectedItem;

                    if(currency == PaymentCurrency.BTC)
                    {
                        IconBULCurrencyView.Source = "btc_icon.png"; 
                    }
                    else{
                        IconBULCurrencyView.Source = "eth_icon.png";
                    }
                }
            }
            else if (sender == PickerXLMCurrency)
            {
                if (PickerXLMCurrency.SelectedItem != null)
                {
                    IconXLMCurrencyView.IsVisible = true;

                    var currency = (PaymentCurrency)PickerXLMCurrency.SelectedItem;

                    if (currency == PaymentCurrency.BTC)
                    {
                        IconXLMCurrencyView.Source = "btc_icon.png";
                    }
                    else
                    {
                        IconXLMCurrencyView.Source = "eth_icon.png";
                    }
                }
            }
        }

		protected override bool IsValid()
		{
			if (!ValidationHelper.ValidateTextField(EntryRecepient.Text)) {
                EntryRecepient.Focus();
				return false;
			}
			if (!ValidationHelper.ValidateNumber(EntryAmount.Text)) {
                EntryAmount.Focus();
				return false;
			}

			return true;
		}

		private bool IsValid(SpendCurrency spendCurrency)
		{
			if (spendCurrency == SpendCurrency.BUL) {
				if (PickerBULCurrency.SelectedItem == null) {
                    
                    EventHandler handleCurrencyHandler = (s, e) => {                         PickerBULCurrency.Focus();} ;                      ShowErrorMessage(AppResources.PleaseSelectPaymentCurrency, false, handleCurrencyHandler); 
					return false;
				}
				if (!ValidationHelper.ValidateNumber(EntryAmountForBUL.Text)) {
                    EntryAmountForBUL.Focus();
					return false;
				}
			} else {
				if (PickerXLMCurrency.SelectedItem == null) {
					
                    EventHandler handleCurrencyHandler = (s, e) => {
                        PickerXLMCurrency.Focus();
                    };

                    ShowErrorMessage(AppResources.PleaseSelectPaymentCurrency, false, handleCurrencyHandler);

					return false;
				}
				if (!ValidationHelper.ValidateNumber(EntryAmountForXLM.Text)) {
                    EntryAmountForXLM.Focus();
					return false;
				}
			}

			return true;
		}

        private void TransactionsScrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {

        }

        private async void WalletsScrolled(object sender, Xamarin.Forms.ScrolledEventArgs e)
        {
            var xOffset = e.ScrollX;

            var max = TopScrollView.ContentSize.Width;
            var w = BULFrameView.WidthRequest;

            if (w <= 0)
            {
                w = 290;
            }

            if (xOffset > ((max - w) / 2))
            {
                Unfocus();

                Gradient.Steps = null;

                Gradient.AddStep(Color.FromHex("#39C8C8"), 0);
                Gradient.AddStep(Color.FromHex("#53E1E1"), 1);

                await TransactionsBULScrollView.FadeTo(0);
                await TransactionsBULScrollView.ScaleTo(0.8f);

                TransactionsXLMScrollView.IsVisible = true;
                TransactionsBULScrollView.IsVisible = false;

                await TransactionsXLMScrollView.FadeTo(1);
                await TransactionsXLMScrollView.ScaleTo(1);
            }
            else
            {
                Unfocus();

                Gradient.Steps = null;

                Gradient.AddStep(Color.FromHex("#4D64E8"), 0);
                Gradient.AddStep(Color.FromHex("#6786EF"), 1);


                await TransactionsBULScrollView.FadeTo(1);
                await TransactionsBULScrollView.ScaleTo(1);

                TransactionsXLMScrollView.IsVisible = false;
                TransactionsBULScrollView.IsVisible = true;

                await TransactionsXLMScrollView.FadeTo(0);
                await TransactionsXLMScrollView.ScaleTo(0.8f);
            }
        }


        void Handle_Focused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
       
        }

        void Handle_Unfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
      
        }
	}
}
