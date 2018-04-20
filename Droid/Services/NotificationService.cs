namespace PaketGlobal.Droid
{
	public class NotificationService : INotificationService
	{
		public NotificationService()
		{

		}

		public void ShowMessage(string text, bool lengthLong = false)
		{
			var activity = (Xamarin.Forms.Platform.Android.FormsAppCompatActivity)MainActivity.Instance;
			activity.RunOnUiThread(() => {
				using (var toast = Android.Widget.Toast.MakeText(activity, text, lengthLong ? Android.Widget.ToastLength.Long : Android.Widget.ToastLength.Short)) {
					toast.Show();
				}
			});
		}
	}
}
