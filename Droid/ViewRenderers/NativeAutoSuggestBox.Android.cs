#if __ANDROID__
using System;
using System.Collections.Generic;
using System.Linq;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Text;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;

namespace dotMorten.Xamarin.Forms
{
    /// <summary>
    ///  Extends AutoCompleteTextView to have similar APIs and behavior to UWP's AutoSuggestBox, which greatly simplifies wrapping it
    /// </summary>
    public class NativeAutoSuggestBox : AutoCompleteTextView
    {
        private bool suppressTextChangedEvent;
        private Func<object, string> textFunc;
        private SuggestCompleteAdapter adapter;

        public NativeAutoSuggestBox(Context context, Android.Util.IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle)
        {
            Init();
        }

        public NativeAutoSuggestBox(Context context, Android.Util.IAttributeSet attrs) : base(context,attrs)
        {
            Init();
        }

        public NativeAutoSuggestBox(Context context) : base(context)
        {
            Init();
        }

        private void Init()
        {
            SetMaxLines(1);
            Threshold = 0;

            ItemClick += OnItemClick;
            Adapter = adapter = new SuggestCompleteAdapter(Context, Android.Resource.Layout.SimpleDropDownItem1Line);

            //
            //setba

            SetPadding(0, this.PaddingTop, this.PaddingRight, this.PaddingBottom);
            CompoundDrawablePadding = 25;

            Typeface tf = Typeface.CreateFromAsset(Android.App.Application.Context.Assets, "Poppins-Medium.ttf");
            this.SetTypeface(tf, TypefaceStyle.Normal);
            this.SetTextSize(Android.Util.ComplexUnitType.Dip, 14);

            this.SetHintTextColor(Android.Graphics.Color.ParseColor("#A7A7A7"));
            this.SetLinkTextColor(Android.Graphics.Color.Black);
            this.SetTextColor(Android.Graphics.Color.Black);
            this.SetBackgroundColor(Android.Graphics.Color.Transparent);

            // this.SetHighlightColor(Android.Graphics.Color.Black);
            // this.Background.SetColorFilter(Android.Graphics.Color.Black, PorterDuff.Mode.SrcAtop);

            var result = new Android.Text.InputTypes();
            result = InputTypes.ClassText | InputTypes.TextFlagNoSuggestions;

            this.InputType = result;
        }

        public void SetItems(IEnumerable<object> items, Func<object, string> labelFunc, Func<object, string> textFunc)
        {
            this.textFunc = textFunc;
            if (items == null)
                adapter.UpdateList(Enumerable.Empty<string>(), labelFunc);
            else
                adapter.UpdateList(items.OfType<object>(), labelFunc);
        }

        public void MakeRequestFocus()
        {
            this.RequestFocus();

            InputMethodManager inputMethodManager = (InputMethodManager)Android.App.Application.Context.GetSystemService(Context.InputMethodService);
            inputMethodManager.ShowSoftInput(this, ShowFlags.Forced);
            inputMethodManager.ToggleSoftInput(ShowFlags.Forced, HideSoftInputFlags.ImplicitOnly);

        }

        public new string Text
        {
            get => base.Text;
            set
            {
                suppressTextChangedEvent = true;
                base.Text = value;
                suppressTextChangedEvent = false;
                this.TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs(AutoSuggestionBoxTextChangeReason.ProgrammaticChange));
            }
        }

        public void SetTextColor(global::Xamarin.Forms.Color color)
        {
            this.SetTextColor(global::Xamarin.Forms.Platform.Android.ColorExtensions.ToAndroid(color));
        }

        public string PlaceholderText
        {
            set => HintFormatted = new Java.Lang.String(value as string ?? "");
        }

        public bool IsSuggestionListOpen
        {
            set
            {
                if (value)
                    ShowDropDown();
                else
                    DismissDropDown();
            }
        }

        protected override void OnTextChanged(ICharSequence text, int start, int lengthBefore, int lengthAfter)
        {
            if (!suppressTextChangedEvent)
                this.TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs(AutoSuggestionBoxTextChangeReason.UserInput));
            base.OnTextChanged(text, start, lengthBefore, lengthAfter);
        }

        private void DismissKeyboard()
        {
            var imm = (Android.Views.InputMethods.InputMethodManager)Context.GetSystemService(Context.InputMethodService);
            imm.HideSoftInputFromWindow(WindowToken, 0);
        }

        private void OnItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            DismissKeyboard();
            var obj = adapter.GetObject(e.Position);
            suppressTextChangedEvent = true;
            base.Text = textFunc(obj);
            suppressTextChangedEvent = false;
            TextChanged?.Invoke(this, new AutoSuggestBoxTextChangedEventArgs(AutoSuggestionBoxTextChangeReason.SuggestionChosen));
            SuggestionChosen?.Invoke(this, new AutoSuggestBoxSuggestionChosenEventArgs(obj));
            QuerySubmitted?.Invoke(this, new AutoSuggestBoxQuerySubmittedEventArgs(Text, obj));
        }

        public override void OnEditorAction([GeneratedEnum] ImeAction actionCode)
        {
            if (actionCode == ImeAction.Done || actionCode == ImeAction.Next)
            {
                DismissDropDown();
                DismissKeyboard();
                QuerySubmitted?.Invoke(this, new AutoSuggestBoxQuerySubmittedEventArgs(Text, null));
            }
            else
                base.OnEditorAction(actionCode);
        }

        protected override void ReplaceText(ICharSequence text)
        {
            //Override to avoid updating textbox on itemclick. We'll do this later using TextMemberPath and raise the proper TextChanged event then
        }

        public new event EventHandler<AutoSuggestBoxTextChangedEventArgs> TextChanged;

        public event EventHandler<AutoSuggestBoxQuerySubmittedEventArgs> QuerySubmitted;

        public event EventHandler<AutoSuggestBoxSuggestionChosenEventArgs> SuggestionChosen;

        private class SuggestCompleteAdapter : ArrayAdapter, IFilterable
        {
            private SuggestFilter filter = new SuggestFilter();
            private List<object> resultList;
            private Func<object, string> labelFunc;

            public SuggestCompleteAdapter(Context context, int textViewResourceId) : base(context, textViewResourceId)
            {
                resultList = new List<object>();
                SetNotifyOnChange(true);
            }

            public void UpdateList(IEnumerable<object> list, Func<object, string> labelFunc)
            {
                this.labelFunc = labelFunc;
                resultList = list.ToList();
                filter.SetFilter(resultList.Select(s=>labelFunc(s)));
                NotifyDataSetChanged();
            }

            public override int Count
            {
                get
                {
                    return resultList.Count;
                }
            }

            public override Filter Filter => filter;

            public override Java.Lang.Object GetItem(int position)
            {
                return labelFunc(GetObject(position));
            }

            public object GetObject(int position)
            {
                return resultList[position];
            }

            public override long GetItemId(int position)
            {
                return base.GetItemId(position);
            }

            private class SuggestFilter : Filter
            {
                private IEnumerable<string> resultList;

                public SuggestFilter()
                {
                }
                public void SetFilter(IEnumerable<string> list)
                {
                    resultList = list;
                }
                protected override FilterResults PerformFiltering(ICharSequence constraint)
                {
                    if (resultList == null)
                        return new FilterResults() { Count = 0, Values = null };
                    var arr = resultList.ToArray();
                    return new FilterResults() { Count = arr.Length, Values = arr };
                }
                protected override void PublishResults(ICharSequence constraint, FilterResults results)
                {
                }
            }
        }
    }
}
#endif