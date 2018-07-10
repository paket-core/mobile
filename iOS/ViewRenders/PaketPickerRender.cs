﻿using UIKit;
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
                if (this.Frame.Size.Width!=0){
                    var textField = this.Control;
                    var element = (PaketPicker)this.Element;

                    BottomBorder = new CALayer
                    {
                        Frame = new CGRect(0.0f, element.HeightRequest - 1, this.Frame.Width-8, 1.0f),
                        BorderWidth = 2.0f,
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
                        textField.LeftView = GetImageView(element.Image, element.ImageHeight, element.ImageWidth);
                        break;
                    case ImageAlignment.Right:
                        textField.RightViewMode = UITextFieldViewMode.Always;
                        textField.RightView = GetImageView(element.Image, element.ImageHeight, element.ImageWidth);
                        break;
                }
            }

            textField.BorderStyle = UITextBorderStyle.None;
        }

        private UIView GetImageView(string imagePath, int height, int width)
        {
            var uiImageView = new UIImageView(UIImage.FromBundle(imagePath))
            {
                Frame = new RectangleF(0, 0, width, height)
            };
            UIView objLeftView = new UIView(new System.Drawing.Rectangle(0, 0, width + 10, height));
            objLeftView.AddSubview(uiImageView);

            return objLeftView;
        }
    }
}