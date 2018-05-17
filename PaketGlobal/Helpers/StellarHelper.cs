using System;
using System.Threading.Tasks;

using stellar_dotnetcore_sdk;
using stellar_dotnetcore_sdk.xdr;

namespace PaketGlobal
{
	public class StellarHelper
	{
		static string horizon_url = "https://horizon-testnet.stellar.org";

		public static async Task<bool> AddTrustToken(KeyPair kp, string trustorSeed)
		{
			try {
				var server = new Server(horizon_url);
				var accResponse = await server.Accounts.Account(kp);
				var source = new Account(kp, accResponse.SequenceNumber);
				var trustor = KeyPair.FromSecretSeed(trustorSeed);

				const string assetCode = "BUL";
				var asset = stellar_dotnetcore_sdk.Asset.CreateNonNativeAsset(assetCode, trustor);
				var operation = new ChangeTrustOperation.Builder(asset, "922337203685").SetSourceAccount(kp).Build();

				var transaction = new stellar_dotnetcore_sdk.Transaction.Builder(source).AddOperation(operation).Build();
				transaction.Sign(source.KeyPair);

				var result = await server.SubmitTransaction(transaction);

				if (result != null) {
					System.Diagnostics.Debug.WriteLine(String.Format("Hash: {0}", result.Hash));
					System.Diagnostics.Debug.WriteLine(String.Format("EnvelopeXdr: {0}", result.EnvelopeXdr));
					System.Diagnostics.Debug.WriteLine(String.Format("ResultXdr: {0}", result.ResultXdr));
					return result != null && result.Hash != null;
				}

				return false;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return false;
			}
		}

		public static async Task<string> SignTransaction(KeyPair keyPair, string xdrData)
		{
			var bytes = Convert.FromBase64String(xdrData);
			var transEnv = TransactionEnvelope.Decode(new XdrDataInputStream(bytes));
			var sourceKP = KeyPair.FromXdrPublicKey(transEnv.Tx.SourceAccount.InnerValue);

			var server = new Server(horizon_url);
			var accResponse = await server.Accounts.Account(sourceKP);
			var source = new Account(sourceKP, accResponse.SequenceNumber);

			var builder = new stellar_dotnetcore_sdk.Transaction.Builder(source);
			foreach (var o in transEnv.Tx.Operations) {
				var operation = PaymentOperation.FromXdr(o);
				builder.AddOperation(operation);
			}

			if (transEnv.Tx.Memo != null && transEnv.Tx.Memo.Text != null) {
				var m = stellar_dotnetcore_sdk.Memo.Text(transEnv.Tx.Memo.Text);
				builder.AddMemo(m);
			}

			var trans = builder.Build();
			trans.Sign(keyPair);
			var signedXdrData = trans.ToEnvelopeXdrBase64();

			return signedXdrData;
		}

		public static async Task<bool> CheckAccountCreated(KeyPair kp)
		{
			try {
				var server = new Server(horizon_url);
				var accResponse = await server.Accounts.Account(kp);
				return accResponse != null;
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return false;
			}
		}

		public static async Task<bool> CheckTokenTrusted()
		{
			var result = await App.Locator.ServiceClient.Balance();
			return result != null;
		}
	}
}
