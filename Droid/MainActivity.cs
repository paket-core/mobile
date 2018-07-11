using Android.OS;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Acr.UserDialogs;
using LY.Count.Android.Sdk;
using GalaSoft.MvvmLight.Ioc;
using Android.Graphics;
using Android.Widget;
using Android.Views;
using System;
using RoundedBoxView.Forms.Plugin.Droid;

namespace PaketGlobal.Droid
{
	[Activity(Label = "PaketGlobal.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		internal static MainActivity Instance { get; private set; }

		protected override void OnCreate(Bundle bundle)
		{
			base.OnCreate(bundle);

            XamEffects.Droid.Effects.Init();
            XFGloss.Droid.Library.Init(this, bundle);
            ZXing.Net.Mobile.Forms.Android.Platform.Init();

            Window.SetSoftInputMode(Android.Views.SoftInput.AdjustResize);
            AndroidBug5497WorkaroundForXamarinAndroid.assistActivity(this);

			Countly.SharedInstance().Init(this, Config.CountlyServerURL, Config.CountlyAppKey).EnableCrashReporting();
            //Countly.SharedInstance().SetLoggingEnabled(true);

            Instance = this;


			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			RegisterServiceContainers();

			global::Xamarin.Forms.Forms.Init(this, bundle);

            RoundedBoxViewRenderer.Init();

            LoadApplication(new App());

			UserDialogs.Init(this);
		}

		protected override void OnStart()
		{
			base.OnStart();

			Countly.SharedInstance().OnStart(this);
		}

		protected override void OnStop()
		{
			Countly.SharedInstance().OnStop();

			base.OnStop();
		}

		protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
		{
			base.OnActivityResult(requestCode, resultCode, data);
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
		{
			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			global::ZXing.Net.Mobile.Forms.Android.PermissionsHandler.OnRequestPermissionsResult(requestCode, permissions, grantResults);
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
		}
	}

    public class AndroidBug5497WorkaroundForXamarinAndroid     {

        // For more information, see https://code.google.com/p/android/issues/detail?id=5497
        // To use this class, simply invoke assistActivity() on an Activity that already has its content view set.

        // CREDIT TO Joseph Johnson (http://stackoverflow.com/users/341631/joseph-johnson) for publishing the original Android solution on stackoverflow.com

        public static void assistActivity(Activity activity)         {             new AndroidBug5497WorkaroundForXamarinAndroid(activity);         }          private Android.Views.View mChildOfContent;         private int usableHeightPrevious;         private FrameLayout.LayoutParams frameLayoutParams;          private AndroidBug5497WorkaroundForXamarinAndroid(Activity activity)         {             FrameLayout content = (FrameLayout)activity.FindViewById(Android.Resource.Id.Content);             mChildOfContent = content.GetChildAt(0);             ViewTreeObserver vto = mChildOfContent.ViewTreeObserver;             vto.GlobalLayout += (object sender, EventArgs e) => {                 possiblyResizeChildOfContent();             };             frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent.LayoutParameters;         }          private void possiblyResizeChildOfContent()         {             int usableHeightNow = computeUsableHeight();             if (usableHeightNow != usableHeightPrevious)             {                 int usableHeightSansKeyboard = mChildOfContent.RootView.Height;                 int heightDifference = usableHeightSansKeyboard - usableHeightNow;                  frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference;                  mChildOfContent.RequestLayout();                 usableHeightPrevious = usableHeightNow;             }         }          private int computeUsableHeight()         {             Rect r = new Rect();             mChildOfContent.GetWindowVisibleDisplayFrame(r);             if (Build.VERSION.SdkInt < BuildVersionCodes.Lollipop)             {                 return (r.Bottom - r.Top);             }             return r.Bottom;         }      } 
}
