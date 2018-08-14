using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Android.Content;
using Android.Content.Res;
using Android.Support.Design.Widget;
using Android.Support.V7.Widget;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using PaketGlobal.Droid;
using PaketGlobal;

using Resource = Android.Resource;
using Android.Support.V4.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;

[assembly: ExportRenderer(typeof(XfxComboBox), typeof(XfxComboBoxRendererDroid))]

namespace PaketGlobal.Droid
{
    public class InstantAppCompatAutoCompleteTextView : AppCompatAutoCompleteTextView {

        public InstantAppCompatAutoCompleteTextView(Context context) : base(context)
        {
        }

        public override bool EnoughToFilter()
        {
            return true;
        }

        protected override void OnFocusChanged(bool gainFocus, [GeneratedEnum] FocusSearchDirection direction, Rect previouslyFocusedRect)
        {
            base.OnFocusChanged(gainFocus, direction, previouslyFocusedRect);

            if(gainFocus && Adapter!=null)
            {
                PerformFiltering(Text, 0);
            }
        }

    }

    public class XfxComboBoxRendererDroid : XfxEntryRendererDroid
    {
        public XfxComboBoxRendererDroid(Context context) : base(context)
        {
        }

        private InstantAppCompatAutoCompleteTextView AutoComplete => (InstantAppCompatAutoCompleteTextView)Control.EditText;

        protected override TextInputLayout CreateNativeControl()
        {
            var textInputLayout = new TextInputLayout(Context);
            var autoComplete = new InstantAppCompatAutoCompleteTextView(Context)
            {
                BackgroundTintList = ColorStateList.ValueOf(GetPlaceholderColor())
            };
            textInputLayout.AddView(autoComplete);
            return textInputLayout;
        }

        protected override void OnElementChanged(ElementChangedEventArgs<XfxEntry> e)
        {
            base.OnElementChanged(e);
            if (e.OldElement != null)
            {
                // unsubscribe
                AutoComplete.ItemClick -= AutoCompleteOnItemSelected;
                var elm = (XfxComboBox) e.OldElement;
                elm.CollectionChanged -= ItemsSourceCollectionChanged;
            }

            if (e.NewElement != null)
            {
                // subscribe
                SetItemsSource();
                SetThreshold();
                KillPassword();
                AutoComplete.ItemClick += AutoCompleteOnItemSelected;
                var elm = (XfxComboBox) e.NewElement;
                elm.CollectionChanged += ItemsSourceCollectionChanged;
            }

            AutoComplete.SetPadding(0, AutoComplete.PaddingTop, AutoComplete.PaddingRight, AutoComplete.PaddingBottom);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Entry.IsPasswordProperty.PropertyName)
                KillPassword();
            if (e.PropertyName == XfxComboBox.ItemsSourceProperty.PropertyName)
                SetItemsSource();
            else if (e.PropertyName == XfxComboBox.ThresholdProperty.PropertyName)
                SetThreshold();
        }

        private void AutoCompleteOnItemSelected(object sender, AdapterView.ItemClickEventArgs args)
        {
            var view = (AutoCompleteTextView) sender;
            var selectedItemArgs = new SelectedItemChangedEventArgs(view.Text);
            var element = (XfxComboBox) Element;
            element.OnItemSelectedInternal(Element, selectedItemArgs);
            HideKeyboard();
        }

        private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            var element = (XfxComboBox) Element;
            ResetAdapter(element);
        }

        private void KillPassword()
        {
            if (Element.IsPassword)
                throw new NotImplementedException("Cannot set IsPassword on a XfxComboBox");
        }

        private void ResetAdapter(XfxComboBox element)
        {
            var adapter = new XfxComboBoxArrayAdapter(Context,Android.Resource.Layout.SimpleDropDownItem1Line,
                element.ItemsSource.ToList(),
                element.SortingAlgorithm);
            AutoComplete.Adapter = adapter;
            adapter.NotifyDataSetChanged();
        }

        private void SetItemsSource()
        {
            var element = (XfxComboBox) Element;
            if (element.ItemsSource == null) return;

            ResetAdapter(element);
        }

        private void SetThreshold()
        {
            var element = (XfxComboBox) Element;
            AutoComplete.Threshold = element.Threshold;
        }
    }
}