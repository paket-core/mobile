using System;
using UIKit;

namespace PaketGlobal.iOS
{
    public class ClipboardService : IClipboardService
    {
        public string GetTextFromClipboard() => UIPasteboard.General.String;
        public void SendTextToClipboard(string text) => UIPasteboard.General.String = text;
    }
}
