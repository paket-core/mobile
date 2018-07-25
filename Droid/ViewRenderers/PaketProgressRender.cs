using System;

using PaketGlobal;
using PaketGlobal.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Content;

[assembly: ExportRenderer(typeof(PaketProgress), typeof(PaketProgressRenderer))]
namespace PaketGlobal.Droid
{
    public class PaketProgressRenderer : ProgressBarRenderer
    {
        public PaketProgressRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ProgressBar> e)
        {
            base.OnElementChanged(e);

            Control.ProgressDrawable.SetColorFilter(Color.FromRgb(182, 231, 233).ToAndroid(), Android.Graphics.PorterDuff.Mode.SrcIn);
            //Control.ProgressTintListColor.FromRgb(182, 231, 233).ToAndroid();
            Control.ProgressTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(182, 231, 233).ToAndroid());
        }
    }
}