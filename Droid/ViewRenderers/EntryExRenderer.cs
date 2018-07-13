using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics.Drawables;
using Android.Content;

using PaketGlobal;
using PaketGlobal.Droid;

[assembly: ExportRenderer (typeof (EntryEx), typeof (EntryExRenderer))]
namespace PaketGlobal.Droid
{
	class EntryExRenderer : EntryRenderer
	{
		private Drawable originalBackground;
		private Android.Text.InputTypes inputTypes;

		public EntryExRenderer(Context context) : base(context)
		{

		}

		protected override void OnElementChanged (ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged (e);

			if (originalBackground == null)
				originalBackground = Control.Background;

			if (e.NewElement != null) {
				UpdateTintColor ();
				Control.Enabled = ((EntryEx)e.NewElement).InputEnabled;
				if (Control.InputType != Android.Text.InputTypes.Null) inputTypes = Control.InputType;
				Control.InputType = ((EntryEx)e.NewElement).EditEnabled ? inputTypes : Android.Text.InputTypes.Null;
				Control.SetTextIsSelectable(true);
			}
		}

		protected override void OnElementPropertyChanged (object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			base.OnElementPropertyChanged (sender, e);

			if (e.PropertyName == EntryEx.TintColorProperty.PropertyName) {
				UpdateTintColor();
			} else if (e.PropertyName == EntryEx.InputEnabledProperty.PropertyName) {
				Control.Enabled = ((EntryEx)Element).InputEnabled;
			} else if (e.PropertyName == EntryEx.EditEnabledProperty.PropertyName) {
				if (Control.InputType != Android.Text.InputTypes.Null) inputTypes = Control.InputType;
				Control.InputType = ((EntryEx)Element).EditEnabled ? inputTypes : Android.Text.InputTypes.Null;
				Control.SetTextIsSelectable(true);
			}
		}

		private void UpdateTintColor ()
		{
			if (Control != null) {
				var color = ((EntryEx)Element).TintColor.ToAndroid ();
				Control.SetHintTextColor (new Android.Graphics.Color (color.R, color.G, color.B, (byte)(color.A * 0.7f)));
				Control.SetTextColor (color);
				var d = originalBackground.GetConstantState ().NewDrawable ();
				d.SetColorFilter (color, Android.Graphics.PorterDuff.Mode.SrcIn);
				Control.Background = d;
			}
		}
	}
}