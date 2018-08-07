using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using Android.App;

namespace PaketGlobal.Droid
{
	public class NotificationService : INotificationService
	{
        private BannerView mBanner;
        private Action<string> callback;

		public NotificationService()
		{

		}

        public void ShowErrorMessage(string text, bool lengthLong = false, EventHandler eventHandler = null)
        {
            if(text.Length>0)
            {
                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);
                AlertDialog alert = dialog.Create();
                alert.SetTitle("Paket Global");
                alert.SetMessage(text);
                alert.SetButton("OK", (c, ev) => {
                    if (eventHandler != null)
                    {
                        eventHandler.Invoke(this, null);
                    }
                });
                alert.Show(); 
            }
        }

		public void ShowMessage(string text, bool lengthLong = false)
		{
			var activity = (Xamarin.Forms.Platform.Android.FormsAppCompatActivity)MainActivity.Instance;
			activity.RunOnUiThread(() => {
				using (var toast = Android.Widget.Toast.MakeText(activity, text, lengthLong ? Android.Widget.ToastLength.Long : Android.Widget.ToastLength.Short)) {
					toast.Show();
				}
			});
		}


        public void ShowWalletNotification(string title, string subTitle, Action<string> action)
        {
            if (mBanner == null)
            {
                mBanner = new BannerView(Xamarin.Forms.Forms.Context);
                mBanner.BannerFinished += HandleBannerFinished;
            }

            callback = action;

            mBanner.ShowWallet(title, subTitle);
        }

        public void ShowPackageNotification(Package package, Action<string> action)
        {
            if(mBanner==null)
            {
                mBanner = new BannerView(Xamarin.Forms.Forms.Context);
                mBanner.BannerFinished += HandleBannerFinished;
            }

            callback = action;

            var list = new List<Package>();
            list.Add(package);
            mBanner.ShowPackage(list);
        }

        protected virtual void HandleBannerFinished(bool canceled, Package package)
        {
            if (!canceled)
            {
                if(package == null)
                {
                    callback(null);
                }
                else{
                    callback(package.PaketId);         
                }
            }
        }
    }
}
