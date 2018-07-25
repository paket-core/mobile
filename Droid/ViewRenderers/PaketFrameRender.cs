using System;

using PaketGlobal;
using PaketGlobal.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Content;
using Android.Support.V4.Content;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(PaketFrame), typeof(PaketFrameRender))]
namespace PaketGlobal.Droid
{
    public class PaketFrameRender : FrameRenderer
    {

        public PaketFrameRender(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Frame> e)
        {
            base.OnElementChanged(e);

            var element = (PaketFrame)this.Element;

            if (element.TopCorners>0)
            {
                Background = ContextCompat.GetDrawable(Context, Resource.Layout.PaketFrameLayout);
             
                GradientDrawable gd = (GradientDrawable)Background;
                gd.SetColor(element.BackgroundColor.ToAndroid());
            }
        }
    }
}
