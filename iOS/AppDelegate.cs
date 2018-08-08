using UIKit;
using Foundation;

using CountlySDK;
using GalaSoft.MvvmLight.Ioc;

using RoundedBoxView.Forms.Plugin.iOSUnified;

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

            global::Xamarin.Forms.Forms.Init();

			RegisterServiceContainers();
			
			//Countly initialization
			var config = new CountlyConfig() {
				AppKey = Config.CountlyAppKey,
				Host = Config.CountlyServerURL,
				//EnableDebug = true,
                Features = new NSObject[] { CountlySDK.Constants.CLYCrashReporting }
			};
			Countly.SharedInstance().StartWithConfig(config);
			Countly.SharedInstance().BeginSession();

			LoadApplication(new App());

            uiApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque, false);

			return base.FinishedLaunching(uiApplication, launchOptions);
		}

		private void RegisterServiceContainers()
		{
			if (!SimpleIoc.Default.IsRegistered<INotificationService>()) {
				SimpleIoc.Default.Register<INotificationService>(() => {
					return new NotificationService();
				});
			}

			if (!SimpleIoc.Default.IsRegistered<IAppInfoService>()) {
				SimpleIoc.Default.Register<IAppInfoService>(() => {
					return new AppInfoService();
				});
			}

			if (!SimpleIoc.Default.IsRegistered<IAccountService>()) {
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
    }
}
