using UIKit;
using CoreGraphics;


using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;
using CoreAnimation;

[assembly: ExportRenderer(typeof(PaketFrameWithShadow), typeof(PaketFrameWithShadowRender))]
namespace PaketGlobal.iOS
{
    public class PaketFrameWithShadowRender : FrameRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var element = (PaketFrameWithShadow)this.Element;

            if (element != null)
            {
                if (Layer != null)
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
