using System;
using System.Linq;

using Xamarin.Auth;
using Xamarin.Forms;

namespace PaketGlobal.Droid
{
	public class AccountService : IAccountService
	{
		public string UserName {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Username;
			}
		}

		public string FullName {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["FullName"];
			}
		}

		public string PhoneNumber {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["PhoneNumber"];
			}
		}

		public string Pubkey {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["Pubkey"];
			}
		}

		public string Mnemonic {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["Mnemonic"];
			}
		}

		public string Transactions {
			get {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["Transactions"];
			}
			set {
				var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
				if (account != null) {
					if (account.Properties.ContainsKey("Transactions")) {
						account.Properties["Transactions"] = value;
					} else {
						account.Properties.Add("Transactions", value);
					}
				}
			}
		}

		public void SetCredentials(string userName, string fullName, string phoneNumber, string pubkey, string mnemonic)
		{
			if (!String.IsNullOrWhiteSpace(userName) && !String.IsNullOrWhiteSpace(fullName)
			    && !String.IsNullOrWhiteSpace(phoneNumber) && !String.IsNullOrWhiteSpace(pubkey)
			    && !String.IsNullOrWhiteSpace(mnemonic)) {
				var account = new Account {
					Username = userName
				};
				account.Properties.Add("FullName", fullName);
				account.Properties.Add("PhoneNumber", phoneNumber);
				account.Properties.Add("Pubkey", pubkey);
				account.Properties.Add("Mnemonic", mnemonic);
				AccountStore.Create(MainActivity.Instance).Save(account, App.AppName);
			}
		}

		public void DeleteCredentials()
		{
			var account = AccountStore.Create(MainActivity.Instance).FindAccountsForService(App.AppName).FirstOrDefault();
			if (account != null) {
				AccountStore.Create(MainActivity.Instance).Delete(account, App.AppName);
			}
		}
	}
}
