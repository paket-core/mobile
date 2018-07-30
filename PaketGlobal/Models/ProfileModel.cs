using System;
using System.Threading.Tasks;

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

		public async Task Load()
		{
			var result = await App.Locator.FundServiceClient.GetUser(App.Locator.Profile.Pubkey, null);
			if (result != null) {
				PaketUser = result.UserDetails.PaketUser;
			}

			var userInfo = await App.Locator.FundServiceClient.UserInfos();
			if (userInfo != null) {
				FullName = userInfo.UserDetails.FullName;
				PhoneNumber = userInfo.UserDetails.PhoneNumber;
				Address = userInfo.UserDetails.Address;
			}
		}

		public async Task<bool> Save()
		{
			var result = await App.Locator.FundServiceClient.UserInfos(FullName, PhoneNumber, Address);
			return result != null;
		}
	}
}
