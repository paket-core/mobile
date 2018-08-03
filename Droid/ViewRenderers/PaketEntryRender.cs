using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics.Drawables;
using Android.Content;
using Android.Graphics;
using Android.Support.V4.Content;
using Android.Widget;
using Android.Support.V7.Widget;
using Android.Views;

using PaketGlobal;
using PaketGlobal.Droid;

using System.ComponentModel;

[assembly: ExportRenderer(typeof(PaketEntry), typeof(PaketEntryRenderer))]
namespace PaketGlobal.Droid
{
    public class PaketEntryRenderer : EntryRenderer
    {
        PaketEntry element;
        public PaketEntryRenderer(Context context) : base(context)
        {

        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if(e.PropertyName=="PaddingRight")
            {
                if(element!=null)
                {
                    Control.SetPadding(0, Control.PaddingTop, element.PaddingRight, Control.PaddingBottom);
                }
            }
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

            if(element.CapSentences==1)
            {
				editText.SetRawInputType(Android.Text.InputTypes.ClassText | Android.Text.InputTypes.TextFlagCapWords);
            }
            editText.CompoundDrawablePadding = 25;


            Typeface tf = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Medium.ttf");
            editText.SetTypeface(tf, TypefaceStyle.Normal);
            editText.SetTextSize(Android.Util.ComplexUnitType.Dip,14);

            if (element.BackgroundV==0)
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayout);
            }
            else if (element.BackgroundV == 3)
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayoutTransparent);
            }
            else if (element.BackgroundV == 1)
            {
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayoutGray);
            }
            else{
                Control.Background = ContextCompat.GetDrawable(Context, Resource.Layout.EntryLayoutLightGray);
            }  
        }

        private BitmapDrawable GetDrawable(string imageEntryImage)
        {
            if (imageEntryImage.Contains(".png"))
            {
                imageEntryImage = imageEntryImage.Replace(".png", "");
            }

            int resID = Resources.GetIdentifier(imageEntryImage, "drawable", this.Context.PackageName);

            return (BitmapDrawable)ContextCompat.GetDrawable(this.Context, resID);
        }

    }
}