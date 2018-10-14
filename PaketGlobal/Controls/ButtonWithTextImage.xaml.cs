using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ButtonWithTextImage
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
                                                                       typeof(ButtonWithTextImage), null,
                                                                       propertyChanged: (bindable, oldVal, newVal) => ((ButtonWithTextImage)bindable).OnTextChange((string)newVal));
                                                                                       

        public static readonly BindableProperty ImageProperty = BindableProperty.Create(nameof(Image), typeof(string),
                                                                       typeof(ButtonWithTextImage), null,
                                                                       propertyChanged: (bindable, oldVal, newVal) => ((ButtonWithTextImage)bindable).OnImageChange((string)newVal));

        public static readonly BindableProperty ColorProperty = BindableProperty.Create(nameof(BtnColor), typeof(string),
                                                               typeof(ButtonWithTextImage), null,
                                                               propertyChanged: (bindable, oldVal, newVal) => ((ButtonWithTextImage)bindable).OnColorChange((string)newVal));

        public event EventHandler Clicked;

        public string Text
        {
            get => (string)TextLabel.Text;
            set => SetValue(TextProperty, value);
        }

        public string Image
        {
            get => (string)GetValue(ImageProperty);
            set => SetValue(ImageProperty, value);
        }

        public string BtnColor
        {
            get => (string)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        private void OnTextChange(string value)
        {
            TextLabel.Text = value;
        }

        private void OnImageChange(string value)
        {
            Icon.Source = value;
        }

        private void OnColorChange(string value)
        {
            Button.ButtonBackground = value;
        }

        void Handle_Clicked(object sender, System.EventArgs e)
        {
            Clicked?.Invoke(this, EventArgs.Empty);
        }

        public ButtonWithTextImage()
        {
            InitializeComponent();

        }
    }
}
