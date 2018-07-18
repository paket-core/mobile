using System;
using System.Collections.Generic;

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

        private Command PurchaseXLMTapCommand;
        private Command PurchaseBULTapCommand;
        private Command SendBULTapCommand;


        public WalletPage()
        {
            InitializeComponent();

            BindingContext = App.Locator.Wallet;

            App.Locator.DeviceService.setStausBarLight();

#if __ANDROID__
            HeaderView.TranslationY = -20;
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
        }

        private void ShowEntry(StackLayout stackLayout)
        {
            stackLayout.Opacity = 0;
            stackLayout.Scale = 0.8;
            stackLayout.IsVisible = true;
            stackLayout.FadeTo(1, 500, Easing.SinIn);
            stackLayout.ScaleTo(1, 250);

            //System.Threading.Tasks.Task.Delay(2000);

            //MainScrollView.ScrollToAsync(stackLayout, ScrollToPosition.End, true);
        }

        private void HideEntry(StackLayout stackLayout)
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
			//layoutActivity.IsVisible = true;
			//activityIndicator.IsRunning = true;

			await ViewModel.Load();

			//await layoutActivity.FadeTo(0);
			//await contentWallet.FadeTo(1);

			//layoutActivity.IsVisible = false;
		}


        private async void ReloadClicked(object sender, System.EventArgs e)
        {
            App.ShowLoading(true);

            await ViewModel.Load();

            App.ShowLoading(false);
        }


        private void ShowXLMActivityClicked(object sender, EventArgs e)
        {
            Console.WriteLine("ShowXLMActivityClicked");
        }

        private void ShowBULActivityClicked(object sender, EventArgs e)
        {
            Console.WriteLine("ShowBULActivityClicked");
        }

		private async void SendClicked(object sender, EventArgs e)
		{
			if (IsValid()) {
				Unfocus();

				App.ShowLoading(true);

                var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, EntryRecepient.Text, long.Parse(EntryAmount.Text));
				if (trans != null) {
					var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
					var result = await App.Locator.ServiceClient.SubmitTransaction(signed);
					
                    if (result != null) {
						await ViewModel.Load();

						ShowMessage("Funds sent successfully");
					} 
                    else {
						ShowMessage("Error sending funds");
					}
				} else {
					ShowMessage("Error sending funds");
				}

				App.ShowLoading(false);
		    }
		}

        private async void BuyBULClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.BUL)) {
				App.ShowLoading(true);

				var currency = (PaymentCurrency)PickerBULCurrency.SelectedItem;
                var amount = long.Parse(EntryAmountForBUL.Text) * 100;
				var result = await App.Locator.FundServiceClient.PurchaseBULs(amount, currency);

				App.ShowLoading(false);

				if (result != null) {
					//labelAmountForBUL.Text = String.Format("Please send your {0} to this address to purchase your BULs", currency);
					
     //               entryBULAddress.Text = result.PaymentAddress;

					//await ViewHelper.ToggleViews(stackBULResult, stackBULPurchase);
				} 
                else {
					ShowMessage("Error purchasing BULs");
				}
			}
		}

		private async void BuyXLMClicked(object sender, System.EventArgs e)
		{
			if (IsValid(SpendCurrency.XLM)) {
				App.ShowLoading(true);

				var currency = (PaymentCurrency)PickerXLMCurrency.SelectedItem;
                var amount = long.Parse(EntryAmountForXLM.Text) * 100;
				var result = await App.Locator.FundServiceClient.PurchaseXLMs(amount, currency);

				App.ShowLoading(false);

				if (result != null) {
					//labelAmountForXLM.Text = String.Format("Please send your {0} to this address to purchase your XLMs", currency);
					//entryXLMAddress.Text = result.PaymentAddress;
					//await ViewHelper.ToggleViews(stackXLMResult, stackXLMPurchase);
				} 
                else {
					ShowMessage("Error purchasing XLMs");
				}
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
                    EntryRecepient.Focus();
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
					ShowMessage("Please select payment currency");
					return false;
				}
				if (!ValidationHelper.ValidateNumber(EntryAmountForBUL.Text)) {
                    EntryAmountForBUL.Focus();
					return false;
				}
			} else {
				if (PickerXLMCurrency.SelectedItem == null) {
					ShowMessage("Please select payment currency");
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


            Console.WriteLine(xOffset);

            if (xOffset>((max-290)/2))
            {
                Unfocus();

                await  TransactionsBULScrollView.FadeTo(0);
                await  TransactionsBULScrollView.ScaleTo(0.8f);

                TransactionsXLMScrollView.IsVisible = true;
                TransactionsBULScrollView.IsVisible = false;

                await TransactionsXLMScrollView.FadeTo(1);
                await TransactionsXLMScrollView.ScaleTo(1);
            }
            else{
                Unfocus();

                await TransactionsBULScrollView.FadeTo(1);
                await TransactionsBULScrollView.ScaleTo(1);

                TransactionsXLMScrollView.IsVisible = false;
                TransactionsBULScrollView.IsVisible = true;

                await TransactionsXLMScrollView.FadeTo(0);
                await TransactionsXLMScrollView.ScaleTo(0.8f);
            }
        }


	}
}
