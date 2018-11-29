using System;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class ActivationPage : BasePage
    {
        private string Word = "";

        public ActivationPage()
        {
            InitializeComponent();

#if __ANDROID__
            backButton.TranslationX = -30;
#endif
            int randomIndex = new Random().Next(3, 10);
            Word = App.Locator.Profile.Mnemonic.Split(' ')[randomIndex];

            int wordIndex = randomIndex + 1;

            entryWord.Placeholder = entryWord.Placeholder.Replace("<number>", wordIndex.ToString());
            wordLabel.Text = wordLabel.Text.Replace("<number>", wordIndex.ToString());

            App.Locator.DeviceService.setStausBarLight();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();
        }


        private void OnBack(object sender, EventArgs e)
        {
            Unfocus();

            Navigation.PopAsync(true);
        }

        private void OnCheck(object sender, EventArgs e)
        {
            errorLabel.IsVisible = false;

            if(entryWord.Text==Word){
                Unfocus();

				App.Locator.Profile.MnemonicGenerated = false;

                var page = new SMSVereficationPage();
                Navigation.PushAsync(page, true);
            }
            else{
                ShowMessage(AppResources.IncorrectMnemonic);

                errorLabel.IsVisible = true;

                entryWord.Focus();
            }
        }

        void Handle_TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            errorLabel.IsVisible = false;
        }
    }
}
