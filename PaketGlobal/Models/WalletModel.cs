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
            MessagingCenter.Subscribe<string, string>("MyApp", "OnStartApp", (sender, arg) =>
            {
                StartTimer();
            });

            MessagingCenter.Subscribe<string, string>("MyApp", "OnStopApp", (sender, arg) =>
            {
                StopTimer();
            });

            MessagingCenter.Subscribe<Workspace, bool>(this, "Logout", (sender, arg) =>
            {
                StopTimer();
            });
        }

        private void StartTimer()
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

        private void StopTimer()
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

			var bal = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);
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
            var bal = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);

            bool enabled = App.Locator.AccountService.ShowNotifications;

            if (bal != null)
            {
                if(Balance==null)
                {
                    Balance = bal; 
                }

                if(bal.BalanceBUL != Balance.BalanceBUL || bal.BalanceXLM != Balance.BalanceXLM)
                {
                    if (enabled)
                    {
#if __IOS__
                                   // Device.BeginInvokeOnMainThread(() => {
                                        //App.Locator.NotificationService.ShowNotification(p1);
                                        //App.Locator.NotificationService.ShowMessage(String.Format("Your package in {0}", p1.FormattedStatus), false);
                                    //});
#else
                        App.Locator.NotificationService.ShowMessage("Your balance has been changed", false);
#endif
                    } 
                }

                Balance = bal;
            }

            if (isneedTimer)
            {
                timer.Start();
            }
        }

        public override void Reset()
        {
            base.Reset();

            if (timer != null)
            {
                StopTimer();

                MessagingCenter.Unsubscribe<string, string>("MyApp", "OnStopApp");
                MessagingCenter.Unsubscribe<string, string>("MyApp", "OnStartApp");
                MessagingCenter.Unsubscribe<Workspace, bool>(this, "Logout");
            }
        }
	}
}
