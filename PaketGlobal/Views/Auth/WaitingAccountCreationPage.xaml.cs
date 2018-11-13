using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class WaitingAccountCreationPage : BasePage
    {
        public WaitingAccountCreationPage(bool isNeedWellcone = false, bool isRestore = false)
        {
            InitializeComponent();

            if(isNeedWellcone)
            {
                ImageWellcome.Opacity = 1;
                ImageWaiting.Opacity = 0;

                StackWellcome.Opacity = 1;
                StackWaiting.Opacity = 0;
            }

            if(isRestore)
            {
                WellcomeLabel.Text = AppResources.WellcomeRestoreMessage;
            }

            var onContinueCommand = new Command(() =>
            {
                App.Locator.Profile.Activated = true;

                var navigationPage = new NavigationPage(new MainPage());

                Application.Current.MainPage = navigationPage;
            });
            XamEffects.Commands.SetTap(NextButton, onContinueCommand);
        }

        public void GoBack()
        {
            Navigation.PopAsync(false);
        }

        public void OpenMainPage()
        {
            Animation();
        }

        private void OnContinue(object sender, System.EventArgs e)
        {
            App.Locator.Profile.Activated = true;
            App.Locator.AccountService.SetPubKey(App.Locator.Profile.Pubkey);

            var navigationPage = new NavigationPage(new MainPage());

            Application.Current.MainPage = navigationPage;
        }

        private void Animation()
        {
            ImageWellcome.FadeTo(1, 250);
            ImageWaiting.FadeTo(0, 250);

            StackWellcome.FadeTo(1, 250);
            StackWaiting.FadeTo(0, 250);
        }
    }
}
