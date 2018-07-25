using UIKit;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;

[assembly: ExportRenderer(typeof(PaketProgress), typeof(PaketProgressRender))]

namespace PaketGlobal.iOS
{

    public class PaketProgressRender : ProgressBarRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.ProgressBar> e)
        {
            base.OnElementChanged(e);

            Control.ProgressTintColor = Color.FromHex("#53C5C7").ToUIColor();
            Control.TrackTintColor = Color.FromHex("#E5E5E5").ToUIColor();
        }
    }
}