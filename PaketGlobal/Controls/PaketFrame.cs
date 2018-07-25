using System;
using Xamarin.Forms;


namespace PaketGlobal
{
    public class PaketFrame : Frame
    {
        public static readonly BindableProperty TopCornersProperty =
            BindableProperty.Create(nameof(TopCorners), typeof(int), typeof(PaketFrame), 0);

        public int TopCorners
        {
            get { return (int)GetValue(TopCornersProperty); }
            set { SetValue(TopCornersProperty, value); }
        }
    }

    public class PaketFrameWithShadow : Frame
    {

    }
}
