using System;
using System.Collections.Generic;

using Android.Views;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Android.Views.Animations;
using Android.Support.V7.Widget;

using PaketGlobal;
using PaketGlobal.Droid;

using Android.Graphics;
using Android.Graphics.Drawables;

namespace PaketGlobal.Droid
{
    /// <summary>
    /// Banner notification view.
    /// </summary>
    public class BannerView : FrameLayout
    {
        #region Declarations

        const double BannerTimeout = 6000;

        Android.Support.V4.View.GestureDetectorCompat gestureDetector;
        SwipeGestureListener gestureListener;

        CardView innerView;

        bool isShowing;

        Package newPackage;

        System.Timers.Timer timer;

        #endregion

        #region Events

        public delegate void BannerFinishedEventHandler(bool canceled, Package package);
        public event BannerFinishedEventHandler BannerFinished;

        #endregion


        public BannerView(Context context) : base(context)
        {
            Init();
        }

        public BannerView(Context context, Android.Util.IAttributeSet atr) : base(context, atr)
        {
            Init();
        }

        public BannerView(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            Init();
        }

        public BannerView(Context context, Android.Util.IAttributeSet atr, int defStyle) : base(context, atr, defStyle)
        {
            Init();
        }

        private void Init()
        {
            this.Clickable = true;

            timer = new System.Timers.Timer(BannerTimeout);
            gestureDetector = new Android.Support.V4.View.GestureDetectorCompat(Context, gestureListener = new SwipeGestureListener());

            innerView = new CardView(Context);
            innerView.UseCompatPadding = true;

            Android.Support.V4.View.ViewCompat.SetElevation(innerView, 4.DpToPx());
            innerView.Radius = 8.DpToPx();

            var w = Android.App.Application.Context.Resources.DisplayMetrics.WidthPixels - 120;

            using (var prms = new LayoutParams(LayoutParams.WrapContent, LayoutParams.WrapContent))
            {
                prms.Width = w;
                prms.SetMargins(10.DpToPx(), 40.DpToPx(), 10.DpToPx(), 0);
                this.AddView(innerView, prms);
            }


            var inflater = (LayoutInflater)Context.GetSystemService(Context.LayoutInflaterService);
            inflater.Inflate(Resource.Layout.banner_notification, innerView);


            Typeface tfMedium = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Medium.ttf");
            Typeface tfNormal = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Regular.ttf");


            var title = innerView.FindViewById(Resource.Id.text_banner_title) as TextView;
            title.SetTypeface(tfMedium, TypefaceStyle.Normal);
            title.SetTextSize(Android.Util.ComplexUnitType.Dip, 14);

            var subtitle = innerView.FindViewById(Resource.Id.text_banner_subtitle) as TextView;
            subtitle.SetTypeface(tfNormal, TypefaceStyle.Normal);
            subtitle.SetTextSize(Android.Util.ComplexUnitType.Dip, 12);

            innerView.Visibility = ViewStates.Gone;
        }


        public void ShowWallet(string title, string subtitle)
        {
            if (!isShowing)
            {
                newPackage = null;

                isShowing = true;

                DisplayBanner(title, subtitle, "color_wallet.png");
            }
        }

        public void ShowStringPackage(string title, string body)
        {
            if (!isShowing)
            {
                isShowing = true;

                var icon = "delivered_icon.png";

                if(title.ToLower().Contains("waiting"))
                {
                    icon = "waiting_pickup.png";
                }
                else if (title.ToLower().Contains("transit"))
                {
                    icon = "in_transit.png";
                }

                DisplayBanner(title, body, icon);
            }
        }

        public void ShowPackage(List<Package> packages)
        {
            if (!isShowing)
            {
                newPackage = packages[0];

                isShowing = true;

                string title = "";
                string subtitle = "";

                if (newPackage.isNewPackage)
                {
                    title = "You have a new Package " + newPackage.ShortEscrow;
                }
                else if(newPackage.isAssigned)
                {
                    title = "Your Package " + newPackage.ShortEscrow + " assigned";
                }
                else if (newPackage.IsExpired)
                {
                    title = "Your Package " + newPackage.ShortEscrow + " expired";
                }
                else
                {
                    title = "Your Package " + newPackage.ShortEscrow + " " + newPackage.FormattedStatus;
                }

                subtitle = "Please check your Packages archive for more details";

                DisplayBanner(title, subtitle, newPackage.StatusIcon);
            }
        }

