using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Timers;
using Xamarin.Forms;

namespace PaketGlobal
{
    public class PackagesModel : BaseViewModel
    {
        private int oldRadius = 0;
        private System.Timers.Timer timer;
        private bool isneedTimer = false;
        public string CurrentDisplayPackageId = "";

        private List<Package> packagesList = new List<Package>();
        private List<AvaiablePackage> availablePackagesList = new List<AvaiablePackage>();

        public List<Package> PackagesList
        {
            get { return packagesList; }
            set { SetProperty(ref packagesList, value); }
        }

        public List<AvaiablePackage> AvailablePackagesList
        {
            get { return availablePackagesList; }
            set { SetProperty(ref availablePackagesList, value); }
        }

        public PackagesModel()
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
            if(timer!=null)
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
			if (timer != null) {
				isneedTimer = false;
				timer.Dispose();
				timer = null;
			}
		}

        public bool IsPackageExpiredNeedShow(Package package)
        {
            var keyExpired = package.PaketId + "_expired";

            if(package.IsExpired)
            {
                if(package.DeadlineDT.AddHours(1) > DateTime.Now.ToLocalTime())
                {
                    if (Application.Current.Properties.ContainsKey(keyExpired))
                    {
                        return false;
                    }
                    else
                    {
                        Application.Current.Properties[keyExpired] = package.PaketId;
                        Application.Current.SavePropertiesAsync();

                        return true;
                    }
                }
            }

            return false;
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


        public async System.Threading.Tasks.Task Load()
        {
            StopTimer();

			var result = await App.Locator.RouteServiceClient.MyPackages();
            if (result != null)
            {
                var sorted = result.Packages.OrderByDescending(h => h.StatusSortValue);

                PackagesList = sorted.ToList();

                if (timer == null)
                {
                    isneedTimer = true;

                    CreateTimer();
                }
            }

            bool enabled = App.Locator.AccountService.ShowNotifications;

            foreach (Package p1 in PackagesList)
            {
                var isExpiredNeedShow = IsPackageExpiredNeedShow(p1);

                if (isExpiredNeedShow)
                {
                    if (enabled)
                    {
                        Device.BeginInvokeOnMainThread(() => {
                            App.Locator.NotificationService.ShowPackageNotification(p1, DidClickNotification);
                        });
                    }
                }
            }

            App.Locator.AccountService.SavePackages(PackagesList);

            CheckLocationUpdate();
        }

        public async System.Threading.Tasks.Task LoadAvailable(int radius, CancellationTokenSource cancellationTokenSource)
        {
            if (oldRadius != radius && radius != 0)
            {
                oldRadius = radius;

            }
            if (radius == 0)
            {
                radius = oldRadius;
            }

            var location = await App.Locator.LocationHelper.GetStringLocation(true);
            var result = await App.Locator.RouteServiceClient.AvailablePackages(location,radius,cancellationTokenSource);
            if (result != null)
            {
                AvailablePackagesList = result.Packages;
            }
        }

        private async System.Threading.Tasks.Task Refresh()
        {
			if (App.Locator.Profile.Activated) {
				var result = await App.Locator.RouteServiceClient.MyPackages();

				if (result != null && result.Packages != null) {
					var packages = result.Packages;

					bool enabled = App.Locator.AccountService.ShowNotifications;

					foreach (Package p1 in packages) {
						foreach (Package p2 in PackagesList) {
							if (p1.PaketId == p2.PaketId) {

                                var isExpiredNeedShow = IsPackageExpiredNeedShow(p1);

                                if ((p1.Status != p2.Status) || (p2.CourierPubkey == null && p1.CourierPubkey != null) || isExpiredNeedShow) {
									
                                    if (p1.PaketId == CurrentDisplayPackageId) {
										MessagingCenter.Send(this, Constants.DISPLAY_PACKAGE_CHANGED, p1);
									}

									if (enabled) {

                                        if((p2.CourierPubkey == null && p1.CourierPubkey != null))
                                        {
                                            p1.isAssigned = true;
                                        }
                        

										Device.BeginInvokeOnMainThread(() => {
											App.Locator.NotificationService.ShowPackageNotification(p1, DidClickNotification);
										});
									}
								}
							}
						}
					}

					if (PackagesList.Count < packages.Count && enabled) {
						//get new package
						var package = packages[packages.Count - 1];
						package.isNewPackage = true;

						Device.BeginInvokeOnMainThread(() => {
							App.Locator.NotificationService.ShowPackageNotification(package, DidClickNotification);
						});
					}

                    if(packages.Count>0)
                    {
                        var sorted = packages.OrderByDescending(h => h.StatusSortValue);

                        PackagesList = sorted.ToList();
                    }
				}

				if (isneedTimer) {
					if (timer != null) {
						timer.Start();
					}
				}

                App.Locator.AccountService.SavePackages(PackagesList);

				CheckLocationUpdate();
			}
        }

        public override void Reset()
        {
            base.Reset();

            if (timer != null)
            {
                StopTimer();

                MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.START_APP);
                MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.STOP_APP);
                MessagingCenter.Unsubscribe<Workspace, bool>(this,Constants.LOGOUT);
            }
        }

        private void DidClickNotification(string obj)
        {
            MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.CLICK_PACKAGE_NOTIFICATION, obj);
        }

        private void CheckLocationUpdate ()
        {
            bool isFound = false;

            var myPubkey = App.Locator.Profile.Pubkey;

            if(PackagesList!=null)
            {
                foreach(Package package in PackagesList)
                {
                    var myRole = myPubkey == package.LauncherPubkey ? PaketRole.Launcher :
                                                     (myPubkey == package.RecipientPubkey ? PaketRole.Recipient : PaketRole.Courier);
                    
                    if(myRole == PaketRole.Courier)
                    {
                        App.Locator.LocationService.StartUpdateLocation();

                        isFound = true;

                        break;
                    }
                }
            }

            if(!isFound)
            {
                App.Locator.LocationService.StopUpdateLocation();
            }
        }

    }
}
