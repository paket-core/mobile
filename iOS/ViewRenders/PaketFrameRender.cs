using UIKit;
using CoreGraphics;


using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;
using CoreAnimation;

[assembly: ExportRenderer(typeof(PaketFrame), typeof(PaketFrameRender))]
namespace PaketGlobal.iOS
{
    public class PaketFrameRender : FrameRenderer
    {
        private CAShapeLayer MaskLayer;

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var element = (PaketFrame)this.Element;

            if (Layer != null && element.TopCorners == 0)
            {
                Layer.BorderColor = UIColor.White.CGColor;
                Layer.CornerRadius = 15;

                Layer.MasksToBounds = false;
                Layer.ShadowOffset = new CGSize(-2, 2);
                Layer.ShadowRadius = 4;
                Layer.ShadowOpacity = 0.1f;
            }
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var element = (PaketFrame)this.Element;

            if (element.TopCorners > 0)
            {
                MaskLayer = new CAShapeLayer();
                MaskLayer.Path = UIBezierPath.FromRoundedRect(Bounds, (UIRectCorner.TopLeft | UIRectCorner.TopRight), new CGSize(element.TopCorners, element.TopCorners)).CGPath;
                Layer.Mask = MaskLayer;
            }
        }
    }
}
