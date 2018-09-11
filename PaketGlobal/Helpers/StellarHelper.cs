using System;
using System.Threading.Tasks;

using stellar_dotnetcore_sdk;
using stellar_dotnetcore_sdk.responses;
using stellar_dotnetcore_sdk.xdr;

using static PaketGlobal.ServiceClient;

namespace PaketGlobal
{
    public class LaunchPackageEventArgs : EventArgs
    {
        private readonly string message;
        private readonly double progress;

        public LaunchPackageEventArgs(string message, double progress)
        {
            this.message = message;
            this.progress = progress;
        }

        public string Message
        {
            get { return this.message; }
        }

        public double Progress
        {
            get { return this.progress; }
        }
    }

	public class StellarHelper
	{
		static string horizon_url = "https://horizon-testnet.stellar.org";

        public delegate void LaunchPackageEventHandler(object sender,LaunchPackageEventArgs args);

        public static async Task<System.Collections.Generic.List<TransactionResponse>> GetTransactionsListFromAccount (KeyPair keyPair, int limit)
        {
            var server = new Server(horizon_url);

            var transactions = await server.Transactions
                                           .ForAccount(keyPair)
                                           .Limit(limit)
                                           .Order(stellar_dotnetcore_sdk.requests.OrderDirection.ASC)
                                           .Execute();

            return transactions.Records;
        }

		public static async Task<StellarOperationResult> CreatePackage(KeyPair escrowKP, string recipientPubkey, string launcherPhone, string recipientPhone, string description,
		                                                               string fromAddress, string toAddress, long deadlineTimestamp, double paymentBuls, double collateralBuls,
		                                                               string eventLocation, string fromLocation, string toLocation, byte[] packagePhoto, LaunchPackageEventHandler eventHandler)
		{
            if (toLocation.Length > 24)
            {
                toLocation = toLocation.Substring(0, 24);
            }

            if (fromLocation.Length > 24)
            {
                fromLocation = fromLocation.Substring(0, 24);
            }

			double steps = 3;
			double currentStep = 1;

			var payment = StellarConverter.ConvertBULToStroops(paymentBuls);

			if (StellarConverter.IsValidBUL(payment) == false) {
				throw new ServiceException(400, AppResources.FractionalDigitsError);
			}

			var collateral = StellarConverter.ConvertBULToStroops(collateralBuls);

			if (StellarConverter.IsValidBUL(collateral) == false) {
				throw new ServiceException(400, AppResources.FractionalDigitsError);
			}

			//Check launcher's balance
			eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep1, currentStep / steps));
			currentStep++;

			var launcherBalance = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (launcherBalance == null || launcherBalance.Account.BalanceBUL < payment) {
				return StellarOperationResult.LowBULsLauncher;
			}

