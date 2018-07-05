using Android.Content;
using Android.Views;
using Android.Text;
using Android.Widget;

using PaketGlobal;
using PaketGlobal.Droid;

using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(HideClipboardEntry), typeof(HideClipboardEntryRenderer))]
namespace PaketGlobal.Droid
{
    public class HideClipboardEntryRenderer : EntryRenderer {

        public HideClipboardEntryRenderer(Context context) : base(context)
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            Control.CustomSelectionActionModeCallback = new Callback();
            Control.Enabled = true;
            Control.InputType = InputTypes.Null;
            Control.SetTextIsSelectable(true);
        }

        public class Callback : Java.Lang.Object, ActionMode.ICallback
        {
            public bool OnActionItemClicked(ActionMode mode, IMenuItem item)
            {
                return false;
            }
            public bool OnCreateActionMode(ActionMode mode, IMenu menu)
            {
                return true;
            }

            public void OnDestroyActionMode(ActionMode mode)
            { 
            
            }
           
            public bool OnPrepareActionMode(ActionMode mode, IMenu menu)
            {
                // Cut - 16908320
                // Copy - 16908321
                // Paste - 16908322
                // Share - 16908341

                if (menu != null)
                {
                    menu.RemoveItem(Android.Resource.Id.Cut);
                    menu.RemoveItem(Android.Resource.Id.Paste);
                }

                return true;
            }
        }
    }
}