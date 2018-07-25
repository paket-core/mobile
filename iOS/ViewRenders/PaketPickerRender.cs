using UIKit;
using CoreAnimation;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;

using System.Drawing;

[assembly: ExportRenderer(typeof(PaketPicker), typeof(PaketPickeryRenderer))]
namespace PaketGlobal.iOS
{
    public class PaketPickeryRenderer : PickerRenderer
    {
        CALayer BottomBorder;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            if(BottomBorder==null){
                if (this.Frame.Size.Width > 0){
                    var textField = this.Control;
                    var element = (PaketPicker)this.Element;

                    BottomBorder = new CALayer
                    {
                        Frame = new CGRect(0.0f, element.HeightRequest - 1, this.Frame.Width, 1.0f),
                        BorderWidth = 1.0f,
                        BorderColor = element.LineColor.ToCGColor()
                    };

                    textField.Layer.AddSublayer(BottomBorder);
                    textField.Layer.MasksToBounds = true;
                }
            }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            var element = (PaketPicker)this.Element;
            var textField = this.Control;
            if (!string.IsNullOrEmpty(element.Image))
            {
                switch (element.ImageAlignment)
                {
                    case ImageAlignment.Left:
                        textField.LeftViewMode = UITextFieldViewMode.Always;
                        textField.LeftView = GetImageView(element.Image, element.ImageWidth, element.ImageHeight);
                        break;
                    case ImageAlignment.Right:
                        textField.RightViewMode = UITextFieldViewMode.Always;
                        textField.RightView = GetImageView(element.Image, element.ImageWidth, element.ImageHeight);
                        break;
                }
            }

            textField.BorderStyle = UITextBorderStyle.None;
            textField.Font = UIFont.FromName("Poppins-Medium", 14);
            textField.TextColor = Xamarin.Forms.Color.FromHex("#555555").ToUIColor();
        }

        private UIView GetImageView(string imagePath, int height, int width)
        {
            var uiImageView = new UIImageView(UIImage.FromBundle(imagePath))
            {
            };
            //UIView objLeftView = new UIView(new System.Drawing.Rectangle(0, 0, uiImageView.Frame.Size.Width + 5, uiImageView.Frame.Size.Height));
            //objLeftView.AddSubview(uiImageView);

            return uiImageView;
        }
    }
}