using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;
using System.Threading;

namespace PaketGlobal.iOS.ViewRenders
{
    public partial class BannerView : UIView
    {
        public static readonly UINib Nib;

        private Action _callback;
        private UISwipeGestureRecognizer swipeGestureRecognizer;

        static BannerView()
        {
            Nib = UINib.FromName("BannerViewNib", NSBundle.MainBundle);
        }

        static public BannerView View()
        {
            var view = Runtime.GetNSObject(NSBundle.MainBundle.LoadNib("BannerViewNib", null, null).ValueAt(0)) as BannerView;
            return view;
        }

        protected BannerView(IntPtr handle) : base(handle)
        {

        }

        public override void AwakeFromNib()
        {
            base.AwakeFromNib();

            this.Frame = new CGRect(10, -140, UIScreen.MainScreen.Bounds.Size.Width - 20, 130);
            this.Alpha = 0;

            swipeGestureRecognizer = new UISwipeGestureRecognizer(OnSwipeBanner);
            swipeGestureRecognizer.Direction = UISwipeGestureRecognizerDirection.Up;
            this.AddGestureRecognizer(swipeGestureRecognizer);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            MainView.BackgroundColor = UIColor.White.ColorWithAlpha(0.97f);

            MainView.Layer.BorderColor = UIColor.White.CGColor;
            MainView.Layer.CornerRadius = 15;

            MainView.Layer.MasksToBounds = false;
            MainView.Layer.ShadowOffset = new CGSize(-2, 2);
            MainView.Layer.ShadowRadius = 4;
            MainView.Layer.ShadowOpacity = 0.1f;
        }

        public void Show()
        {
            UIView.Animate(0.3f, () =>
            {
                this.Alpha = 1;
                this.Frame = new CGRect(10, 40, UIScreen.MainScreen.Bounds.Size.Width - 20, 130);
            });

            var delayTimer = new Timer((state) =>
                    InvokeOnMainThread(() => Hide()), null, 6 * 1000, Timeout.Infinite);      
        }

        private void Hide()
        {
            UIView.Animate(0.3f, () =>
            {
                this.Alpha = 0;
                this.Frame = new CGRect(10, -140, UIScreen.MainScreen.Bounds.Size.Width - 20, 130);
            }, () => {
                this.RemoveFromSuperview();  
            });
        }

        public void SetPackage(Package package, Action callback)
        {
            _callback = callback;

            if(package.isNewPackage)
            {
                TitleLabel.Text = "You have a new Package";   
            }
            else{
                TitleLabel.Text = "Your Package " + package.FormattedStatus;
            }


            IconView.Image = new UIImage(package.StatusIcon);
        }

        public void SetWallet(Action callback)
        {
            _callback = callback;

            TitleLabel.Text = "Your Balance has been changed";
            SubTitleLabel.Text = "Please check your Wallet for more details";
            IconView.Image = new UIImage("color_wallet.png");
        }

        partial void OnClickBanner(UIButton sender)
        {
            Hide();

            _callback();
        }

        private void OnSwipeBanner()
        {
            Hide();
        }

    }
}
