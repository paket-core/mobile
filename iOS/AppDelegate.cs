using UIKit;
using Foundation;
using ObjCRuntime;

using CountlySDK;
using GalaSoft.MvvmLight.Ioc;

using RoundedBoxView.Forms.Plugin.iOSUnified;

using ProgressRingControl.Forms.Plugin.iOS;

using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace PaketGlobal.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
        {
            ZXing.Net.Mobile.Forms.iOS.Platform.Init();

            XFGloss.iOS.Library.Init();
            XamEffects.iOS.Effects.Init();
            RoundedBoxViewRenderer.Init();
            ProgressRingRenderer.Init();
            Xamarin.FormsMaps.Init();
            Xamarin.FormsGoogleMaps.Init("AIzaSyDnC69vNlEd0nv9-nnI5NDFY2zj6WChPOw");

            global::Xamarin.Forms.Forms.Init();

            RegisterServiceContainers();

            //Countly initialization
            var config = new CountlyConfig()
            {
                AppKey = Config.CountlyAppKey,
                Host = Config.CountlyServerURL,
                //EnableDebug = true,
                Features = new NSObject[] { CountlySDK.Constants.CLYCrashReporting }
            };
            Countly.SharedInstance().StartWithConfig(config);
            Countly.SharedInstance().BeginSession();

            LoadApplication(new App());

            uiApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, false);

            uiApplication.SetMinimumBackgroundFetchInterval(3600);

            if (UIDevice.CurrentDevice.CheckSystemVersion(8, 0))
            {
                var notificationSettings = UIUserNotificationSettings.GetSettingsForTypes(
                    UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound, null
                );

                uiApplication.RegisterUserNotificationSettings(notificationSettings);
            }

            return base.FinishedLaunching(uiApplication, launchOptions);
        }

        private void RegisterServiceContainers()
        {
            if (!SimpleIoc.Default.IsRegistered<INotificationService>())
            {
                SimpleIoc.Default.Register<INotificationService>(() => {
                    return new NotificationService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAppInfoService>())
            {
                SimpleIoc.Default.Register<IAppInfoService>(() => {
                    return new AppInfoService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IAccountService>())
            {
                SimpleIoc.Default.Register<IAccountService>(() => {
                    return new AccountService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IClipboardService>())
            {
                SimpleIoc.Default.Register<IClipboardService>(() => {
                    return new ClipboardService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IDeviceService>())
            {
                SimpleIoc.Default.Register<IDeviceService>(() => {
                    return new DeviceService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<ILocationSharedService>())
            {
                SimpleIoc.Default.Register<ILocationSharedService>(() => {
                    return new LocationSharedService();
                });
            }

            if (!SimpleIoc.Default.IsRegistered<IEventSharedService>())
            {
                SimpleIoc.Default.Register<IEventSharedService>(() => {
                    return new EventSharedService();
                });
            }
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            var urlComponents = new NSUrlComponents(url, false);

            var queryItems = urlComponents.QueryItems;

            foreach (NSUrlQueryItem item in queryItems)
            {
                if (item.Name == "id")
                {
                    var package = item.Value;

                    if (package != null && package != "")
                    {
                        Xamarin.Forms.MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.APP_LAUNCHED_FROM_DEEP_LINK, package);
                    }
                }
            }

            return true;
        }


        [Export("application:performFetchWithCompletionHandler:")]
        public override async void PerformFetch(UIApplication application, Action<UIBackgroundFetchResult> completionHandler)
        {
            if (application.ApplicationState != UIApplicationState.Inactive)
            {
                if (App.Locator.Profile.Activated)
                {
                    await App.Locator.RouteServiceClient.AddEvent(Constants.EVENT_APP_USED);
                }
            }
            else if (application.ApplicationState != UIApplicationState.Active)
            {
                if (App.Locator.Profile.Activated)
                {
                    var savedJson = NSUserDefaults.StandardUserDefaults.StringForKey("packages");

                    List<Package> savedPackages = new List<Package>();

                    if (savedJson != null)
                    {
                        savedPackages = JsonConvert.DeserializeObject<List<Package>>(savedJson);
                    }

                    var result = await App.Locator.RouteServiceClient.MyPackages();

                    if (result != null && result.Packages != null)
                    {
                        var packages = result.Packages;

                        var json = JsonConvert.SerializeObject(packages);
                        NSUserDefaults.StandardUserDefaults.SetString(json, "packages");
                        NSUserDefaults.StandardUserDefaults.Synchronize();

                        bool enabled = App.Locator.AccountService.ShowNotifications;

                        if (enabled)
                        {
                            foreach (Package p1 in packages)
                            {
                                foreach (Package p2 in savedPackages)
                                {
                                    if (p1.PaketId == p2.PaketId)
                                    {
                                        var isExpiredNeedShow = App.Locator.Packages.IsPackageExpiredNeedShow(p1);

                                        if ((p1.Status != p2.Status) || (p2.CourierPubkey == null && p1.CourierPubkey != null) || isExpiredNeedShow)
                                        {
                                            if ((p1.Status != p2.Status))
                                            {
                                                var text = "Your package " + p1.ShortEscrow + " " + p1.FormattedStatus;
                                                PublishLocalNotification(text, "Please check your Packages archive for more details",application);
                                            }
                                            else if (isExpiredNeedShow)
                                            {
                                                var text = "Your package " + p1.ShortEscrow + " expired";
                                                PublishLocalNotification(text, "Please check your Packages archive for more details",application);
                                            }
                                            else
                                            {
                                                var text = "Your package " + p1.ShortEscrow + " assigned";
                                                PublishLocalNotification(text, "Please check your Packages archive for more details",application);
                                            }
                                        }
                                    }
                                }
                            }

                            if ((savedPackages.Count < packages.Count && enabled) && savedJson != null)
                            {
                                PublishLocalNotification("You have new package", "Please check your Packages archive for more details",application);
                            }
                        }
                    }
                }
            }

            completionHandler(UIBackgroundFetchResult.NewData);
        }

        [Export("application:didReceiveLocalNotification:")]
        public override void ReceivedLocalNotification(UIApplication application, UILocalNotification notification)
        {
        }


        private void PublishLocalNotification(string name, string text, UIApplication application)
        {
            // create the notification
            var notification = new UILocalNotification();

            // set the fire date (the date time in which it will fire)
            notification.FireDate = NSDate.FromTimeIntervalSinceNow(1);

            // configure the alert
            notification.AlertAction = name;
            notification.AlertBody = text;

            // modify the badge
            notification.ApplicationIconBadgeNumber =0;

            // set the sound to be the default sound
            notification.SoundName = UILocalNotification.DefaultSoundName;

            // schedule it
            application.ScheduleLocalNotification(notification);
        }

    }
}
