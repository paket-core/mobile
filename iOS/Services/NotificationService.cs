using System;

using UIKit;
using Foundation;
using CoreGraphics;

using GlobalToast;
using GlobalToast.Animation;

using PaketGlobal.iOS;
using PaketGlobal.iOS.ViewRenders;

namespace PaketGlobal.iOS
{
	public class NotificationService : INotificationService
	{
        private string CurrentPackageId;
        private Action<string> callback;
        private bool isDialogShow = false;

		public NotificationService()
		{
            Toast.GlobalAnimator = new ScaleAnimator();
            Toast.GlobalLayout.MarginBottom = 16f;
            Toast.GlobalAppearance.MessageColor = UIColor.White;
            Toast.GlobalAppearance.TitleFont = UIFont.FromName("Poppins-Regular", 12);
            Toast.GlobalAppearance.Color = UIColor.Black.ColorWithAlpha(0.7f);
		}

        public void ShowErrorMessage(string text, bool lengthLong = false, EventHandler eventHandler = null, string nextButton = null, string cancelButton = null)
        {
            if(text.Length>0 && !isDialogShow)
            {
                Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
                {
                    isDialogShow = true;

                    AppDelegate app = UIApplication.SharedApplication.Delegate as AppDelegate;

                    //Create Alert
                    var alertController = UIAlertController.Create("DeliverIt", text, UIAlertControllerStyle.Alert);

                    //Add Action
                    if(cancelButton!=null)
                    {
                        alertController.AddAction(UIAlertAction.Create(cancelButton, UIAlertActionStyle.Default, (obj) => isDialogShow = false));
                    }
                    else{
                        alertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, (obj) => isDialogShow = false));
                    }

                    if(nextButton != null)
                    {
                        alertController.AddAction(UIAlertAction.Create(nextButton, UIAlertActionStyle.Default, (action) => {
                            isDialogShow = false;
                            if(eventHandler!=null)
                            {
                                eventHandler.Invoke(this, EventArgs.Empty);
                            }
                        }));
                    }

                    // Present Alert
                    app.Window.RootViewController.PresentViewController(alertController, true, null);
                });
            }
       
        }

        public void ShowMessage(string text, bool lengthLong = false)
		{
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() =>
            {
                Toast.MakeToast(text)
                     .SetShowShadow(false) // Default is true
                     .SetPosition(ToastPosition.Bottom) // Default is Bottom
                     .Show();
            });
		}

        public void ShowWalletNotification(string title, string subTitle, Action<string> action)
        {
            if(IsShow()==false)
            {
                callback = action;

                var application = UIApplication.SharedApplication.Delegate as AppDelegate;

                var bannerView = BannerView.View();
                bannerView.SetWallet(DidClickBanner);
                bannerView.Show();
                application.Window.AddSubview(bannerView);

                CurrentPackageId = null;
            }
        }

        public void ShowPackageStringNotification(string title, string body, Action<string> action)
        {
            if (IsShow() == false)
            {
                callback = action;

                var application = UIApplication.SharedApplication.Delegate as AppDelegate;

                var bannerView = BannerView.View();
                bannerView.SetPackageString(title, DidClickBanner);
                bannerView.Show();
                application.Window.AddSubview(bannerView);

                CurrentPackageId = "";
            }
        }

        public void ShowPackageNotification(Package package, Action<string> action)
        {
            if (IsShow() == false)
            {
                callback = action;

                var application = UIApplication.SharedApplication.Delegate as AppDelegate;

                var bannerView = BannerView.View();
                bannerView.SetPackage(package, DidClickBanner);
                bannerView.Show();
                application.Window.AddSubview(bannerView);

                CurrentPackageId = package.PaketId;
            }
        }

        private void DidClickBanner()
        {
            callback(CurrentPackageId);
        }


        private bool IsShow()
        {
            var application = UIApplication.SharedApplication.Delegate as AppDelegate;

            foreach (UIView view in application.Window.Subviews)
            {
                if (view is BannerView)
                {
                    return true;
                }
            }

            return false;
        }
	}
}
