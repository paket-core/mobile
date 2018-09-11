using System;
using Xamarin.Forms;

namespace PaketGlobal
{
    public class PaketSlider : Slider
    {
        // Events for external use (for example XAML)
        public event EventHandler TouchDown;
        public event EventHandler TouchUp;

        // Events called by renderers
        public EventHandler TouchDownEvent;
        public EventHandler TouchUpEvent;

        public PaketSlider()
        {
            TouchDownEvent = delegate
            {
                TouchDown?.Invoke(this, EventArgs.Empty);
            };
            TouchUpEvent = delegate
            {
                TouchUp?.Invoke(this, EventArgs.Empty);
            };
        }
    }
}
