using System;
using Android.Content;
using Android.Views;
using PaketGlobal;
using PaketGlobal.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(PaketSlider), typeof(PaketSliderRender))]

namespace PaketGlobal.Droid
{
    public class PaketSliderRender : Xamarin.Forms.Platform.Android.SliderRenderer
    {
        private bool assign = false;

        public PaketSliderRender(Context context) : base(context)
        {

        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.RequestDisallowInterceptTouchEvent(true);
                    break;
                case MotionEventActions.Move:
                    //This is the core of the problem!!!
                    Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.Parent.RequestDisallowInterceptTouchEvent(true);
                    break;
                case MotionEventActions.Up:
                    var element = (PaketSlider)Element;
                    element.TouchUpEvent(this, EventArgs.Empty);
                    break;
                default:
                    break;
            }
            return base.DispatchTouchEvent(e);
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<Slider> e)
        //{
        //    base.OnElementChanged(e);

        //    var element = (PaketSlider)Element;
        //    if (Control != null)
        //    {
        //        var seekBar = Control;

        //        if(!assign)
        //        {
        //            assign = true;

        //            seekBar.StartTrackingTouch += (sender, args) =>
        //            {
        //                element.TouchDownEvent(this, EventArgs.Empty);
        //            };

        //            seekBar.StopTrackingTouch += (sender, args) =>
        //            {
        //                element.TouchUpEvent(this, EventArgs.Empty);
        //            };
        //            //////// On Android you need to check if ProgressChange by user
        //            //seekBar.ProgressChanged += delegate (object sender, Android.Widget.SeekBar.ProgressChangedEventArgs args)
        //            //{
        //            //    if (args.FromUser)
        //            //    {
        //            //        element.Value = (element.Minimum + ((element.Maximum - element.Minimum) * (args.Progress) / 1000.0));
        //            //    }
        //            //};
        //        }
             
        //    }

        //}

    }
}
