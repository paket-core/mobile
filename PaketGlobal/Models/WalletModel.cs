using System;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class WalletModel : BaseViewModel
	{
        private System.Timers.Timer timer;
        private bool isneedTimer = false;

		private BalanceData balance;
		private PriceData price;

        public WalletModel()
        {
            SubscribeToNotifications();
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
            StopTimer();

			var bal = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (bal != null) {
                
				Balance = bal;

                if (timer == null)
                {
                    isneedTimer = true;

                    CreateTimer();
                }
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

				if (bal.Account.BalanceBUL != Balance.Account.BalanceBUL) {
					if (enabled) {
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

	}
}
