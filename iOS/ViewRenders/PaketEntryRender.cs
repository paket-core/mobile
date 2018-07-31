using UIKit;
using CoreAnimation;
using CoreGraphics;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using PaketGlobal;
using PaketGlobal.iOS;

using System.Drawing;

[assembly: ExportRenderer(typeof(PaketEntry), typeof(PaketEntryRenderer))]
namespace PaketGlobal.iOS
{
    public class PaketEntryRenderer : EntryRenderer
    {
        UIView BottomBorder;

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();

            var textField = this.Control;
            var element = (PaketEntry)this.Element;

            if (BottomBorder == null)
            {
                if (this.Frame.Size.Width > 0)
                {
                    BottomBorder = new UIView
                    {
                        Frame = new CGRect(0.0f, element.HeightRequest - 1, this.Frame.Width, 1.0f),
                        BackgroundColor = element.LineColor.ToUIColor()
                    };

                    textField.AddSubview(BottomBorder);
                }
            }

            BottomBorder.Frame = new CGRect(0.0f, element.HeightRequest - 1, this.Frame.Width, 1.0f);

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            var element = (PaketEntry)this.Element;
            var textField = this.Control;
            if (!string.IsNullOrEmpty(element.Image))
            {
                switch (element.ImageAlignment)
                {
                    case ImageAlignment.Left:
                        textField.LeftViewMode = UITextFieldViewMode.Always;
                        textField.LeftView = GetImageView(element.Image, element.ImageHeight, element.ImageWidth);
                        break;
                    case ImageAlignment.Right:
                        textField.RightViewMode = UITextFieldViewMode.Always;
                        textField.RightView = GetImageView(element.Image, element.ImageHeight, element.ImageWidth);
                        break;
                }
            }

            textField.BorderStyle = UITextBorderStyle.None;
            textField.Layer.MasksToBounds = true;

            textField.Font = UIFont.FromName("Poppins-Medium", 14);
            textField.TextColor = Xamarin.Forms.Color.FromHex("#555555").ToUIColor();
            textField.SpellCheckingType = UITextSpellCheckingType.No;
            textField.AutocorrectionType = UITextAutocorrectionType.No;
            textField.AutocapitalizationType = UITextAutocapitalizationType.Sentences;

            if(element.IsEnabled==false)
            {
                textField.TextColor = Xamarin.Forms.Color.FromHex("#A7A7A7").ToUIColor();
            }

            if(element.CapSentences==1)
            {
                textField.AutocapitalizationType = UITextAutocapitalizationType.Words;
            }

        }

        private UIView GetImageView(string imagePath, int height, int width)
        {
            var uiImageView = new UIImageView(UIImage.FromBundle(imagePath))
            {
                Frame = new RectangleF(0, 0, width, height)
            };
            UIView objLeftView = new UIView(new System.Drawing.Rectangle(0, 0, width + 5, height));
            objLeftView.AddSubview(uiImageView);

            return objLeftView;
        }
    }
}