﻿using System.ComponentModel;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V7.Widget;
using Android.Text;
using Android.Text.Method;
using Android.Util;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Application = Android.App.Application;
using Color = Xamarin.Forms.Color;
using AColor = Android.Graphics.Color;
using FormsAppCompat = Xamarin.Forms.Platform.Android.AppCompat;
using PaketGlobal;
using PaketGlobal.Droid;
using Android.Graphics;
using Android.Support.V4.Content;

[assembly: ExportRenderer(typeof(XfxEntry), typeof(XfxEntryRendererDroid))]

namespace PaketGlobal.Droid
{
    public class XfxEntryRendererDroid : FormsAppCompat.ViewRenderer<XfxEntry, TextInputLayout>,
        ITextWatcher,
        TextView.IOnEditorActionListener
    {
        private bool _hasFocus;
        private ColorStateList _defaultTextColor;

        public XfxEntryRendererDroid(Context context) : base(context)
        {
            AutoPackage = false;
        }

        protected EditText EditText => Control.EditText;

        public bool OnEditorAction(TextView v, ImeAction actionId, KeyEvent e)
        {
            if ((actionId == ImeAction.Done) || ((actionId == ImeAction.ImeNull) && (e.KeyCode == Keycode.Enter)))
            {
                Control.ClearFocus();
                HideKeyboard();
                ((IEntryController)Element).SendCompleted();
            }
            return true;
        }

        public virtual void AfterTextChanged(IEditable s)
        {
        }

        public virtual void BeforeTextChanged(ICharSequence s, int start, int count, int after)
        {
        }

        public virtual void OnTextChanged(ICharSequence s, int start, int before, int count)
        {
            if (string.IsNullOrWhiteSpace(Element.Text) && (s.Length() == 0)) return;
            ((IElementController)Element).SetValueFromRenderer(Entry.TextProperty, s.ToString());
        }

        protected override TextInputLayout CreateNativeControl()
        {
            var textInputLayout = new TextInputLayout(Context);
            var editText = new AppCompatEditText(Context)
            {
                SupportBackgroundTintList = ColorStateList.ValueOf(GetPlaceholderColor())
            };
            editText.SetTextSize(ComplexUnitType.Sp, (float)Element.FontSize);
            textInputLayout.AddView(editText);
            return textInputLayout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
                if (Control != null)
                    EditText.FocusChange -= ControlOnFocusChange;

            if (e.NewElement != null)
            {
                var ctrl = CreateNativeControl();
                SetNativeControl(ctrl);

                if (!string.IsNullOrWhiteSpace(Element.AutomationId))
                    EditText.ContentDescription = Element.AutomationId;
                
                _defaultTextColor = EditText.TextColors;

                Focusable = true;
                EditText.ShowSoftInputOnFocus = true;

                // Subscribe
                EditText.FocusChange += ControlOnFocusChange;
                EditText.AddTextChangedListener(this);
                EditText.SetOnEditorActionListener(this);
                EditText.ImeOptions = ImeAction.Done;

                SetText();
                SetHintText();
                SetErrorText();
                SetFontAttributesSizeAndFamily();
                SetInputType();
                SetTextColor();
                SetHorizontalTextAlignment();
                SetFloatingHintEnabled();
                SetIsEnabled();
                SetErrorDisplay();
                SetLabelAndUnderlineColor();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.PlaceholderProperty.PropertyName)
                SetHintText();
            else if (e.PropertyName == XfxEntry.ErrorTextProperty.PropertyName)
                SetErrorText();
            else if (e.PropertyName == Entry.IsPasswordProperty.PropertyName ||
                e.PropertyName == InputView.KeyboardProperty.PropertyName)
                SetInputType();
            else if (e.PropertyName == Entry.TextProperty.PropertyName)
                SetText();
            else if (e.PropertyName == Entry.HorizontalTextAlignmentProperty.PropertyName)
                SetHorizontalTextAlignment();
            else if (e.PropertyName == XfxEntry.FloatingHintEnabledProperty.PropertyName)
                SetFloatingHintEnabled();
            else if (e.PropertyName == VisualElement.IsEnabledProperty.PropertyName)
                SetIsEnabled();
            else if ((e.PropertyName == Entry.FontAttributesProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontFamilyProperty.PropertyName) ||
                     (e.PropertyName == Entry.FontSizeProperty.PropertyName))
                SetFontAttributesSizeAndFamily();
            else if (e.PropertyName == XfxEntry.ActivePlaceholderColorProperty.PropertyName ||
                     e.PropertyName == Entry.PlaceholderColorProperty.PropertyName)
                SetLabelAndUnderlineColor();
            else if (e.PropertyName == Entry.TextColorProperty.PropertyName)
                SetTextColor();
        }

        private void ControlOnFocusChange(object sender, FocusChangeEventArgs args)
        {
            _hasFocus = args.HasFocus;
            if (_hasFocus)
            {
                var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);

                EditText.PostDelayed(() =>
                    {
                        EditText.RequestFocus();
                        manager.ShowSoftInput(EditText, 0);
                    },
                    100);
            }

            var isFocusedPropertyKey = Element.GetInternalField<BindablePropertyKey>("IsFocusedPropertyKey");
            ((IElementController)Element).SetValueFromRenderer(isFocusedPropertyKey, _hasFocus);
            SetUnderlineColor(_hasFocus ?  GetActivePlaceholderColor(): GetPlaceholderColor());
        }

