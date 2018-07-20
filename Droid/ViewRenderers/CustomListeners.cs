using System;
using Android.Views;

namespace PaketGlobal.Droid
{
	/// <summary>
	/// Swipe gesture listener.
	/// </summary>
	public class SwipeGestureListener : GestureDetector.SimpleOnGestureListener
	{
		const int SWIPE_THRESHOLD = 100;
		const int SWIPE_VELOCITY_THRESHOLD = 100;

		public Action SwipeRight { get; set; }
		public Action SwipeLeft { get; set; }
		public Action SwipeTop { get; set; }
		public Action SwipeBottom { get; set; }

		public override bool OnDown(MotionEvent e)
		{
			return true;
		}

		public override bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
		{
			bool result = false;
			try {
				float diffY = e2.GetY() - e1.GetY();
				float diffX = e2.GetX() - e1.GetX();
				if (Math.Abs(diffX) > Math.Abs(diffY)) {
					if (Math.Abs(diffX) > SWIPE_THRESHOLD && Math.Abs(velocityX) > SWIPE_VELOCITY_THRESHOLD) {
						if (diffX > 0) {
							OnSwipeRight();
						} else {
							OnSwipeLeft();
						}
						result = true;
					}
				} else if (Math.Abs(diffY) > SWIPE_THRESHOLD && Math.Abs(velocityY) > SWIPE_VELOCITY_THRESHOLD) {
					if (diffY > 0) {
						OnSwipeBottom();
					} else {
						OnSwipeTop();
					}
					result = true;
				}
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
			}
			return result;
		}

		public void OnSwipeRight()
		{
			SwipeRight?.Invoke();
		}

		public void OnSwipeLeft()
		{
			SwipeLeft?.Invoke();
		}

		public void OnSwipeTop()
		{
			SwipeTop?.Invoke();
		}

		public void OnSwipeBottom()
		{
			SwipeBottom?.Invoke();
		}
	}
}
