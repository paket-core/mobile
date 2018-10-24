using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.ContactService;
using libphonenumber;
using System.Linq;

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
                    return fPhone.Trim();
                }
                catch (NumberParseException)
                {
                    var fPhone = ISO3166.GetCurrentCallingCode() + tphone;
                    fPhone = fPhone.Replace(" ", "");
                    fPhone = fPhone.Replace("(", "");
                    fPhone = fPhone.Replace(")", "");
                    fPhone = fPhone.Replace("-", "");
                    return fPhone.Trim();
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
                    var s = Convert.ToString(numberProto.CountryCode);
                    if(!s.Contains("+"))
                    {
                        s = "+" + s;
                    }
                    return s;
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
                return fPhone.Trim();
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
        private List<BookContact> SearchItems = new List<BookContact>();

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

            App.Locator.DeviceService.setStausBarLight();

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
                    App.ShowLoading(true);

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

                    var asc = Items.OrderBy(item => item.Name);
                    Items = asc.ToList();

                    ItemsListView.ItemsSource = Items;

                    App.ShowLoading(false);
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

        private void TextChanged(object sender, Xamarin.Forms.TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(e.NewTextValue))
            {
                ItemsListView.ItemsSource = Items;
            }
            else{
                ItemsListView.ItemsSource = Items.Where(x => x.Name.StartsWith(e.NewTextValue));  
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
