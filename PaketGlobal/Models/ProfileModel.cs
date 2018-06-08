using System;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public class ProfileModel : BaseViewModel
	{
		private string pubkey;
		private string fullName;
		private string paketUser;
		private string phoneNumber;
		private string address;

		public string Pubkey {
			get { return pubkey; }
			set { SetProperty(ref pubkey, value); }
		}

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
			var userInfo = await App.Locator.FundServiceClient.UserInfos();
			if (userInfo != null) {
				Pubkey = userInfo.UserDetails.Pubkey;
				FullName = userInfo.UserDetails.FullName;
				PaketUser = userInfo.UserDetails.PaketUser;
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
