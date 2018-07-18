using System;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class PackagesModel : BaseViewModel
	{
        private System.Timers.Timer timer;
        private bool isneedTimer = false;

		private List<Package> packagesList = new List<Package>();

		public List<Package> PackagesList {
			get { return packagesList; }
			set { SetProperty(ref packagesList, value); }
		}

		public PackagesModel()
		{
            MessagingCenter.Subscribe<string, string>("MyApp", "OnStartApp", (sender, arg) =>
            {
                if (timer != null)
                {
                    StartTimer();
                }
            });

            MessagingCenter.Subscribe<string, string>("MyApp", "OnStopApp", (sender, arg) =>
            {
                if (timer != null)
                {
                    StopTimer();
                }
            });

            MessagingCenter.Subscribe<Workspace, bool>(this, "Logout", (sender, arg) => {
                if (timer != null)
                {
                    StopTimer();

                    timer = null;
                }
            });
		}

        private void StartTimer()
        {
            isneedTimer = true;
            timer.Enabled = true;
            timer.Start();
        }

        private void StopTimer()
        {
            isneedTimer = false;
            timer.Close();
            timer.Stop();
            timer.Enabled = false;
        }

        public override void Reset()
        {
            base.Reset();

            if(timer!=null)
            {
                StopTimer();

                timer = null;

                MessagingCenter.Unsubscribe<string, string>("MyApp", "OnStopApp");
                MessagingCenter.Unsubscribe<string, string>("MyApp", "OnStartApp");
            }
        }


		public async System.Threading.Tasks.Task Load()
		{
			var result = await App.Locator.ServiceClient.MyPackages();
			if (result != null) {
				PackagesList = result.Packages;

                if(timer==null){
                    isneedTimer = true;

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
		}

        private async  System.Threading.Tasks.Task Refresh()
        {
            var result = await App.Locator.ServiceClient.MyPackages();

            if(result.Packages!=null)
            {
                var packages = result.Packages;

                foreach (Package p1 in packages)
                {
                    foreach (Package p2 in PackagesList)
                    {
                        if(p1.PaketId==p2.PaketId)
                        {
                            if(p1.Status!=p2.Status)
                            {
                                App.Locator.NotificationService.ShowMessage(String.Format("Your package in {0}",p1.FormattedStatus), false);
                            }
                        }
                    }
                }

                PackagesList = packages;
            }

            if(isneedTimer)
            {
                timer.Start();
            }
        }

	}
}
