using System;
using System.Threading.Tasks;

using stellar_dotnetcore_sdk;

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
				var asset = Asset.CreateNonNativeAsset(assetCode, trustor);
				var operation = new ChangeTrustOperation.Builder(asset, "922337203685").SetSourceAccount(kp).Build();

				var transaction = new Transaction.Builder(source).AddOperation(operation).Build();
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
	}
}
