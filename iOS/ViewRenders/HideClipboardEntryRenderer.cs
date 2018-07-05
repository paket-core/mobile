﻿using DisableClipboardOperationsdemo.iOS;
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
        private UITapGestureRecognizer tapGestureRecognizer;

        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);

            if(Control!=null && tapGestureRecognizer==null) {
                tapGestureRecognizer = new UITapGestureRecognizer(HandleTap);
                Control.InputView = new UIView();
                Control.InputAccessoryView = new UIView();
                Control.AddGestureRecognizer(tapGestureRecognizer);
            }
        
        }


        private void HandleTap()
        {
            BecomeFirstResponder();

            var copyItem = new UIMenuItem("Copy", new Selector("Copy:"));

            var menu = UIMenuController.SharedMenuController;
            menu.MenuItems = new[] { copyItem };
            menu.SetTargetRect(this.Control.Bounds, this.Control);
            menu.SetMenuVisible(true, true);
        }

        public override bool CanBecomeFirstResponder { get { return true; } }

        public override bool CanPerform(Selector action, NSObject withSender)
        {
            return action.Name == "Copy:";
        }

        [Export("Copy:")]
        void Copy(UIMenuController controller)
        {
            UIPasteboard clipboard = UIPasteboard.General;
            clipboard.String = Control.Text;
        }

    }
}