using System;

using Android.Content;

using Xamarin.Forms;

namespace PaketGlobal.Droid
{
    public class ClipboardService : IClipboardService
    {
        public string GetTextFromClipboard()
        {
            var clipboardmanager = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);
            var item = clipboardmanager.PrimaryClip.GetItemAt(0);
            var text = item.Text;
            return text;
        }

        public void SendTextToClipboard(string text)
        {
            // Get the Clipboard Manager
            var clipboardManager = (ClipboardManager)Forms.Context.GetSystemService(Context.ClipboardService);

            // Create a new Clip
            var clip = ClipData.NewPlainText("", text);

            // Copy the text
            clipboardManager.PrimaryClip = clip;
        }
    }
}
