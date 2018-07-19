using System;
using System.Threading.Tasks;

using stellar_dotnetcore_sdk;
using stellar_dotnetcore_sdk.xdr;
using static PaketGlobal.ServiceClient;

namespace PaketGlobal
{
	public class StellarHelper
	{
		static string horizon_url = "https://horizon-testnet.stellar.org";

        public static async Task<StellarOperationResult> LaunchPackage (KeyPair escrowKP, string recipientPubkey, long deadlineTimestamp, string courierPubkey, double paymentBuls, double collateralBuls)
		{
            var payment =  StellarConverter.ConvertBULToStroops(paymentBuls);

            if (StellarConverter.IsValidBUL(payment)==false)
            {
                throw new ServiceException(400, "You can't specify more then 7 fractional digits");
            }

            var collateral = StellarConverter.ConvertBULToStroops(collateralBuls);

            if (StellarConverter.IsValidBUL(collateral)==false)
            {
                throw new ServiceException(400, "You can't specify more then 7 fractional digits");
            }

			//Check launcher's balance
			var launcherBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (launcherBalance == null || launcherBalance.BalanceBUL < paymentBuls) {
				return StellarOperationResult.LowBULsLauncher;
			}

			//Check courier's balance
			var courierBalance = await App.Locator.ServiceClient.Balance(courierPubkey);
			if (courierBalance == null || launcherBalance.BalanceBUL < collateralBuls) {
				return StellarOperationResult.LowBULsCourier;
			}

			//Create escrow account
			var accountResult = await App.Locator.ServiceClient.PrepareCrateAccount(App.Locator.Profile.Pubkey, escrowKP.Address, 4);//Change to 20000200
			if (accountResult != null) {
				//Sign escrow account transaction
				var signedCreate = await SignTransaction(App.Locator.Profile.KeyPair, accountResult.CreateTransaction);
				if (signedCreate != null) {
					//Submit escrow account
					var submitCreate = await App.Locator.ServiceClient.SubmitTransaction(signedCreate);
					if (submitCreate != null) {
						//Add token trust to escrow account
						var trustResult = await AddTrustToken(escrowKP);
						if (trustResult) {
							//Prepare escrow
							var launchResult = await App.Locator.ServiceClient.PrepareEscrow(escrowKP.Address, App.Locator.Profile.Pubkey,
																							 recipientPubkey, deadlineTimestamp,
																							 courierPubkey, paymentBuls,
																							 collateralBuls, (d) => {
																								 return App.Locator.Profile.SignData(d, escrowKP);
																							 });
							if (launchResult != null) {
								//Sign options transaction
								var signedOptions = await SignTransaction(escrowKP, launchResult.SetOptionsTransaction);
								if (signedOptions != null) {
									//Submit options transaction
									var submitOptions = await App.Locator.ServiceClient.SubmitTransaction(signedOptions);
									if (submitOptions != null) {
										//Prepare send payment
										var paymentTrans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, escrowKP.Address, paymentBuls);
										if (paymentTrans != null) {
											//Sign payment transaction
											var signed = await SignTransaction(App.Locator.Profile.KeyPair, paymentTrans.Transaction);
											if (signed != null) {
												//Submit payment transaction
												var paymentResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
												if (paymentResult != null) {
													//var newLauncherBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);//TODO remove balance check
													//if (newLauncherBalance.BalanceBUL == launcherBalance.BalanceBUL - paymentBuls) {
														App.Locator.Profile.AddTransaction(escrowKP.Address, launchResult);//save payment transaction data
														return StellarOperationResult.Success;
													//}

													//return StellarOperationResult.IncositentBalance;
												}

												return StellarOperationResult.FailSendBuls;
											}

											return StellarOperationResult.FailSendBuls;
										}

										return StellarOperationResult.FailSendBuls;
									}

									return StellarOperationResult.FaileSubmitOptions;
								}

								return StellarOperationResult.FaileSubmitOptions;
							}

							return StellarOperationResult.FailedLaunchPackage;
						}

						return StellarOperationResult.FailAddTrust;
					}

					return StellarOperationResult.FailSubmitCreateAccount;
				}

				return StellarOperationResult.FailSubmitCreateAccount;
			}

			return StellarOperationResult.FailCreateAccount;


			//As the escrow account, call /prepare_escrow. Check balances

			//As the escrow account, sign and submit the set_options_transaction.

			//Call /bul_account on the escrow account and verify that the signers are properly set.

			//Make note of the BUL balances of the launcher by calling /bul_account.

			//Transfer the payment from the launcher to the escrow

			//Make note of the BUL balances of the launcher by calling /bul_account. It should be the same as before minus the payment
		}

		public static async Task<StellarOperationResult> AcceptPackageAsCourier(string escrowPubkey, long collateral, string paymentTransaction)
		{
			var courierBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (courierBalance == null || courierBalance.BalanceBUL < collateral) {
				return StellarOperationResult.LowBULsCourier;
			}

            var trans = await App.Locator.ServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, escrowPubkey, (collateral/10000000.0f));
			if (trans != null) {
				var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
				var paymentResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
				if (paymentResult != null) {
					var acceptResult = await App.Locator.ServiceClient.AcceptPackage(escrowPubkey);
					if (acceptResult != null) {
						//var newCourierBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);//TODO remove balance check
						//if (newCourierBalance.BalanceBUL == courierBalance.BalanceBUL - collateral) {
							App.Locator.Profile.AddTransaction(escrowPubkey, paymentTransaction);
							return StellarOperationResult.Success;
						//}

						//return StellarOperationResult.IncositentBalance;
					} else {
						return StellarOperationResult.FailAcceptPackage;
					}
				} else {
					return StellarOperationResult.FailSendCollateral;
				}
			} else {
				return StellarOperationResult.FailSendCollateral;
			}
		}

		public static async Task<StellarOperationResult> AcceptPackageAsRecipient(string escrowPubkey, string paymentTransaction)
		{
			//var courierBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);

			var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, paymentTransaction);//sign the payment transaction
			var submitResult = await App.Locator.ServiceClient.SubmitTransaction(signed);
			if (submitResult != null) {
				var result = await App.Locator.ServiceClient.AcceptPackage(escrowPubkey);//accept the package
				if (result != null) {
					return StellarOperationResult.Success;
				} else {
					return StellarOperationResult.FailAcceptPackage;
				}
			} else {
				return StellarOperationResult.FailAcceptPackage;
			}
		}

		public static async Task<bool> RefundEscrow(string refundTranscation, string mergeTransaction)
		{
			var refundResult = await App.Locator.ServiceClient.SubmitTransaction(refundTranscation);
			if (refundResult != null) {
				var mergeResult = await ReclaimEscrow(mergeTransaction);
				return mergeResult;
			}

			return false;
		}

		public static async Task<bool> ReclaimEscrow(string mergeTransaction)
		{
			var result = await App.Locator.ServiceClient.SubmitTransaction(mergeTransaction);
			return result != null;
		}

		public static async Task<bool> AddTrustToken(KeyPair kp, string trustorSeed = "SC2PO5YMP7VISFX75OH2DWETTEZ4HVZOECMDXOZIP3NBU3OFISSQXAEP")
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
			try {
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
			} catch (Exception ex) {
				System.Diagnostics.Debug.WriteLine(ex);
				return null;
			}
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
			var result = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);
			return result != null;
		}
	}
}