        protected AColor GetPlaceholderColor() => Element.PlaceholderColor.ToAndroid(Color.FromHex("#80000000"));

        private AColor GetActivePlaceholderColor() => Element.ActivePlaceholderColor.ToAndroid(global::Android.Resource.Attribute.ColorAccent, Context);

        protected virtual void SetLabelAndUnderlineColor()
        {
            var defaultColor = GetPlaceholderColor();
            var activeColor = GetActivePlaceholderColor();

            SetHintLabelDefaultColor(defaultColor);
            SetHintLabelActiveColor(activeColor);
            SetUnderlineColor(_hasFocus ? activeColor : defaultColor);
        }

        private void SetUnderlineColor(AColor color)
        {
            var element = (ITintableBackgroundView)EditText;
            element.SupportBackgroundTintList = ColorStateList.ValueOf(AColor.Transparent);
        }

        private void SetHintLabelActiveColor(AColor color)
        {
          
        }

        private void SetHintLabelDefaultColor(AColor color)
        {
            var hint = Control.Class.GetDeclaredField("mDefaultTextColor");
            hint.Accessible = true;
            hint.Set(Control, new ColorStateList(new int[][] { new[] { 0 } }, new int[] { color }));
        }

        private void SetText()
        {
            if (EditText.Text != Element.Text)
            {
                EditText.Text = Element.Text;
                if (EditText.IsFocused)
                    EditText.SetSelection(EditText.Text.Length);
            }
        }

        private void SetHintText()
        {
            Control.Hint = Element.Placeholder;
        }

        private void SetTextColor()
        {
            if (Element.TextColor == Color.Default)
            {
                EditText.SetTextColor(_defaultTextColor);
            }
            else
            {
                EditText.SetTextColor(Element.TextColor.ToAndroid());
            }
        }

        private void SetHorizontalTextAlignment()
        {
            switch (Element.HorizontalTextAlignment)
            {
                case Xamarin.Forms.TextAlignment.Center:
                    EditText.Gravity = GravityFlags.CenterHorizontal;
                    break;
                case Xamarin.Forms.TextAlignment.End:
                    EditText.Gravity = GravityFlags.Right;
                    break;
                default:
                    EditText.Gravity = GravityFlags.Left;
                    break;
            }
        }

        public void SetFloatingHintEnabled()
        {
            Control.HintEnabled = false;
            Control.HintAnimationEnabled = false;
            SetFontAttributesSizeAndFamily();
        }

        public void SetErrorDisplay()
        {
            switch (Element.ErrorDisplay)
            {
                case ErrorDisplay.None:
                    Control.ErrorEnabled = false;
                    break;
                case ErrorDisplay.Underline:
                    Control.ErrorEnabled = true;
                    break;
            }
        }

        protected void HideKeyboard()
        {
            var manager = (InputMethodManager)Application.Context.GetSystemService(Context.InputMethodService);
            manager.HideSoftInputFromWindow(EditText.WindowToken, 0);
        }

        private void SetFontAttributesSizeAndFamily()
        {
            Typeface tf = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Medium.ttf");

            EditText.Typeface = Control.Typeface = tf;
            EditText.SetTextSize(ComplexUnitType.Sp, 14);
        }

        private void SetErrorText()
        {
            Control.Error = !string.IsNullOrEmpty(Element.ErrorText) ? Element.ErrorText : null;
        }

        private void SetIsEnabled()
        {
            EditText.Enabled = Element.IsEnabled;
        }

        private void SetInputType()
        {
            EditText.InputType = Element.Keyboard.ToInputType();
            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassText) == InputTypes.ClassText)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.TextVariationPassword;
            }
            if (Element.IsPassword && (EditText.InputType & InputTypes.ClassNumber) == InputTypes.ClassNumber)
            {
                EditText.TransformationMethod = new PasswordTransformationMethod();
                EditText.InputType = EditText.InputType | InputTypes.NumberVariationPassword;
            }
        }
    }
}