        private void DisplayBanner(string titleString, string subTitleString, string iconName)
        {
            int resID = Resources.GetIdentifier(iconName.Replace(".png", ""), "drawable", this.Context.PackageName);

            var title = innerView.FindViewById(Resource.Id.text_banner_title) as TextView;
            var subtitle = innerView.FindViewById(Resource.Id.text_banner_subtitle) as TextView;
            var image = innerView.FindViewById(Resource.Id.image_banner) as ImageView;

            title.Text = titleString;
            subtitle.Text = subTitleString;
            image.SetImageResource(resID);

            var wm = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var prms = new WindowManagerLayoutParams(WindowManagerLayoutParams.WrapContent, WindowManagerLayoutParams.WrapContent, WindowManagerTypes.ApplicationPanel,
                                                     WindowManagerFlags.NotFocusable | WindowManagerFlags.WatchOutsideTouch | WindowManagerFlags.LayoutNoLimits,
                                                     Android.Graphics.Format.Translucent);
            prms.Gravity = GravityFlags.Top | GravityFlags.CenterHorizontal;

            try
            {
                wm.AddView(this, prms);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

			gestureListener.SwipeTop += HandleBannerSwipeTop;
			gestureListener.SwipeLeft += HandleBannerSwipeLeft;
			gestureListener.SwipeRight += HandleBannerSwipeRight;
            timer.Elapsed += HandleBannerTimeout;
            this.Click += HandleBannerClick;

            var inAnim = AnimationUtils.LoadAnimation(Context, Resource.Animation.slide_in_top);
            inAnim.Duration = 300;
            inAnim.AnimationEnd += InAnimationEnd;
            innerView.Visibility = ViewStates.Visible;
            innerView.StartAnimation(inAnim);

            timer.Stop();
            timer.Start();
        }

        void InAnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            e.Animation.AnimationEnd -= InAnimationEnd;
            e.Animation.Dispose();
        }

        /// <summary>
        /// Hides the banner.
        /// </summary>
        public void Hide(SwipeDirection direction)
        {
            if (isShowing)
            {
				gestureListener.SwipeTop -= HandleBannerSwipeTop;
				gestureListener.SwipeLeft -= HandleBannerSwipeLeft;
				gestureListener.SwipeRight -= HandleBannerSwipeRight;
                timer.Stop();
                timer.Elapsed -= HandleBannerTimeout;
                this.Click -= HandleBannerClick;

				var outAnim = AnimationUtils.LoadAnimation(Context, direction == SwipeDirection.Top ? Resource.Animation.slide_out_top : (direction == SwipeDirection.Left ? Resource.Animation.slide_out_left : Resource.Animation.slide_out_right));
                outAnim.Duration = 300;
                outAnim.AnimationEnd += OutAnimationEnd;
                innerView.StartAnimation(outAnim);
            }
        }

        void HandleBannerTimeout(object sender, System.Timers.ElapsedEventArgs e)
        {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                OnBannerFinished(true, null);
            });
        }

        void OutAnimationEnd(object sender, Animation.AnimationEndEventArgs e)
        {
            e.Animation.AnimationEnd -= OutAnimationEnd;
            e.Animation.Dispose();
            innerView.Visibility = ViewStates.Gone;

            var wm = Context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            wm.RemoveView(this);

            isShowing = false;
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            gestureDetector?.OnTouchEvent(e);

            return base.OnTouchEvent(e);
        }

        void HandleBannerClick(object sender, EventArgs e)
        {
            OnBannerFinished(false, newPackage);
        }

		void HandleBannerSwipeTop()
		{
			HandleBannerSwipe(SwipeDirection.Top);
		}

		void HandleBannerSwipeLeft()
		{
			HandleBannerSwipe(SwipeDirection.Left);
		}

		void HandleBannerSwipeRight()
		{
			HandleBannerSwipe(SwipeDirection.Right);
		}

		void HandleBannerSwipe(SwipeDirection direction)
        {
			OnBannerFinished(true, null, direction);
        }

		void OnBannerFinished(bool canceled, Package package, SwipeDirection direction = SwipeDirection.Top)
        {
            Hide(direction);
            BannerFinished?.Invoke(canceled, package);
        }
    }
}
