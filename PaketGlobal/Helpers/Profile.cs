﻿using System;
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

		public string Seed {
			get { return App.Locator.AccountService.Seed; }
		}

		public string Mnemonic {
			get { return App.Locator.AccountService.Mnemonic; }
		}

		public bool Activated {
			get { return App.Locator.AccountService.Activated; }
			set { App.Locator.AccountService.Activated = value; }
		}

		public KeyPair KeyPair { get; set; }

		public string Pubkey {
			get { return KeyPair?.Address; }
		}

		public Profile()
		{
			TryRestoreKeyPair();
		}

		public void SetCredentials (string userName, string fullName, string phoneNumber, string seed, string mnemonic)
		{
			App.Locator.AccountService.SetCredentials(userName, fullName, phoneNumber, seed, mnemonic);
		}

		public void DeleteCredentials ()
		{
			KeyPair = null;
			App.Locator.AccountService.DeleteCredentials ();
		}

		private void TryRestoreKeyPair()
		{
			if (!String.IsNullOrWhiteSpace(Mnemonic)) {
				KeyPair = GenerateKeyPairFromMnemonic(Mnemonic).KeyPair;
			} else if (!String.IsNullOrWhiteSpace(Seed)) {
				KeyPair = GenerateKeyPairFromSeed(Seed).KeyPair;
			}
		}

		public static KeyData GenerateKeyPairFromMnemonic(string mnemonic = null)
		{
			//Restore seed from word list
			var mo = String.IsNullOrWhiteSpace(mnemonic) ? new Mnemonic(Wordlist.English, WordCount.Twelve)
						   : new Mnemonic(mnemonic, Wordlist.English);
			var extKey = mo.DeriveExtKey();
			//var seed = extKey.PrivateKey.ToBytes();
			var seed = StrKey.EncodeStellarSecretSeed(extKey.PrivateKey.ToBytes());
			var kd = GenerateKeyPair(seed, mo);
			return kd;
		}

		public static KeyData GenerateKeyPairFromSeed(string seed)
		{
			var kd = GenerateKeyPair(seed);
			return kd;
		}

		private static KeyData GenerateKeyPair(string seed, Mnemonic mo = null)
		{
			//Recover private key
			var kp = KeyPair.FromSecretSeed(seed);
			//var kp = KeyPair.FromSecretSeed("SDJGBJZMQ7Z4W3KMSMO2HYEV56DJPOZ7XRR7LJ5X2KW6VKBSLELR7MRQ");//Launcher
			//var kp = KeyPair.FromSecretSeed("SBOLPN4HNTCLA3BMRS6QG62PXZUFOZ5RRMT6LPJHUPGQLBP5PZY4YFIT");//Courier
			//var kp = KeyPair.FromSecretSeed("SA5OXLJ2JCX4PF3G5WKSG66CXJQXQFCT62NQJ747XET5E2FR6TVIE4ET");//Recepient

			return new KeyData { Mnemonic = mo, KeyPair = kp };
		}

		public string SignData(string data)
		{
			return SignData(data, KeyPair);
		}

		public string SignData(string data, KeyPair kp)
		{
			var bytes = Encoding.UTF8.GetBytes(data);
			var signed = kp.Sign(bytes);
			var result = Encoders.Base64.EncodeData(signed);
			return result;
		}

		public bool AddTransaction(string paketId, string paymentTranscation)
		{
			var transData = new LaunchPackageData {
				PaymentTransaction = paymentTranscation
			};
			return AddTransaction(paketId, transData);
		}

		public bool AddTransaction(string paketId, LaunchPackageData transactionData)
		{
			try {
				Dictionary<string, LaunchPackageData> transactions = null;
				var transData = App.Locator.AccountService.Transactions;

				if (transData != null) transactions = JsonConvert.DeserializeObject<Dictionary<string, LaunchPackageData>>(transData);
				else transactions = new Dictionary<string, LaunchPackageData>();

				if (!transactions.ContainsKey(paketId)) {
					transactions.Add(paketId, transactionData);
				}

				transData = JsonConvert.SerializeObject(transactions);
				App.Locator.AccountService.Transactions = transData;

				return true;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return false;
			}
		}


		public LaunchPackageData GetTransaction(string paketId)
		{
			try {
				var transData = App.Locator.AccountService.Transactions;

				if (transData == null) return null;

				var transactions = JsonConvert.DeserializeObject<Dictionary<string, LaunchPackageData>>(transData);

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
