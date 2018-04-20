using Xamarin.Forms;

namespace PaketGlobal
{
	public class ViewHelper
	{
		public static async System.Threading.Tasks.Task ToggleViews (View inView, View outView = null, bool animated = true)
		{
			if (inView != null) {
				inView.IsVisible = true;
			}

			if (animated) {
				if (inView != null) {
					inView.Opacity = 0.0;
					inView.FadeTo (1.0);
				}
				if (outView != null) {
					await outView.FadeTo (0.0);
				}
			}

			if (outView != null) {
				outView.IsVisible = false;
			}
		}

		public static void SwitchActivityIndicator (ActivityIndicator indicator, bool state)
		{
			indicator.Opacity = state ? 0.0 : 1.0;
			indicator.IsRunning = state;
			indicator.FadeTo (state ? 1.0 : 0.0);
		}
	}
}
