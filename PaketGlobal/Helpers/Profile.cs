using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using NBitcoin;
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using stellar_dotnetcore_sdk;

namespace PaketGlobal
{
	public class Profile
	{
		public event EventHandler<EventArgs> Changed;

		public string UserName {
			get { return App.Locator.AccountService.UserName; }
		}

		public string FullName {
			get { return App.Locator.AccountService.FullName; }
		}

		public string PhoneNumber {
			get { return App.Locator.AccountService.PhoneNumber; }
		}

		public string Pubkey {
			get { return App.Locator.AccountService.Pubkey; }
		}

		public string Mnemonic {
			get { return App.Locator.AccountService.Mnemonic; }
		}

		public KeyPair KeyPair { get; set; }

		public Profile()
		{
			TryRestoreKeyPair();
		}

		public void SetCredentials (string userName, string fullName, string phoneNumber, string pubkey, string mnemonic)
		{
			App.Locator.AccountService.SetCredentials(userName, fullName, phoneNumber, pubkey, mnemonic);
		}

		public void DeleteCredentials ()
		{
			App.Locator.AccountService.DeleteCredentials ();
		}

		private void TryRestoreKeyPair()
		{
			if (!String.IsNullOrWhiteSpace(Pubkey) && !String.IsNullOrWhiteSpace(Mnemonic)) {
				KeyPair = GenerateKeyPair(Mnemonic).KeyPair;
			}
		}

		public static KeyData GenerateKeyPair(string mnemonic = null)
		{
			//Restore seed from word list
			var mo = String.IsNullOrWhiteSpace(mnemonic) ? new Mnemonic(Wordlist.English, WordCount.Twelve)
						   : new Mnemonic(mnemonic, Wordlist.English);
			var extKey = mo.DeriveExtKey();
			var seed = extKey.PrivateKey.ToBytes();

			//Recover private key
			var kp = KeyPair.FromSecretSeed(seed);
			//var kp = KeyPair.FromSecretSeed("SDJGBJZMQ7Z4W3KMSMO2HYEV56DJPOZ7XRR7LJ5X2KW6VKBSLELR7MRQ");//Launcher
			//var kp = KeyPair.FromSecretSeed("SBOLPN4HNTCLA3BMRS6QG62PXZUFOZ5RRMT6LPJHUPGQLBP5PZY4YFIT");//Courier
			//var kp = KeyPair.FromSecretSeed("SA5OXLJ2JCX4PF3G5WKSG66CXJQXQFCT62NQJ747XET5E2FR6TVIE4ET");//Recepient

			return new KeyData { Mnemonic = mo, KeyPair = kp };
		}

		public string SignData(string data)
		{
			var bytes = Encoding.UTF8.GetBytes(data);
			var signed = KeyPair.Sign(bytes);
			var result = Encoders.Base64.EncodeData(signed);
			return result;
		}

		public bool AddTransaction(string paketId, string trans)
		{
			try {
				Dictionary<string, string> transactions = null;
				var transData = App.Locator.AccountService.Transactions;

				if (transData != null) transactions = JsonConvert.DeserializeObject<Dictionary<string, string>>(transData);
				else transactions = new Dictionary<string, string>();

				if (!transactions.ContainsKey(paketId)) {
					transactions.Add(paketId, trans);
				}

				transData = JsonConvert.SerializeObject(transactions);
				App.Locator.AccountService.Transactions = transData;

				return true;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return false;
			}
		}


		public string GetTransaction(string paketId)
		{
			try {
				var transData = App.Locator.AccountService.Transactions;

				if (transData == null) return null;

				var transactions = JsonConvert.DeserializeObject<Dictionary<string, string>>(transData);

				if (transactions.ContainsKey(paketId)) {
					var trans = transactions[paketId];
					return trans;
				}

				return null;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return null;
			}
		}

		public bool RemoveTransaction(string paketId)
		{
			try {
				var transData = App.Locator.AccountService.Transactions;

				if (transData == null) return false;

				var transactions = JsonConvert.DeserializeObject<Dictionary<string, string>>(transData);

				if (transactions.ContainsKey(paketId)) {
					transactions.Remove(paketId);
					return false;
				}

				return false;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return false;
			}
		}

		protected virtual void OnChanged (EventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}

		public class KeyData
		{
			public Mnemonic Mnemonic { get; set; }
			public KeyPair KeyPair { get; set; }

			public string MnemonicString {
				get { return Mnemonic != null ? String.Join(" ", Mnemonic.Words) : null; }
			}
		}
	}
}
