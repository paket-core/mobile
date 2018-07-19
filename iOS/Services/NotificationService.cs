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

		public NotificationService()
		{
            Toast.GlobalAnimator = new ScaleAnimator();
            Toast.GlobalLayout.MarginBottom = 16f;
            Toast.GlobalAppearance.MessageColor = UIColor.White;
            Toast.GlobalAppearance.TitleFont = UIFont.FromName("Poppins-Regular", 12);
            Toast.GlobalAppearance.Color = UIColor.Black.ColorWithAlpha(0.7f);
		}

		public void ShowMessage(string text, bool lengthLong = false)
		{
            // More configurations
            Toast.MakeToast(text)
                 .SetShowShadow(false) // Default is true
                 .SetPosition(ToastPosition.Bottom) // Default is Bottom
                 .Show();
		}

        public void ShowNotification(Package package)
        {
            var bannerView = BannerView.View();
            bannerView.Frame = new CGRect(10, -140, UIScreen.MainScreen.Bounds.Size.Width - 20, 130);
            bannerView.Alpha = 0;
            bannerView.SetPackage(package, DidClickBanner);

            var application = UIApplication.SharedApplication.Delegate as AppDelegate;

            foreach(UIView view in application.Window.Subviews)
            {
                if(view is BannerView)
                {
                    return;
                }
            }

            application.Window.AddSubview(bannerView);

            UIView.Animate(0.3f, () =>
            {
                bannerView.Alpha = 1;
                bannerView.Frame = new CGRect(10, 40, UIScreen.MainScreen.Bounds.Size.Width - 20, 130);
            });

            CurrentPackageId = package.PaketId;
        }

        private void DidClickBanner()
        {
            Console.WriteLine("Send Back DATA");
        }
	}
}
