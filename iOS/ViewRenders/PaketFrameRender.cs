using UIKit;
using CoreGraphics;


using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;

[assembly: ExportRenderer(typeof(MaterialFrame), typeof(MaterialFrameRenderer))]
namespace PaketGlobal.iOS
{
    public class MaterialFrameRenderer : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            if (Layer != null)
            {
                if (Layer.ShadowOpacity != 0.1f)
                {
                    Layer.BorderColor = UIColor.White.CGColor;
                    Layer.CornerRadius = 15;

                    Layer.MasksToBounds = false;
                    Layer.ShadowOffset = new CGSize(-2, 2);
                    Layer.ShadowRadius = 4;
                    Layer.ShadowOpacity = 0.1f;
                }
            }
        }
    }
}
