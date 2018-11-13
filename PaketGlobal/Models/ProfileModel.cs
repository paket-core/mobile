using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class ProfileModel : BaseViewModel
	{
		private string fullName;
		private string paketUser;
		private string phoneNumber;
		private string address;

		public string FullName {
			get { return fullName; }
			set { SetProperty(ref fullName, value); }
		}

		public string PaketUser {
			get { return paketUser; }
			set { SetProperty(ref paketUser, value); }
		}

		public string PhoneNumber {
			get { return phoneNumber; }
			set { SetProperty(ref phoneNumber, value); }
		}

		public string Address {
			get { return address; }
			set { SetProperty(ref address, value); }
		}

        public string StoredPhoneNumber
        {
            get{

                object fromStorage;

                if (Application.Current.Properties.ContainsKey(Constants.STORED_PHONE))
                {
                    Application.Current.Properties.TryGetValue(Constants.STORED_PHONE, out fromStorage);

                    return fromStorage as string;
                }

                return null;
            }
        }

		public async Task Load()
		{
            try{
                var result = await App.Locator.IdentityServiceClient.GetUser(App.Locator.Profile.Pubkey, null);
                if (result != null)
                {
                    PaketUser = result.UserDetails.PaketUser;
                }

                var userInfo = await App.Locator.IdentityServiceClient.UserInfos();
                if (userInfo != null)
                {
                    FullName = userInfo.UserDetails.FullName;
                    PhoneNumber = userInfo.UserDetails.PhoneNumber;
                    Address = userInfo.UserDetails.Address;
                    Application.Current.Properties[Constants.STORED_PHONE] = phoneNumber;
                }
            }
            catch (Exception ex){
                Console.WriteLine(ex);
            }
		}

		public async Task<bool> Save()
		{
			var result = await App.Locator.IdentityServiceClient.UserInfos(FullName, PhoneNumber, Address);
			return result != null;
		}
	}
}
