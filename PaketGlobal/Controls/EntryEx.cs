using System;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class EntryEx : Entry
	{
		public static readonly BindableProperty TintColorProperty = BindableProperty.Create<EntryEx, Color> (p => p.TintColor, Color.Black);
		public static readonly BindableProperty InputEnabledProperty = BindableProperty.Create<EntryEx, bool> (p => p.InputEnabled, true);

		public Color TintColor {
			get {
				return (Color)GetValue (TintColorProperty);
			}
			set {
				SetValue (TintColorProperty, value);
			}
		}

		public bool InputEnabled {
			get {
				return (bool)GetValue (InputEnabledProperty);
			}
			set {
				SetValue (InputEnabledProperty, value);
			}
		}

		public EntryEx () : base ()
		{
			
		}
	}
}
