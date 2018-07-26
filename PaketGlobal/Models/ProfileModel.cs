using System;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public class ProfileModel : BaseViewModel
	{
        private bool tryToHack = false;

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
            var oldName = PaketUser;

			var userInfo = await App.Locator.FundServiceClient.UserInfos();
			if (userInfo != null) {
				Pubkey = userInfo.UserDetails.Pubkey;
				FullName = userInfo.UserDetails.FullName;
				PaketUser = userInfo.UserDetails.PaketUser;
				PhoneNumber = userInfo.UserDetails.PhoneNumber;
				Address = userInfo.UserDetails.Address;

                if(oldName!=null)
                {
                    PaketUser = oldName;
                }

                if (PaketUser==null){
                    var result = await App.Locator.FundServiceClient.GetUser(Pubkey, null);
                    if (result != null) {
                        PaketUser = result.UserDetails.Call_Sign;
                    }
                }
			}

            //HACK:
            if(Pubkey==null && tryToHack==false)
            {
                //Problems in the API.
                //When the user does not have information about the address, phone, name.
                //An api call does not return a public key.
                //You must add a public key in the response in any case, even if the user does not have KYC.
                //But i fixed this.
                //I made a small hack to get the public key, but I think you need to fix this on the server.
                //Now if we do not recieve a public key, i save the address, mail and name as blank lines (example: "") and request information again.

                //Sending request to: v2/user_infos, Method: POST
                //Status: OK, Content: {
                //"status": 200, 
                // "user_details": { }
                //}


                //After add info

                //Sending request to: v2/user_infos, Method: POST
                //Status: OK, Content: {
                //"status": 200, 
                //"user_details": {
                //"address": "Address1", 
                //"full_name": "Ddd", 
                //"phone_number": "123", 
                //"pubkey": "GDLM2LIZKQJW46DJ5USFXVXG47YDDPHCPEJ2TDKABOYKONLO73AFF5DL", 
                //"timestamp": "Thu, 26 Jul 2018 19:14:23 GMT"
                //}
                //}


                FullName = "";
                PhoneNumber = "";
                Address = "";

                bool save = await Save();

                if(save)
                {
                    tryToHack = true;

                    await Load();
                }
            }

		}

		public async Task<bool> Save()
		{
			var result = await App.Locator.FundServiceClient.UserInfos(FullName, PhoneNumber, Address);
			return result != null;
		}
	}
}
