using UIKit;
using ObjCRuntime;
using Foundation;
using CoreAnimation;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;

[assembly: ExportRenderer(typeof(EntryWithoutBorders), typeof(EntryWithoutBordersRenderer))]
namespace PaketGlobal.iOS
{
   public class EntryWithoutBordersRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.Entry> e)
        {
            base.OnElementChanged(e);

            if (Control == null) 
                return;

            // Need to connect to Sizechanged event because first render time, Entry has no size (-1).
            if (e.NewElement != null){
                e.NewElement.SizeChanged += (obj, args) =>
                {
                    // get native control (UITextField)
                    var entry = Control;

                    // Create borders (bottom only)
                    var border = new CALayer();

                    border.BorderColor = UIColor.LightGray.ColorWithAlpha(0.5f).CGColor;
                    border.Frame = new CGRect(x: 0, y: 39, width: 1024, height: 0.5f);
                    border.BorderWidth = 0.5f;

                    entry.Layer.AddSublayer(border);
                    entry.Layer.MasksToBounds = true;
                    entry.BackgroundColor = UIColor.Clear;

                    entry.BorderStyle = UITextBorderStyle.None;
                    entry.ClipsToBounds = true;
                };
            }
        }
    }
}