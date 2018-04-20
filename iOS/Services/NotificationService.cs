using System;
using UIKit;
using Foundation;

namespace PaketGlobal.iOS
{
	public class NotificationService : INotificationService
	{
		public NotificationService()
		{

		}

		public void ShowMessage(string text, bool lengthLong = false)
		{
			var alert = new UIAlertView("", text, null, "OK");
			alert.Show();
		}
	}
}
