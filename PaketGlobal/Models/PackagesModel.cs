﻿using System;
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
        public string CurrentDisplayPackageId = "";

        private List<Package> packagesList = new List<Package>();

        public List<Package> PackagesList
        {
            get { return packagesList; }
            set { SetProperty(ref packagesList, value); }
        }

        public PackagesModel()
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
            if(timer!=null)
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


        public async System.Threading.Tasks.Task Load()
        {
            StopTimer();

            var result = await App.Locator.ServiceClient.MyPackages();
            if (result != null)
            {
                PackagesList = result.Packages;

                if (timer == null)
                {
                    isneedTimer = true;

                    CreateTimer();
                }
            }
        }

        private async System.Threading.Tasks.Task Refresh()
        {
            var result = await App.Locator.ServiceClient.MyPackages();

            if (result.Packages != null)
            {
                var packages = result.Packages;

                bool enabled = App.Locator.AccountService.ShowNotifications;


                foreach (Package p1 in packages)
                {
                    foreach (Package p2 in PackagesList)
                    {
                        if (p1.PaketId == p2.PaketId)
                        {
                            if (p1.Status != p2.Status)
                            {
                                if (p1.PaketId == CurrentDisplayPackageId)
                                {
                                    MessagingCenter.Send(this, "CurrentDisplayPackageChanged", p1);
                                }

                                if (enabled)
                                {
                                    Device.BeginInvokeOnMainThread(() => {
                                        App.Locator.NotificationService.ShowPackageNotification(p1,DidClickNotification);
                                    });
                                }
                            }
                        }
                    }
                }

                if (PackagesList.Count < packages.Count && enabled)
                {
                    //get new package
                    var package = packages[packages.Count-1];
                    package.isNewPackage = true;

                    Device.BeginInvokeOnMainThread(() => {
                        App.Locator.NotificationService.ShowPackageNotification(package,DidClickNotification);
                    });
                }

                PackagesList = packages;
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

        private void DidClickNotification(string obj)
        {
            MessagingCenter.Send<string, string>("MyApp", "DidClickPackageNotification", obj);
        }

    }
}
