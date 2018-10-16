using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class SegmentView : RelativeLayout, ISwipeCallBack
    {
        public int SelectedIndex = 0;

        public event EventHandler Clicked;

        public SegmentView()
        {
            InitializeComponent();

          // SwipeListener swipeListener = new SwipeListener(FrameView, this);

            SecondLabel.Text = SecondLabel.Text.ToUpperInvariant();
            FirstLabel.Text = FirstLabel.Text.ToUpperInvariant();


            var tapCommand_1 = new Command(() =>
            {
                if(SelectedIndex==0)
                {
                    return;
                }

                SelectedIndex = 0;

                Animate();
            });

            var tapCommand_2 = new Command(() =>
            {
                if (SelectedIndex == 1)
                {
                    return;
                }

                SelectedIndex = 1;

                Animate();
            });

            XamEffects.Commands.SetTap(FirstLabel, tapCommand_1);
            XamEffects.Commands.SetTap(SecondLabel, tapCommand_2);

    }

    private async void Animate()
        {
            if (SelectedIndex==0)
            {
                FirstLabel.TextColor = Color.Black;
                SecondLabel.TextColor = Color.FromHex("#E5E5E5");

                await SelectedView.TranslateTo(0, SelectedView.TranslationY, 100);
            }
            else if (SelectedIndex == 1)
            {
                SecondLabel.TextColor = Color.Black;
                FirstLabel.TextColor = Color.FromHex("#E5E5E5");

               await SelectedView.TranslateTo((this.Width/2)-17, SelectedView.TranslationY, 100);
            }

            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public void SelectIndex(int index)
        {
            SelectedIndex = index;
            Animate();
        }

        public void onLeftSwipe(View view)
        {
            if (SelectedIndex == 0)
            {
                return;
            }

            SelectedIndex = 0;

            Animate();
        }    

        public void onRightSwipe(View view)
        {
            if (SelectedIndex == 1)
            {
                return;
            }

            SelectedIndex = 1;

            Animate();
        }

        public void onTopSwipe(View view)
        {
       
        }

        public void onNothingSwiped(View view)
        {

        }

        public void onBottomSwipe(View view)
        {

        }
    }
}
