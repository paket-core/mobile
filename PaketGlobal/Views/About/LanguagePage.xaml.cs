using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace PaketGlobal
{
    public class Language
    {
        public string Name { get; set; }
        public bool Selected { get; set; }
        public string Code { get; set; }
    }

    public partial class LanguagePage : BasePage
    {
        private List<Language> Languages = new List<Language>();

        public LanguagePage()
        {
            InitializeComponent();

            var eng = new Language
            {
                Name = "English",
                Code = "en",
                Selected = false
            };

            var ru = new Language
            {
                Name = "Russian",
                Code = "ru",
                Selected = false
            };


            Languages.Add(eng);
            Languages.Add(ru);

            var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();

            bool isSelected = false;

            foreach(Language lang in Languages)
            {
                if(ci.Name == lang.Code)
                {
                    lang.Selected = true;
                    isSelected = true;
                }
            }

            if(!isSelected)
            {
                Languages[0].Selected = true;
            }

            LanguageListView.ItemsSource = Languages;

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else{
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;

            var item = e.SelectedItem as Language;

            var ci = new CultureInfo(item.Code);

            AppResources.Culture = ci;
            DependencyService.Get<ILocalize>().SetLocale(ci);

            App.Locator.Workspace.ChangeLanguage();
        }
    }
}
