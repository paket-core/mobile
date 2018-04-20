using System;

[assembly: Xamarin.Forms.Dependency (typeof(PaketGlobal.Droid.ScreenScale))]
namespace PaketGlobal.Droid
{
	public class ScreenScale : IScreenScale
	{
		public float GetScreenScale ()
		{
			return Android.App.Application.Context.Resources.DisplayMetrics.ScaledDensity;
		}
	}
}

