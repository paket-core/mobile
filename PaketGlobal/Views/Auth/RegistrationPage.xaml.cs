using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class RegistrationPage : BasePage
    {
        Command LongMnemonicTapCommand;

        public RegisterViewModel ViewModel
        {
            get { return BindingContext as RegisterViewModel; }
        }

        public RegistrationPage()
        {
            InitializeComponent();
            AddLongTaps();

            BindingContext = new RegisterViewModel();

            //if (!String.IsNullOrWhiteSpace(App.Locator.Profile.Pubkey))
            //{
            //    if (App.Locator.Profile.UserName != null)
            //    {
            //        layoutLogin.IsVisible = false;
            //        layoutRegistration.IsVisible = false;
            //        layoutProvideInfo.IsVisible = false;
            //        layoutFundPrompt.IsVisible = true;
            //        mnemonicLabel.Text = App.Locator.Profile.Mnemonic;
            //     //   CheckActivation();
            //    }
            //    else
            //    {
            //        layoutLogin.IsVisible = false;
            //        layoutRegistration.IsVisible = false;
            //        layoutFundPrompt.IsVisible = false;
            //        layoutProvideInfo.IsVisible = true;
            //    }
            //}
        }


        private void AddLongTaps() {
            LongMnemonicTapCommand = new Command(() =>
            {
           //     App.Locator.ClipboardService.SendTextToClipboard(mnemonicLabel.Text);
            });

          //  XamEffects.Commands.SetLongTap(mnemonicLabel, LongMnemonicTapCommand);

        //    App.Navigation.PushAsync(new MainPage());
         //   App.Locator.NavigationService.NavigateTo("");
        }

    }
}
