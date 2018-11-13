using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using Android.App;
using Android.Content;
using static Android.App.SearchManager;

namespace PaketGlobal.Droid
{
    public class OnDismissListener : Java.Lang.Object, IDialogInterfaceOnDismissListener
    {
        private readonly Action action;

        public OnDismissListener(Action action)
        {
            this.action = action;
        }

        public void OnDismiss(IDialogInterface dialog)
        {
            this.action();
        }
    }

    public class NotificationService : INotificationService
	{
        private BannerView mBanner;
        private Action<string> callback;
        private bool isDialogShow = false;

		public NotificationService()
		{

		}

        public void ShowErrorMessage(string text, bool lengthLong = false, EventHandler eventHandler = null, string nextButton = null, string cancelButton = null)
        {
            if(text.Length>0 && !isDialogShow)
            {
                isDialogShow = true;

                Android.App.AlertDialog.Builder dialog = new AlertDialog.Builder(Xamarin.Forms.Forms.Context);
                dialog.SetOnDismissListener(new OnDismissListener(() =>
                {
                    isDialogShow = false;
                }));

                string btn = "OK";

                if (cancelButton!=null)
                {
                    btn = cancelButton;
                }

                AlertDialog alert = dialog.Create();
                alert.SetTitle("DeliverIt");
                alert.SetMessage(text);
                alert.SetButton(btn, (c, ev) => {
                    isDialogShow = false;

                    if (eventHandler != null)
                    {
                        eventHandler.Invoke(this, null);
                    }
                });

                if(nextButton != null)
                {
                    alert.SetButton2(nextButton, (c, ev) => {
                        isDialogShow = false;

                        if (eventHandler != null)
                        {
                            eventHandler.Invoke(this, EventArgs.Empty);
                        }
                    });
                }

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

        public void ShowPackageStringNotification(string title, string body, Action<string> action)
        {
            var context = MainActivity.Instance;

            if (mBanner == null)
            {
                mBanner = new BannerView(context);
                mBanner.BannerFinished += HandleBannerFinished;
            }

            callback = action;

            mBanner.ShowStringPackage(title, body);
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
                    if(package==null)
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
}
