using System;
using Xamarin.Forms;


namespace PaketGlobal
{
    public enum ImageAlignment
    {
        Left,
        Right
    }

    public class PaketEntry : Entry
    {
        public static readonly BindableProperty ImageProperty =
            BindableProperty.Create(nameof(Image), typeof(string), typeof(PaketEntry), string.Empty);

        public static readonly BindableProperty LineColorProperty =
            BindableProperty.Create(nameof(LineColor), typeof(Color), typeof(PaketEntry), Color.Gray.MultiplyAlpha(0.3));

        public static readonly BindableProperty ImageHeightProperty =
            BindableProperty.Create(nameof(ImageHeight), typeof(int), typeof(PaketEntry), 20);

        public static readonly BindableProperty ImageWidthProperty =
            BindableProperty.Create(nameof(ImageWidth), typeof(int), typeof(PaketEntry), 20);

        public static readonly BindableProperty ImageAlignmentProperty =
            BindableProperty.Create(nameof(ImageAlignment), typeof(ImageAlignment), typeof(PaketEntry), ImageAlignment.Right);

        public static readonly BindableProperty BackgroundProperty =
            BindableProperty.Create(nameof(BackgroundV), typeof(int), typeof(PaketEntry), 0);

        public static readonly BindableProperty CapSentencesProperty =
            BindableProperty.Create(nameof(CapSentences), typeof(int), typeof(PaketEntry), 0);


        public int CapSentences
        {
            get { return (int)GetValue(CapSentencesProperty); }
            set { SetValue(CapSentencesProperty, value); }
        }

        public int BackgroundV
        {
            get { return (int)GetValue(BackgroundProperty); }
            set { SetValue(BackgroundProperty, value); }
        }

        public Color LineColor
        {
            get { return (Color)GetValue(LineColorProperty); }
            set { SetValue(LineColorProperty, value); }
        }

        public int ImageWidth
        {
            get { return (int)GetValue(ImageWidthProperty); }
            set { SetValue(ImageWidthProperty, value); }
        }

        public int ImageHeight
        {
            get { return (int)GetValue(ImageHeightProperty); }
            set { SetValue(ImageHeightProperty, value); }
        }

        public string Image
        {
            get { return (string)GetValue(ImageProperty); }
            set { SetValue(ImageProperty, value); }
        }

        public ImageAlignment ImageAlignment
        {
            get { return (ImageAlignment)GetValue(ImageAlignmentProperty); }
            set { SetValue(ImageAlignmentProperty, value); }
        }
    }
}
