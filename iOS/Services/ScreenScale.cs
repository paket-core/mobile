using System;

[assembly: Xamarin.Forms.Dependency (typeof (PaketGlobal.iOS.ScreenScale))]
namespace PaketGlobal.iOS
{
	public class ScreenScale : IScreenScale
	{
		public float GetScreenScale ()
		{
			return (float)UIKit.UIScreen.MainScreen.Scale;
		}
	}
}
