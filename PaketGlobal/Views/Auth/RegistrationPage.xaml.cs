using System;
using System.Collections.Generic;

using Xamarin.Forms;
using XamEffects;

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
                Console.WriteLine("LONG TAP");
            });

            XamEffects.Commands.SetLongTap(MnemonicLabel, LongMnemonicTapCommand);
        }

    }
}
