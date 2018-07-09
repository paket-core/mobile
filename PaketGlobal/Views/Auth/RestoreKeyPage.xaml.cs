using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RestoreKeyPage : BasePage
    {
        public RestoreKeyPage()
        {
            InitializeComponent();
        }

        void OnGenerate(object sender, EventArgs e)
        {
            App.Locator.NavigationService.NavigateTo(Locator.RegistrationPage);
        }
    }
}
