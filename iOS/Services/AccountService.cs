using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using Newtonsoft.Json;
using Xamarin.Auth;

namespace PaketGlobal.iOS
{
    public class AccountService : IAccountService
    {
        public string UserName
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("PaketUser") ? account.Properties["PaketUser"] : null;
            }
        }

        public string FullName
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("FullName") ? account.Properties["FullName"] : null;
            }
        }

        public string PhoneNumber
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("PhoneNumber") ? account.Properties["PhoneNumber"] : null;
            }
        }

        public string Seed
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("Seed") ? account.Properties["Seed"] : null;
            }
        }

        public string Mnemonic
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("Mnemonic") ? account.Properties["Mnemonic"] : null;
            }
        }

        public string Transactions
        {
            get
            {
                var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
                return account != null && account.Properties.ContainsKey("Transactions") ? account.Properties["Transactions"] : null;
            }
            set
            {
                var store = AccountStore.Create();
                var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
                if (account != null)
                {
                    if (account.Properties.ContainsKey("Transactions"))
                    {
                        account.Properties["Transactions"] = value;
                    }
                    else
                    {
                        account.Properties.Add("Transactions", value);
                    }
                    store.Save(account, App.AppName);
                }
            }
        }

        public string ActivationAddress{
            get
            {
                string value = NSUserDefaults.StandardUserDefaults.StringForKey("ActivationAddress");

                if (value == null)
                {
                    return "";
                }

                return NSUserDefaults.StandardUserDefaults.StringForKey("ActivationAddress");
            }
            set
            {
                NSUserDefaults.StandardUserDefaults.SetString(value, "ActivationAddress");
                NSUserDefaults.StandardUserDefaults.Synchronize();
            } 
        }

        public bool ShowNotifications
        {
            get
            {
                string value = NSUserDefaults.StandardUserDefaults.StringForKey("Notifications"); 

                if (value==null)
                {
                    return true;
                }

                return NSUserDefaults.StandardUserDefaults.BoolForKey("Notifications");
            }
            set{
                NSUserDefaults.StandardUserDefaults.SetBool(value, "Notifications");
                NSUserDefaults.StandardUserDefaults.Synchronize();
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

		public bool MnemonicGenerated {
			get {
				var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
				return account != null && account.Properties.ContainsKey("MnemonicGenerated") ? bool.Parse(account.Properties["MnemonicGenerated"]) : false;
			}
			set {
				var store = AccountStore.Create();
				var account = store.FindAccountsForService(App.AppName).FirstOrDefault();
				if (account != null) {
					var b = value ? bool.TrueString : bool.FalseString;
					if (account.Properties.ContainsKey("MnemonicGenerated")) {
						account.Properties["MnemonicGenerated"] = b;
					} else {
						account.Properties.Add("MnemonicGenerated", b);
					}
					store.Save(account, App.AppName);
				}
			}
		}

		public void SetCredentials(string userName, string fullName, string phoneNumber, string address, string seed, string mnemonic)
		{
			if (!String.IsNullOrWhiteSpace(seed) || !String.IsNullOrWhiteSpace(mnemonic)) {
				var mnemonicGenerated = MnemonicGenerated;

				var account = new Account {
					Username = "PaketUser"
				};
				if (userName != null) account.Properties.Add("PaketUser", userName);
				if (fullName != null) account.Properties.Add("FullName", fullName);
				if (phoneNumber != null) account.Properties.Add("PhoneNumber", phoneNumber);
				if (address != null) account.Properties.Add("ActivationAddress", address);
				if (seed != null) account.Properties.Add("Seed", seed);
				if (mnemonic != null) account.Properties.Add("Mnemonic", mnemonic);

				account.Properties.Add("ShowNotifications", "true");

				AccountStore.Create().Save(account, App.AppName);

				MnemonicGenerated = mnemonicGenerated;
			}
		}

		public void DeleteCredentials()
		{
			var account = AccountStore.Create().FindAccountsForService(App.AppName).FirstOrDefault();
			if (account != null) {
				AccountStore.Create().Delete(account, App.AppName);
			}
		}

        public void SavePackages(List<Package> packages)
        {
            if(packages!=null)
            {
                var json = JsonConvert.SerializeObject(packages);
                NSUserDefaults.StandardUserDefaults.SetString(json, "packages");
                NSUserDefaults.StandardUserDefaults.Synchronize(); 
            }
 
        }
	}
}
