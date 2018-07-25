using System;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class BottomBarPage : TabbedPage
	{
		public enum BarThemeTypes { Light, DarkWithAlpha, DarkWithoutAlpha }

		public bool FixedMode { get; set; }

		public BarThemeTypes BarTheme { get; set; }

		public void RaiseCurrentPageChanged()
		{
			OnCurrentPageChanged();
		}
	}
}

