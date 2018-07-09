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

            Control.ProgressTintColor = Color.FromRgb(182, 231, 233).ToUIColor();
            Control.TrackTintColor = Color.FromRgb(188, 203, 219).ToUIColor();
        }
    }
}