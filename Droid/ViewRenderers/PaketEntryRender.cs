using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics.Drawables;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;

using PaketGlobal;
using PaketGlobal.Droid;

[assembly: ExportRenderer(typeof(PaketEntry), typeof(PaketEntryRenderer))]
namespace PaketGlobal.Droid
{
    public class PaketEntryRenderer : EntryRenderer
    {
        PaketEntry element;
        public PaketEntryRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || e.NewElement == null)
                return;

            element = (PaketEntry)this.Element;

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
            editText.SetTextSize(Android.Util.ComplexUnitType.Dip,14);
                    
            Control.Background.SetColorFilter(element.LineColor.ToAndroid(), PorterDuff.Mode.SrcIn);  

            //Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayout);           
            //Control.Background.SetColorFilter(Android.Graphics.Color.Transparent, Android.Graphics.PorterDuff.Mode.SrcOver);
        }

        private BitmapDrawable GetDrawable(string imageEntryImage)
        {
            //  var resID = (int)typeof(Resource.Drawable).GetField(imageEntryImage).GetValue(null);

            if (imageEntryImage.Contains(".png"))
            {
                imageEntryImage = imageEntryImage.Replace(".png", "");
            }

            int resID = Resources.GetIdentifier(imageEntryImage, "drawable", this.Context.PackageName);

            return (BitmapDrawable)ContextCompat.GetDrawable(this.Context, resID);


            //var drawable = ContextCompat.GetDrawable(this.Context, resID);
            //var bitmap = ((BitmapDrawable)drawable).Bitmap;

            //return new BitmapDrawable(Resources, Bitmap.CreateScaledBitmap(bitmap, element.ImageWidth * 3, element.ImageHeight * 3, true));
        }

    }
}