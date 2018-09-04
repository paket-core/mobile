using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.ContactService;

namespace PaketGlobal
{
    public class BookContact : BaseViewModel
    {
        private string contactPhoto;  

        public string ContactPhoto
        {
            get { return contactPhoto; }
            set { contactPhoto = value; }
        }
    }

    public partial class ContactsBookPage : BasePage
    {
        private List<Plugin.ContactService.Shared.Contact> Items = new List<Plugin.ContactService.Shared.Contact>();

        public ContactsBookPage()
        {
            InitializeComponent();

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else
            {
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif
        }

        protected async override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                await LoadContacts();
            }
        }

        private async Task LoadContacts()
        {
            try
            {
                var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Contacts);

                if (hasPermission)
                {
                    var contacts = await CrossContactService.Current.GetContactListAsync();

                    foreach(Plugin.ContactService.Shared.Contact contact in contacts)
                    {
                        if(contact.Numbers.Count>0)
                        {
                            if(!contact.Number.Contains("#"))
                            {
                                //var bookContact = new BookContact();
                                //bookContact.Name = contact.Name;
                                //bookContact.Number = contact.Number;
                                //bookContact.PhotoUri = contact.PhotoUri;
                                //bookContact.PhotoUriThumbnail = contact.PhotoUriThumbnail;

                                //if (bookContact.PhotoUriThumbnail != null)
                                //{
                                //    //var source = ImageSource.FromUri(new Uri(bookContact.PhotoUriThumbnail));
                                //    bookContact.ContactPhoto = bookContact.PhotoUriThumbnail;
                                //}
                                //else{
                                //    bookContact.ContactPhoto = "bul_icon.png";
                                //}

                                //Items.Add(contact); 
                            }
                        }
                    }

                    ItemsListView.ItemsSource = Items;
                }
                else
                {
                    ShowMessage(AppResources.ContactsAccessNotGranted);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
           
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
                return;
        }
    }
}
