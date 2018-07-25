// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using UIKit;

namespace PaketGlobal.iOS.ViewRenders
{
    [Register("BannerView")]
    partial class BannerView
    {
        [Outlet]
        UIView MainView { get; set; }
        [Outlet]
        UILabel TitleLabel { get; set; }
        [Outlet]
        UIImageView IconView { get; set; }
        [Outlet]
        UILabel SubTitleLabel { get; set; }

        [Action("OnClickBanner:")]
        partial void OnClickBanner(UIButton sender);

        void ReleaseDesignerOutlets()
        {
        }
    }
}
