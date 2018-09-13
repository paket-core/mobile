using System;
using PaketGlobal;
using PaketGlobal.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(PaketSlider), typeof(PaketSliderRender))]
namespace PaketGlobal.iOS
{
    class PaketSliderRender : Xamarin.Forms.Platform.iOS.SliderRenderer
    {
        private bool isAssign = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        {
            base.OnElementChanged(e);

            if (!isAssign)
            {
                isAssign = true;

                var slider = Control;
                // Cast your element here
                var element = (PaketGlobal.PaketSlider)Element;

                slider.TouchDown += (sender, args) =>
                {
                    element.TouchDownEvent(this, EventArgs.Empty);
                };
                slider.TouchUpInside += (sender, args) =>
                {
                    element.TouchUpEvent(this, EventArgs.Empty);
                };
                slider.TouchUpOutside += (sender, args) =>
                {
                    element.TouchUpEvent(this, EventArgs.Empty);
                };
            }
        }
    }
}
