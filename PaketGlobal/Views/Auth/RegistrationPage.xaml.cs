using System;
using System.Collections.Generic;

using Xamarin.Forms;


namespace PaketGlobal
{
    public partial class RegistrationPage : BasePage
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        public void YourFunctionToHandleMadTaps(Object sender, EventArgs ea)
        {
            Console.WriteLine("tap");
        }

    }
}
