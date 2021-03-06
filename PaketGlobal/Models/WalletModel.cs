﻿using System;
using System.Globalization;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class WalletModel : BaseViewModel
	{
        private System.Timers.Timer timer;
        private bool isneedTimer = false;

		private BalanceData balance;
		private PriceData price;

        public double XLM_Ratio = 0;
        public double BUL_Ratio = 0;

        public WalletModel()
        {
            SubscribeToNotifications();

            if(App.Current.Properties.ContainsKey(Constants.BULL_RATIO))
            {
                CultureInfo myUSCulture = new CultureInfo("en-US");

                object fromStorageBul;
                object fromStorageXlm;

                Application.Current.Properties.TryGetValue(Constants.BULL_RATIO, out fromStorageBul);
                Application.Current.Properties.TryGetValue(Constants.XLM_RATIO, out fromStorageXlm);

                var bul = Convert.ToDouble(fromStorageBul as string, myUSCulture);
                var xlm = Convert.ToDouble(fromStorageXlm as string, myUSCulture);

                XLM_Ratio = xlm;
                BUL_Ratio = bul;
            }
        }

        private void SubscribeToNotifications()
        {
            MessagingCenter.Subscribe<string, string>(Constants.NOTIFICATION, Constants.START_APP, (sender, arg) =>
            {
                StartTimer();
            });

            MessagingCenter.Subscribe<string, string>(Constants.NOTIFICATION, Constants.STOP_APP, (sender, arg) =>
            {
                StopTimer();
            });

            MessagingCenter.Subscribe<Workspace, bool>(this,Constants.LOGOUT, (sender, arg) =>
            {
                StopTimer();
            });
        }

        public void StartTimer()
        {
            if (timer != null)
            {
                isneedTimer = true;
                timer.Enabled = true;
                timer.Start();
            }
            else{
                CreateTimer();
            }

        }

        public void StopTimer()
        {
            if (timer != null)
            {
                isneedTimer = false;
                timer.Close();
                timer.Stop();
                timer.Enabled = false;
                timer = null;
            }
        }

        private void CreateTimer()
        {
            if(timer==null)
            {
                timer = new System.Timers.Timer();
                //Execute the function every 10 seconds.
                timer.Interval = 10000;
                //Don't start automaticly the timer.
                timer.AutoReset = false;
                //Attach a function to handle.
                timer.Elapsed += async (sender, e) => await Refresh();
                //Start timer.
                timer.Start();
            }
        }

		public BalanceData Balance {
			get { return balance; }
			set { SetProperty(ref balance, value); }
		}

		public PriceData Price {
			get { return price; }
			set { SetProperty(ref price, value); }
		}

		public async System.Threading.Tasks.Task Load()
		{
            try
            {
                StopTimer();

                LoadRatio();

                var bal = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
                if (bal != null)
                {

                    Balance = bal;

                    if (timer == null)
                    {
                        isneedTimer = true;

                        CreateTimer();
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
		}

        private async void SendEvent(long oldBalance, long newBalance, string currency)
        {
            try{
                var kwargs = new WalletKwargs();
                kwargs.old_balance = Convert.ToString(oldBalance);
                kwargs.new_balance = Convert.ToString(newBalance);
                kwargs.currency = currency;

                var json = JsonConvert.SerializeObject(kwargs);

                var result = await App.Locator.RouteServiceClient.AddEvent(Constants.BALANCE_CHANGED, json);

                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private async System.Threading.Tasks.Task Refresh()
		{
			var bal = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);

			bool enabled = App.Locator.AccountService.ShowNotifications;

			if (bal != null) {
				if (Balance == null) {
					Balance = bal;
				}

                if (bal.Account.BalanceXLM != Balance.Account.BalanceXLM)
                {
                    SendEvent(Balance.Account.BalanceBUL, bal.Account.BalanceBUL, "XLM");
                }

                if (bal.Account.BalanceBUL != Balance.Account.BalanceBUL)
                {
                    SendEvent(Balance.Account.BalanceBUL, bal.Account.BalanceBUL, "BUL");

                    if (enabled)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            App.Locator.NotificationService.ShowWalletNotification("Your balance has been changed", "Please check your Wallet page\nfor more details", DidClickNotification);

                        });
                    }
                }

				Balance = bal;
			}

			if (isneedTimer) {
				if (timer != null) {
					timer.Start();
				}
			}
		}

        public override void Reset()
        {
            base.Reset();

            if (timer != null)
            {
                StopTimer();

                MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.STOP_APP);
                MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.START_APP);
                MessagingCenter.Unsubscribe<Workspace, bool>(this,Constants.LOGOUT);
            }
        }

        private void DidClickNotification(string obj)
        {
            MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.CLICK_WALLET_NOTIFICATION, "");
        }


        private async void LoadRatio()
        {
            var xlm = await App.Locator.IdentityServiceClient.GetWalletRatio("XLM");
            var bul = await App.Locator.IdentityServiceClient.GetWalletRatio("BUL");

            if(xlm != null && bul != null)
            {
                CultureInfo myUSCulture = new CultureInfo("en-US");

                Application.Current.Properties[Constants.BULL_RATIO] = xlm.Ratio;
                Application.Current.Properties[Constants.XLM_RATIO] = xlm.Ratio;
                await Application.Current.SavePropertiesAsync();

                XLM_Ratio = Convert.ToDouble(xlm.Ratio, myUSCulture);
                BUL_Ratio = Convert.ToDouble(bul.Ratio, myUSCulture);
            }
        }
	}
}
