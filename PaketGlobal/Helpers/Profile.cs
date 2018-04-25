using System;
using NBitcoin;
using NBitcoin.DataEncoders;
using Stellar;

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
			var kp = KeyPair.FromRawSeed(seed);

			return new KeyData { Mnemonic = mo, KeyPair = kp };
		}

		public string SignData(string data)
		{
			var signed = KeyPair.Sign(data);
			var result = Encoders.Base64.EncodeData(signed);
			return result;
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
