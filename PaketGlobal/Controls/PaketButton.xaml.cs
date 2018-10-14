using System;
using System.Windows.Input;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PaketButton
    {
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand),
            typeof(PaketButton));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter),
            typeof(object),
            typeof(PaketButton));

        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
            typeof(PaketButton), null,
            propertyChanged: (bindable, oldVal, newVal) => ((PaketButton)bindable).OnTextChange((string)newVal));

        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool),
            typeof(PaketButton), false,
            propertyChanged: (bindable, oldVal, newVal) => ((PaketButton)bindable).OnIsBusy((bool)newVal));

        public static readonly BindableProperty ButtonBackgroundProperty = BindableProperty.Create(nameof(ButtonBackground), typeof(string),
             typeof(PaketButton), null,
             propertyChanged: (bindable, oldVal, newVal) => ((PaketButton)bindable).OnButtonBackgroundChange((string)newVal));

        public static readonly BindableProperty DisabledProperty = BindableProperty.Create(nameof(Disabled), typeof(bool),
          typeof(PaketButton), false,
          propertyChanged: (bindable, oldVal, newVal) => ((PaketButton)bindable).OnIsDisabled((bool)newVal));

        public event EventHandler Clicked;

        public PaketButton()
        {
            InitializeComponent();
            GetButtonStyle();
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string ButtonBackground
        {
            get => (string)GetValue(ButtonBackgroundProperty);
            set => SetValue(ButtonBackgroundProperty, value);
        }

        public bool Disabled
        {
            get => (bool)GetValue(DisabledProperty);
            set => SetValue(DisabledProperty, value);
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        private void OnTextChange(string value)
        {
            InnerButton.Text = value;
        }

        private void OnButtonBackgroundChange(string value){
            InnerButton.BackgroundColor = Color.FromHex(value);
        }

        private void OnClicked(object sender, EventArgs e)
        {
            Clicked?.Invoke(this, EventArgs.Empty);

            if (Command == null || !Command.CanExecute(CommandParameter))
                return;

            Command.Execute(CommandParameter);
        }

        private void OnIsDisabled(bool value)
        {
            if(!value)
            {
                InnerButton.Opacity = 1.0f;
                if(ButtonBackground==null)
                {
                    ButtonBackground = "#53C5C7";
                }
                InnerButton.BackgroundColor = Color.FromHex(ButtonBackground);
            }
            else
            {
                InnerButton.Opacity = 0.5f;
                InnerButton.BackgroundColor = Color.FromHex("#A7A7A7");
            }
        }

         private async void OnIsBusy(bool value)
        {
            if (value)
            {
                InnerActivityView.IsVisible = true;
                await InnerActivityView.FadeTo(1);
            }
            else
            {
                await InnerActivityView.FadeTo(0);
                InnerActivityView.IsVisible = false;
            }

            InnerActivityIndicator.IsRunning = value;
        }

        private void GetButtonStyle()
        {
            InnerBoxView.WidthRequest = InnerButton.Width;
            InnerBoxView.HeightRequest = InnerButton.Height;
            InnerBoxView.CornerRadius = InnerButton.CornerRadius;
            InnerBoxView.BackgroundColor = InnerButton.BackgroundColor;
            InnerBoxView.BorderThickness = (int)InnerButton.BorderWidth;
            InnerBoxView.BorderColor = InnerButton.BorderColor;
            InnerActivityIndicator.Color = InnerButton.TextColor;
        }
    }
}
