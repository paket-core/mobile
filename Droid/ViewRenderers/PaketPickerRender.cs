using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics.Drawables;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

using PaketGlobal;
using PaketGlobal.Droid;

[assembly: ExportRenderer(typeof(PaketPicker), typeof(PaketPickerRenderer))]
namespace PaketGlobal.Droid
{
    public class PaketPickerRenderer : PickerRenderer
    {
        PaketPicker element;
       
        public PaketPickerRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<Picker> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            element = (PaketPicker)this.Element;

            Control.SetPadding(0, Control.PaddingTop, Control.PaddingRight, Control.PaddingBottom);

            var editText = this.Control;
            if (!string.IsNullOrEmpty(element.Image))
            {
                switch (element.ImageAlignment)
                {
                    case ImageAlignment.Left:
                        editText.SetCompoundDrawablesWithIntrinsicBounds(GetDrawable(element.Image), null, null, null);
                        break;
                    case ImageAlignment.Right:
                        editText.SetCompoundDrawablesWithIntrinsicBounds(null, null, GetDrawable(element.Image), null);
                        break;
                }
            }
            editText.CompoundDrawablePadding = 25;

            Typeface tf = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Medium.ttf");
            editText.SetTypeface(tf, TypefaceStyle.Normal);
            editText.SetTextSize(Android.Util.ComplexUnitType.Dip, 14);

            if (element.BackgroundV == 0)
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayout);
            }
            else if (element.BackgroundV == 3)
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayoutTransparent);
            }
            else
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayoutGray);
            }

          //  Control.Background.SetColorFilter(element.LineColor.ToAndroid(), PorterDuff.Mode.SrcIn);
        }

        private BitmapDrawable GetDrawable(string imageEntryImage)
        {
            if (imageEntryImage.Contains(".png"))
            {
                imageEntryImage = imageEntryImage.Replace(".png", "");
            }

            int resID = Resources.GetIdentifier(imageEntryImage, "drawable", this.Context.PackageName);

            return (BitmapDrawable)ContextCompat.GetDrawable(this.Context, resID);

         //   if(imageEntryImage.Contains(".png")){
         //       imageEntryImage = imageEntryImage.Replace(".png", "");
         //   }

         ////   var resID = (int)typeof(Resource.Drawable).GetField(imageEntryImage).GetValue(null);

            //int resID = Resources.GetIdentifier(imageEntryImage, "drawable", this.Context.PackageName);
            //var drawable = ContextCompat.GetDrawable(this.Context, resID);
            //var bitmap = ((BitmapDrawable)drawable).Bitmap;

            //return new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, element.ImageWidth * 2, element.ImageHeight * 2, true));
        }

    }
}