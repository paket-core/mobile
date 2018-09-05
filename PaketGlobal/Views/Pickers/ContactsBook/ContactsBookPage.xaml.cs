using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.ContactService;
using libphonenumber;

namespace PaketGlobal
{
    public class BookContact : BaseViewModel
    {
        private string name;
        private string phone;
        private string contactPhoto;

        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        public string Phone
        {
            get { return phone; }
            set { SetProperty(ref phone, value); }
        }

        public string ContactPhoto
        {
            get { return contactPhoto; }
            set { SetProperty(ref contactPhoto, value); }
        }

        public ImageSource PhotoSource
        {
            get
            {
                return ImageSource.FromFile("empty_photo.png");
                //                if(ContactPhoto=="empty_photo.png")
                //                {
                //                    return ImageSource.FromFile(ContactPhoto);
                //                }
                //                else{
                //#if __IOS__
                //                    var uri = new Uri(ContactPhoto);
                //                    return ImageSource.FromUri(uri);
                //#elif __ANDROID__
                //                    var uri = Android.Net.Uri.Parse(new System.Uri(ContactPhoto).ToString());

                //                    // or when not in an activity (e.g. a service):
                //                    var stream = Android.App.Application.Context.ContentResolver.OpenInputStream(uri);

                //                    // eventually convert the stream to imagesource for consumption in Xamarin Forms:
                //                    var imagesource = Xamarin.Forms.ImageSource.FromStream(() => stream);

                //                    return imagesource;
                //#endif
                //    }
                //}
            }
        }

        public BookContact(string _name, string _phone, string _photo)
        {
            Name = _name;
            Phone = _phone;
            ContactPhoto = (_photo == null) ? "empty_photo.png" : _photo;
        }

        public string NationalPhone
        {
            get
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
                try
                {
                    PhoneNumber numberProto = phoneUtil.Parse(InternationalPhone, "");
                    var fPhone = numberProto.Format(PhoneNumberUtil.PhoneNumberFormat.NATIONAL);
                    return fPhone;
                }
                catch (NumberParseException)
                {
                    return "";
                }
            }
        }

        public string InternationalPhone
        {
            get{
                var tphone = Phone;
                if (!tphone.Contains("+"))
                {
                    tphone = "+" + tphone;
                }

                PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
                try
                {
                    PhoneNumber numberProto = phoneUtil.Parse(tphone,"");
                    var fPhone = numberProto.Format(PhoneNumberUtil.PhoneNumberFormat.E164);
                    return fPhone;
                }
                catch (NumberParseException)
                {
                    var fPhone = ISO3166.GetCurrentCallingCode() + tphone;
                    fPhone = fPhone.Replace(" ", "");
                    fPhone = fPhone.Replace("(", "");
                    fPhone = fPhone.Replace(")", "");
                    fPhone = fPhone.Replace("-", "");
                    return fPhone;
                }
            }
        }

        public string CountryCode
        {
            get
            {
                PhoneNumberUtil phoneUtil = PhoneNumberUtil.Instance;
                try
                {
                    PhoneNumber numberProto = phoneUtil.Parse(InternationalPhone, "");
                    return Convert.ToString(numberProto.CountryCode);
                }
                catch (NumberParseException)
                {
                    return null;
                }
            }
        }

        public string SimplePhone
        {
            get{
                var fPhone = Phone;
                fPhone = fPhone.Replace(" ", "");
                fPhone = fPhone.Replace("(", "");
                fPhone = fPhone.Replace(")", "");
                fPhone = fPhone.Replace("-", "");
                return fPhone;
            }
        }
    }

    public class ContactsBookPageEventArgs : EventArgs
    {
        private readonly BookContact item;

        public ContactsBookPageEventArgs(BookContact item)
        {
            this.item = item;
        }

        public BookContact Item
        {
            get { return this.item; }
        }
    }

    public partial class ContactsBookPage : BasePage
    {
        private List<BookContact> Items = new List<BookContact>();

        public delegate void ContactsBookPageEventHandler(object sender, ContactsBookPageEventArgs args);
        public ContactsBookPageEventHandler eventHandler;

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
                                var bookContact = new BookContact(contact.Name, contact.Number, contact.PhotoUriThumbnail);
                                Items.Add(bookContact); 
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

            Navigation.PopAsync();

            eventHandler(this, new ContactsBookPageEventArgs(e.SelectedItem as BookContact));
        }
    }
}