			//Check courier's balance
			eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep2, currentStep / steps));
			currentStep++;

			var createResult = await App.Locator.RouteServiceClient.CreatePackage(escrowKP.Address, recipientPubkey, launcherPhone, recipientPhone, deadlineTimestamp, paymentBuls, collateralBuls,
			                                                                      description, fromAddress, toAddress, fromLocation, toLocation, eventLocation, packagePhoto, (d) => {
																					  return App.Locator.Profile.SignData(d, escrowKP);
																				  });
			if (createResult != null) {
				return StellarOperationResult.Success;
			}


			return StellarOperationResult.FailedLaunchPackage;
		}

        public static async Task<StellarOperationResult> LaunchPackage (KeyPair escrowKP, string recipientPubkey, long deadlineTimestamp, string courierPubkey, double paymentBuls, double collateralBuls, LaunchPackageEventHandler eventHandler)
		{
            double steps = 12;
            double currentStep = 1;

            var payment = paymentBuls; //StellarConverter.ConvertBULToStroops(paymentBuls);

            if (StellarConverter.IsValidBUL(payment)==false)
            {
                throw new ServiceException(400, AppResources.FractionalDigitsError);
            }

            var collateral = collateralBuls; //StellarConverter.ConvertBULToStroops(collateralBuls);

            if (StellarConverter.IsValidBUL(collateral)==false)
            {
                throw new ServiceException(400, AppResources.FractionalDigitsError);
            }

            //Check launcher's balance
            eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep1,currentStep / steps));

            currentStep++;

			var launcherBalance = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (launcherBalance == null || launcherBalance.Account.BalanceBUL < payment) {
				return StellarOperationResult.LowBULsLauncher;
			}

			//Check courier's balance
            eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep2, currentStep / steps));

            currentStep++;

			var courierBalance = await App.Locator.BridgeServiceClient.Balance(courierPubkey);
			if (courierBalance == null || courierBalance.Account.BalanceBUL < collateral) {
				return StellarOperationResult.LowBULsCourier;
			}

			//Create escrow account
            eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep3, currentStep / steps));

            currentStep++;

			var accountResult = await App.Locator.BridgeServiceClient.PrepareCrateAccount(App.Locator.Profile.Pubkey, escrowKP.Address, 4);//Change to 20000200
			if (accountResult != null) {
				//Sign escrow account transaction
                eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep4, currentStep / steps));

                currentStep++;

				var signedCreate = await SignTransaction(App.Locator.Profile.KeyPair, accountResult.CreateTransaction);
				if (signedCreate != null) {
					//Submit escrow account

                    eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep5, currentStep / steps));

                    currentStep++;

					var submitCreate = await App.Locator.BridgeServiceClient.SubmitTransaction(signedCreate);
					if (submitCreate != null) {
						//Add token trust to escrow account

                        eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep6, currentStep / steps));

                        currentStep++;

						var trustResult = await AddTrustToken(escrowKP);
						if (trustResult) {
							//Prepare escrow

                            eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep7, currentStep / steps));

                            currentStep++;

							var launchResult = await App.Locator.BridgeServiceClient.PrepareEscrow(escrowKP.Address, App.Locator.Profile.Pubkey,
							                                                                       recipientPubkey, deadlineTimestamp, courierPubkey,
																								   paymentBuls, collateralBuls, (d) => {
																									   return App.Locator.Profile.SignData(d, escrowKP);
																								   });
							if (launchResult != null) {
								var createResult = await App.Locator.RouteServiceClient.FinalizePackage(escrowKP.Address, launchResult.SetOptionsTransaction, launchResult.RefundTransaction,
																										launchResult.MergeTransaction, launchResult.PaymentTransaction, (d) => {
																											return App.Locator.Profile.SignData(d, escrowKP);
																										});
								if (createResult != null) {
									//Sign options transaction
									eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep8, currentStep / steps));

									currentStep++;

									var signedOptions = await SignTransaction(escrowKP, launchResult.SetOptionsTransaction);
									if (signedOptions != null) {
										//Submit options transaction

										eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep9, currentStep / steps));

										currentStep++;

										var submitOptions = await App.Locator.BridgeServiceClient.SubmitTransaction(signedOptions);
										if (submitOptions != null) {
											//Prepare send payment
											eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep10, currentStep / steps));

											currentStep++;

											var paymentTrans = await App.Locator.BridgeServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, escrowKP.Address, paymentBuls);
											if (paymentTrans != null) {
												//Sign payment transaction
												eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep11, currentStep / steps));

												currentStep++;

												var signed = await SignTransaction(App.Locator.Profile.KeyPair, paymentTrans.Transaction);
												if (signed != null) {
													//Submit payment transaction
													eventHandler("", new LaunchPackageEventArgs(AppResources.LaunchPackageStep12, currentStep / steps));

													currentStep++;

													var paymentResult = await App.Locator.BridgeServiceClient.SubmitTransaction(signed);
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

        public static async Task<StellarOperationResult> AssignPackage(string escrowPubkey, long collateral, string location)
        {
            var courierBalance = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
            if (courierBalance == null || courierBalance.Account.BalanceBUL < collateral)
            {
                return StellarOperationResult.LowBULsCourier;
            }

            var trans = await App.Locator.RouteServiceClient.AssignPackage(escrowPubkey, location);
            if(trans != null)
            {
                return StellarOperationResult.Success;
            }

            return StellarOperationResult.FailAcceptPackage;
        }

		public static async Task<StellarOperationResult> AcceptPackageAsCourier(string escrowPubkey, long collateral, string paymentTransaction, string location)
		{
			var courierBalance = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
			if (courierBalance == null || courierBalance.Account.BalanceBUL < collateral) {
				return StellarOperationResult.LowBULsCourier;
			}

			var trans = await App.Locator.BridgeServiceClient.PrepareSendBuls(App.Locator.Profile.Pubkey, escrowPubkey, (collateral/10000000.0f));
			if (trans != null) {
				var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, trans.Transaction);
				var paymentResult = await App.Locator.BridgeServiceClient.SubmitTransaction(signed);
				if (paymentResult != null) {
					var acceptResult = await App.Locator.RouteServiceClient.AcceptPackage(escrowPubkey,location);
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

		public static async Task<StellarOperationResult> AcceptPackageAsRecipient(string escrowPubkey, string paymentTransaction, string location)
		{
			//var courierBalance = await App.Locator.ServiceClient.Balance(App.Locator.Profile.Pubkey);

			var signed = await StellarHelper.SignTransaction(App.Locator.Profile.KeyPair, paymentTransaction);//sign the payment transaction
			var submitResult = await App.Locator.BridgeServiceClient.SubmitTransaction(signed);
			if (submitResult != null) {
				var result = await App.Locator.RouteServiceClient.AcceptPackage(escrowPubkey, location);//accept the package
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
			var refundResult = await App.Locator.BridgeServiceClient.SubmitTransaction(refundTranscation);
			if (refundResult != null) {
				var mergeResult = await ReclaimEscrow(mergeTransaction);
				return mergeResult;
			}

			return false;
		}

		public static async Task<bool> ReclaimEscrow(string mergeTransaction)
		{
			var result = await App.Locator.BridgeServiceClient.SubmitTransaction(mergeTransaction);
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
			var result = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);
			return result != null;
		}

        public static async Task<bool> CheckTokenTrustedWithPubKey(string pubKey)
        {
			var result = await App.Locator.BridgeServiceClient.Balance(pubKey);
            return result != null;
        }

        public static bool IsValidPubKey(string key)
        {
            return IsValid(StrKey.VersionByte.ACCOUNT_ID, key);
        }

        public static bool IsValid(StrKey.VersionByte versionByte, string encoded)         {
            if (encoded?.Length != 56)             {                 return false;             }              try             {                 var decoded = StrKey.DecodeCheck(versionByte, encoded);                 if (decoded.Length != 32)                 {                     return false;                 }             }             catch             {                 return false;             }             return true;         }
	}
}
