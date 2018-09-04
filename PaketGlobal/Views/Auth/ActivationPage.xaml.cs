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

            entryWord.Placeholder = entryWord.Placeholder.Replace("5", wordIndex.ToString());
            wordLabel.Text = wordLabel.Text.Replace("5", wordIndex.ToString());

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

            Navigation.PopToRootAsync(true);
        }

        private void OnCheck(object sender, EventArgs e)
        {
            if(entryWord.Text==Word){
                Unfocus();

                var page = new SMSVereficationPage();
                Navigation.PushAsync(page, true);
            }
            else{
                entryWord.Focus();
            }
        }

    }
}
