using UIKit;
using Foundation;

using CountlySDK;
using GalaSoft.MvvmLight.Ioc;


namespace PaketGlobal.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication uiApplication, NSDictionary launchOptions)
		{
			ZXing.Net.Mobile.Forms.iOS.Platform.Init();

			RegisterServiceContainers();
			
            global::Xamarin.Forms.Forms.Init();

            XamEffects.iOS.Effects.Init(); //write here

			//Countly initialization
			var config = new CountlyConfig() {
				AppKey = Config.CountlyAppKey,
				Host = Config.CountlyServerURL,
				//EnableDebug = true,
				Features = new NSObject[] { Constants.CLYCrashReporting }
			};
			Countly.SharedInstance().StartWithConfig(config);
			Countly.SharedInstance().BeginSession();

			LoadApplication(new App());

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
		}
	}
}
