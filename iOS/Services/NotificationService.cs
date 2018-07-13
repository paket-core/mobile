using System;

using UIKit;
using Foundation;

using GlobalToast;
using GlobalToast.Animation;

namespace PaketGlobal.iOS
{
	public class NotificationService : INotificationService
	{
		public NotificationService()
		{
            Toast.GlobalAnimator = new ScaleAnimator();
            Toast.GlobalLayout.MarginBottom = 16f;
            Toast.GlobalAppearance.MessageColor = UIColor.White;
            Toast.GlobalAppearance.TitleFont = UIFont.FromName("Poppins-Regular", 12);
            Toast.GlobalAppearance.Color = UIColor.Black.ColorWithAlpha(0.7f);
		}

		public void ShowMessage(string text, bool lengthLong = false)
		{
            // More configurations
            Toast.MakeToast(text)
                 .SetShowShadow(false) // Default is true
                 .SetPosition(ToastPosition.Bottom) // Default is Bottom
                 .Show();
		}
	}
}
