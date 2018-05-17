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

		public string FullName {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["FullName"];
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

		public string Mnemonic {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account?.Properties["Mnemonic"];
			}
		}

		public string Transactions {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account != null && account.Properties.ContainsKey("Transactions") ? account.Properties["Transactions"] : null;
			}
			set {
				var store = AccountStore.Create();
				var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
				if (account != null) {
					if (account.Properties.ContainsKey("Transactions")) {
						account.Properties["Transactions"] = value;
					} else {
						account.Properties.Add("Transactions", value);
					}
					store.Save(account, App.AppName);
				}
			}
		}

		public bool Activated {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account != null && account.Properties.ContainsKey("Activated") ? bool.Parse(account.Properties["Activated"]) : false;
			}
			set {
				var store = AccountStore.Create();
				var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
				if (account != null) {
					var b = value ? bool.TrueString : bool.FalseString;
					if (account.Properties.ContainsKey("Activated")) {
						account.Properties["Activated"] = b;
					} else {
						account.Properties.Add("Activated", b);
					}
					store.Save(account, App.AppName);
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
