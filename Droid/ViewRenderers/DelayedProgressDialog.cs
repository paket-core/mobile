using System;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Views;
using Android.Widget;

namespace PaketGlobal.Droid
{
    public class DelayedProgressDialog : DialogFragment
    {
        private static int DELAY_MILLISECOND = 450;
        private static int SHOW_MIN_MILLISECOND = 300;
        private static int PROGRESS_CONTENT_SIZE_DP = 80;

        private ProgressBar mProgressBar;
        private bool startedShowing;
        private long mStartMillisecond;
        private long mStopMillisecond;

        public DelayedProgressDialog()
        {
        }

        public override Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(this.Activity);
            LayoutInflater inflater = this.Activity.LayoutInflater;

            builder.SetView(inflater.Inflate(Resource.Layout.progress_dialog, null));
           
            return builder.Create();
        }

        public override void OnStart()
        {
            base.OnStart();

            mProgressBar = this.Dialog.FindViewById(Resource.Id.progress) as ProgressBar;

            if (this.Dialog.Window != null)
            {
                int px = (int)(PROGRESS_CONTENT_SIZE_DP * this.Resources.DisplayMetrics.Density);

                this.Dialog.Window.SetLayout(px, px);
                this.Dialog.Window.SetBackgroundDrawable(new ColorDrawable(Color.Transparent));
            }
        }

        public override void Show(FragmentManager manager, string tag)
        {
            mStartMillisecond = Java.Lang.JavaSystem.CurrentTimeMillis();
            startedShowing = false;
            mStopMillisecond = long.MaxValue;

            this.ShowDialogAfterDelay(manager, tag);
        }

        private void ShowDialogAfterDelay(FragmentManager fm, string tag)
        {
            startedShowing = true;

            FragmentTransaction ft = fm.BeginTransaction();
            ft.Add(this, tag);
            ft.CommitAllowingStateLoss();
        }

        public void Cancel()
        {
            if (startedShowing)
            {
                if (mProgressBar != null)
                {
                    CancelWhenShowing();
                }
                else
                {
                    CancelWhenNotShowing();
                }
            }
        }

        private void CancelWhenShowing()
        {
            DismissAllowingStateLoss();
        }

        private void CancelWhenNotShowing()
        {
            DismissAllowingStateLoss();
        }
    }
}
