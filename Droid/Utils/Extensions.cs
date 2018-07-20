using System;

using Android.App;
using Android.Content;
using Android.Locations;
using Android.Net;
using Android.OS;
using Android.Util;


namespace PaketGlobal.Droid
{
	public static partial class Extensions
	{

		public static bool HasFroyo() {
			// Can use static final constants like FROYO, declared in later versions
			// of the OS since they are inlined at compile time. This is guaranteed behavior.
			return Build.VERSION.SdkInt >= BuildVersionCodes.Froyo;
		}

		public static bool HasGingerbread() {
			return Build.VERSION.SdkInt >= BuildVersionCodes.Gingerbread;
		}

		public static bool HasHoneycomb() {
			return Build.VERSION.SdkInt >= BuildVersionCodes.Honeycomb;
		}

		public static bool HasHoneycombMR1() {
			return Build.VERSION.SdkInt >= BuildVersionCodes.HoneycombMr1;
		}

		public static bool HasJellyBean() {
			return Build.VERSION.SdkInt >= BuildVersionCodes.JellyBean;
		}

		public static bool HasKitKat() {
			return Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;
		}

		public static bool HasLollipop(){
			return Build.VERSION.SdkInt >= BuildVersionCodes.Kitkat;
		}

		public static int DpToPx(this int px)
		{
			return (int)TypedValue.ApplyDimension(ComplexUnitType.Dip, px, Application.Context.Resources.DisplayMetrics);
		}

		public static float DpToPx(this float px)
		{
			return TypedValue.ApplyDimension(ComplexUnitType.Dip, px, Application.Context.Resources.DisplayMetrics);
		}

        public static int PxToDp(this int dp)
        {
            return (int)(dp / Application.Context.Resources.DisplayMetrics.Density);
        }


		public static float DistanceBetweenPoints(this System.Drawing.PointF p1, System.Drawing.PointF p2)
		{
			var dist = (float)Math.Sqrt(Math.Pow(p2.X - p1.X, 2) + Math.Pow(p2.Y - p1.Y, 2));
			return dist;
		}
	}
}

