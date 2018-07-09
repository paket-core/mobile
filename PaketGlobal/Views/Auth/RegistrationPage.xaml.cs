using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RegistrationPage : BasePage
    {
        Command LongMnemonicTapCommand;

        public RegistrationPage()
        {
       
            InitializeComponent();

            LongMnemonicTapCommand = new Command(() =>
            {
                App.Locator.ClipboardService.SendTextToClipboard(MnemonicLabel.Text);
            });

            XamEffects.Commands.SetLongTap(MnemonicLabel, LongMnemonicTapCommand);
        }

    }
}
