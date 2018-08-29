using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace PaketGlobal
{

    public partial class PaketEntryWithProgress
    {
        public static readonly BindableProperty TextProperty = BindableProperty.Create(nameof(Text), typeof(string),
                                                                        typeof(PaketEntryWithProgress), null,
                                                                        propertyChanged: (bindable, oldVal, newVal) => ((PaketEntryWithProgress)bindable).OnTextChange((string)newVal));

        public static readonly BindableProperty PlacholderProperty = BindableProperty.Create(nameof(Placholder), typeof(string),
                                                                        typeof(PaketEntryWithProgress), null,
                                                                        propertyChanged: (bindable, oldVal, newVal) => ((PaketEntryWithProgress)bindable).OnPlacholderChange((string)newVal));

        public static readonly BindableProperty TopTextProperty = BindableProperty.Create(nameof(TopText), typeof(string),
                                                                       typeof(PaketEntryWithProgress), null,
                                                                       propertyChanged: (bindable, oldVal, newVal) => ((PaketEntryWithProgress)bindable).OnTopTextChange((string)newVal));

        public static readonly BindableProperty IsBusyProperty = BindableProperty.Create(nameof(IsBusy), typeof(bool),
                                                                        typeof(PaketEntryWithProgress), false,
                                                                        propertyChanged: (bindable, oldVal, newVal) => ((PaketEntryWithProgress)bindable).OnIsBusy((bool)newVal));

        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand),
                                                                        typeof(PaketEntryWithProgress));

        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter),
                                                                        typeof(object), typeof(PaketEntryWithProgress));

        public static readonly BindableProperty EntryHeightProperty = BindableProperty.Create(nameof(EntryHeight), typeof(int),
                                                                typeof(PaketEntryWithProgress), 0,
                                                                propertyChanged: (bindable, oldVal, newVal) => ((PaketEntryWithProgress)bindable).OnEntryHeightChange((int)newVal));
        
        public event EventHandler Completed;
        public event EventHandler Unfocus;

        public string Text
        {
            get => (string)EntryView.Text;
            set => SetValue(TextProperty, value);
        }

        public string Placholder
        {
            get => (string)GetValue(PlacholderProperty);
            set => SetValue(PlacholderProperty, value);
        }

        public string TopText
        {
            get => (string)GetValue(TopTextProperty);
            set => SetValue(TopTextProperty, value);
        }

        public bool IsBusy
        {
            get => (bool)GetValue(IsBusyProperty);
            set => SetValue(IsBusyProperty, value);
        }

        public int EntryHeight
        {
            get => (int)GetValue(EntryHeightProperty);
            set => SetValue(EntryHeightProperty, value);
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

        public PaketEntryWithProgress()
        {
            InitializeComponent();
        }


        private void OnTextChange(string value)
        {
            EntryView.Text = value;
        }

        private void OnPlacholderChange(string value)
        {
            EntryView.Placeholder = value;
        }

        private void OnTopTextChange(string value)
        {
            TopLabel.Text = value;

            TopLabel.IsVisible = (TopLabel.Text.Length > 0);
        }

        private void OnEntryHeightChange(int value)
        {
            if(value>0)
            {
                EntryView.HeightRequest = value;
            }
        }

        private void OnIsBusy(bool value)
        {
            ProgressIndicator.IsRunning = value;

            EntryView.IsEnabled = !value;

            if(value)
            {
                EntryView.PaddingRight = 110;
            }
        }

        private void FieldCompleted(object sender, System.EventArgs e)
        {
            Completed?.Invoke(this, EventArgs.Empty);

            if (Command == null || !Command.CanExecute(CommandParameter))
                return;

            Command.Execute(CommandParameter);
        }

        private void FieldUnfocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            Unfocus?.Invoke(this, EventArgs.Empty);

            if (Command == null || !Command.CanExecute(CommandParameter))
                return;

            Command.Execute(CommandParameter);
        }

        private void FieldTextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            ToDefaultState();
        }

        public void FocusField()
        {
            EntryView.Focus();
        }

        private void ShowErrorWithText(string text)
        {
            EntryView.PaddingRight = 110;

            TopLabel.TextColor = Color.FromHex("#D43F51");
            TopLabel.Text = text;
            TopLabel.IsVisible = true;
            EntryView.LineColor = Color.FromHex("#D43F51");

            StatusImage.Source = "warning.png";
            StatusImage.IsVisible = true;
        }

        private void ShowSuccess()
        {
            EntryView.PaddingRight = 110;

            TopLabel.Text = TopText;
            TopLabel.IsVisible = (TopLabel.Text.Length > 0);
            TopLabel.TextColor = Color.FromHex("#A7A7A7");
            EntryView.LineColor = Color.FromHex("#E5E5E5");

            StatusImage.Source = "success.png";
            StatusImage.IsVisible = true;
        }

        public void ToDefaultState()
        {
            EntryView.PaddingRight = 35;

            TopLabel.Text = TopText;
            TopLabel.IsVisible = (TopLabel.Text.Length > 0);
            TopLabel.TextColor = Color.FromHex("#A7A7A7");
            EntryView.LineColor = Color.FromHex("#E5E5E5");

            StatusImage.IsVisible = false;
        }

        public async Task<string> CheckValidCallSignOrPubKey()
        {
			if (String.IsNullOrWhiteSpace(Text) == false) {
				if (Text.Length == 56) {
					var valid = StellarHelper.IsValidPubKey(Text);

					if (valid == false) {
						ShowErrorWithText(AppResources.InvalidPubKey);

						return "";
					} else {
                        ToDefaultState();

						this.IsBusy = true;

						var result = await App.Locator.IdentityServiceClient.GetUser(Text, null);

						var trusted = await StellarHelper.CheckTokenTrustedWithPubKey(Text);

						if (result == null && !trusted) {
							ShowErrorWithText(AppResources.UserNotRegistered);
						} else if (!trusted) {
							ShowErrorWithText(AppResources.UserNotTrusted);
						} else if (result != null && trusted) {
							ShowSuccess();
						}
                  

						this.IsBusy = false;
					}

					return Text;
				} else {
                    ToDefaultState();

					this.IsBusy = true;

					var result = await App.Locator.IdentityServiceClient.GetUser(null, Text);

					if (result == null) {
						ShowErrorWithText(AppResources.UserNotFound);

						this.IsBusy = false;

						return "";
					} else {
						var trusted = await StellarHelper.CheckTokenTrustedWithPubKey(result.UserDetails.Pubkey);

						if (!trusted) {
							ShowErrorWithText(AppResources.UserNotTrusted);
						} else {
							ShowSuccess();
						}
					}

					this.IsBusy = false;

					return result.UserDetails.Pubkey;
				}
			}

            return "";
        }
    }
}
