using System;

using UIKit;

using PaketGlobal;
using PaketGlobal.iOS;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(BottomBarPage), typeof(BottomBarPageRender))]
namespace PaketGlobal.iOS
{
    public class BottomBarPageRender : TabbedRenderer
    {
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);

            TabBar.Translucent = false;
            TabBar.BarTintColor = UIColor.White;
        }
    }
}
