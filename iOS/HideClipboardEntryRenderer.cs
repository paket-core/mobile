using DisableClipboardOperationsdemo.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using ObjCRuntime;
using Foundation;
using PaketGlobal;

[assembly: ExportRenderer(typeof(HideClipboardEntry), typeof(HideClipboardEntryRenderer))]
namespace DisableClipboardOperationsdemo.iOS
{
    public class HideClipboardEntryRenderer : EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
        }

        public override bool CanPerform(Selector action, NSObject withSender)
        {
            if (action == new ObjCRuntime.Selector("paste:") || action == new ObjCRuntime.Selector("cut:"))
                return false;

            return base.CanPerform(action, withSender);
        }

        public override NSObject GetTargetForAction(Selector action, NSObject sender)
        {
            if (action == new Selector("paste:") || action == new Selector("cut:"))
                return null;
            
            return base.GetTargetForAction(action, sender);
        }

   
    }
}