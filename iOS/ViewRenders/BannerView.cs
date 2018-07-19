using System;
using CoreGraphics;
using Foundation;
using ObjCRuntime;
using UIKit;

namespace PaketGlobal.iOS.ViewRenders
{
    public partial class BannerView : UIView
    {
        public static readonly UINib Nib;

        private Action _callback;

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

        public void SetPackage(Package package, Action callback)
        {
            _callback = callback;
        }

        partial void OnClickBanner(UIButton sender)
        {
            _callback();
        }
    }
}
