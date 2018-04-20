using System;
using System.Linq;

using Xamarin.Auth;

namespace PaketGlobal.iOS
{
	public class AccountService : IAccountService
	{
		public string UserName {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Username;
			}
		}

		public string PhoneNumber {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["PhoneNumber"];
			}
		}

		public string Pubkey {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["Pubkey"];
			}
		}

		public void SetCredentials(string userName, string phoneNumber, string pubkey)
		{
			if (!string.IsNullOrWhiteSpace(userName) && !string.IsNullOrWhiteSpace(phoneNumber) && !string.IsNullOrWhiteSpace(pubkey)) {
				var account = new Account {
					Username = userName
				};
				account.Properties.Add("PhoneNumber", phoneNumber);
				account.Properties.Add("Pubkey", pubkey);
				AccountStore.Create().Save(account, App.AppName);
			}
		}

		public void DeleteCredentials()
		{
			var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
			if (account != null) {
				AccountStore.Create().Delete(account, App.AppName);
			}
		}
	}
}
