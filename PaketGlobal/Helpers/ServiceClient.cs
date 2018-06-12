using System;
using System.Net;
using System.Text;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;

namespace PaketGlobal
{
	public class ServiceClient
	{
		public readonly string apiVersion;

		public delegate string PubKeyHandler();
		public delegate string SignHandler(string data);

		string restUrl;
		string customUrl;

		readonly RestClient restClient;

		ServiceStackSerializer _serializer;

		public PubKeyHandler TryGetPubKey { get; set; }
		public SignHandler TrySign { get; set; }

		public ServiceClient(string url, string version, string custom = null)
		{
			restUrl = url;
			apiVersion = version;
			customUrl = custom;
			_serializer = new ServiceStackSerializer();

			restClient = new RestClient(url);
			restClient.UserAgent = "PaketGlobal";
			restClient.Timeout = 60 * 1000;
			restClient.AddHandler("application/json", _serializer);
		}

		#region API Methods

		#region User

		public async Task<UserData> RegisterUser(string paketUser, string fullName, string phoneNumber, string pubkey)
		{
			//pubkey = "debug";//TODO for Debug purposes

			var request = PrepareRequest(apiVersion + "/create_user", Method.POST);

			request.AddParameter("call_sign", paketUser);
			//request.AddParameter("full_name", fullName);
			//request.AddParameter("phone_number", phoneNumber);

			return await SendRequest<UserData>(request, pubkey);
		}

		public async Task<CreateStellarAccountData> CreateStellarAccount(PaymentCurrency currency)
		{
			//pubkey = "debug";//TODO for Debug purposes

			var request = PrepareRequest(apiVersion + "/create_stellar_account", Method.POST);

			request.AddParameter("payment_currency", currency.ToString());

			return await SendRequest<CreateStellarAccountData>(request);
		}

		public async Task<UserData> GetUser(string pubkey)
		{
			//pubkey = "debug";//TODO for Debug purposes

			var request = PrepareRequest(apiVersion + "/get_user", Method.POST);

			request.AddParameter("pubkey", pubkey);

			return await SendRequest<UserData>(request, signData: false);
		}

		public async Task<UserData> UserInfos()
		{
			return await UserInfos(null, null, null);
		}

		public async Task<UserData> UserInfos(string fullName, string phoneNumber, string address)
		{
			//pubkey = "debug";//TODO for Debug purposes

			var request = PrepareRequest(apiVersion + "/user_infos", Method.POST);

			if (fullName != null) request.AddParameter("full_name", fullName);
			if (phoneNumber != null) request.AddParameter("phone_number", phoneNumber);
			if (address != null) request.AddParameter("address", address);

			return await SendRequest<UserData>(request);
		}

		public async Task<PrefundData> FundTestUser(string pubkey)
		{
			var client = new RestClient(customUrl);
			client.UserAgent = "PaketGlobal";
			client.Timeout = 60 * 1000;
			client.AddHandler("application/json", _serializer);
			client.AddHandler("application/hal+json", _serializer);

			var request = PrepareRequest("", Method.POST);

			request.AddParameter("addr", pubkey);

			return await SendRequest<PrefundData>(request, pubkey, signData: false, customClient: client);
		}

		#endregion User

		#region Wallet

		public async Task<BalanceData> Balance(string pubkey)
		{
			var request = PrepareRequest(apiVersion + "/bul_account", Method.POST);

			request.AddParameter("queried_pubkey", pubkey);

			return await SendRequest<BalanceData>(request, signData: false);
		}

		public async Task<PriceData> Price()
		{
			var request = PrepareRequest(apiVersion + "/price", Method.POST);

			return await SendRequest<PriceData>(request, signData: false);
		}

		public async Task<PurchaseTokensData> PurchaseBULs(long euroCents, PaymentCurrency paymentCurrency)
		{
			var request = PrepareRequest(apiVersion + "/purchase_bul", Method.POST);

			request.AddParameter("euro_cents", euroCents);
			request.AddParameter("payment_currency", paymentCurrency.ToString());

			return await SendRequest<PurchaseTokensData>(request);
		}

		public async Task<PurchaseTokensData> PurchaseXLMs(long euroCents, PaymentCurrency paymentCurrency)
		{
			var request = PrepareRequest(apiVersion + "/purchase_xlm", Method.POST);

			request.AddParameter("euro_cents", euroCents);
			request.AddParameter("payment_currency", paymentCurrency.ToString());

			return await SendRequest<PurchaseTokensData>(request);
		}

		public async Task<SubmitTransactionData> SendBuls(string toPubkey, long amountBuls)
		{
			var request = PrepareRequest(apiVersion + "/send_buls", Method.POST);

			request.AddParameter("to_pubkey", toPubkey);
			request.AddParameter("amount_buls", amountBuls);

			return await SendRequest<SubmitTransactionData>(request);
		}

		public async Task<SendBulsData> PrepareSendBuls(string fromPubkey, string toPubkey, long amountBuls)
		{
			var request = PrepareRequest(apiVersion + "/prepare_send_buls", Method.POST);

			request.AddParameter("from_pubkey", fromPubkey);
			request.AddParameter("to_pubkey", toPubkey);
			request.AddParameter("amount_buls", amountBuls);

			return await SendRequest<SendBulsData>(request);
		}

		public async Task<SubmitTransactionData> SubmitTransaction(string signedTrans)
		{
			var request = PrepareRequest(apiVersion + "/submit_transaction", Method.POST);

			request.AddParameter("transaction", signedTrans);

			return await SendRequest<SubmitTransactionData>(request);
		}

		public async Task<WalletPubkeyData> WalletPubkey()
		{
			var request = PrepareRequest(apiVersion + "/wallet_pubkey", Method.POST);

			return await SendRequest<WalletPubkeyData>(request);
		}

		public async Task<PrepareCreateAccountData> PrepareCrateAccount(string fromPubkey, string newPubkey, int startingBalance)
		{
			var request = PrepareRequest(apiVersion + "/prepare_create_account", Method.POST);

			request.AddParameter("from_pubkey", fromPubkey);
			request.AddParameter("new_pubkey", newPubkey);
			request.AddParameter("starting_balance", startingBalance);

			return await SendRequest<PrepareCreateAccountData>(request);
		}

		#endregion Wallet

		#region Packages

		public async Task<AcceptPackageData> AcceptPackage(string escrowPubkey)
		{
			var request = PrepareRequest(apiVersion + "/accept_package", Method.POST);

			request.AddParameter("escrow_pubkey", escrowPubkey);

			return await SendRequest<AcceptPackageData>(request);
		}

		public async Task<LaunchPackageData> PrepareEscrow(string escrowPubkey, string launcherPubkey, string recipientPubkey, long deadlineTimestamp, string courierPubkey, long paymentBuls, long collateralBuls, SignHandler customSign)
		{
			var request = PrepareRequest(apiVersion + "/prepare_escrow", Method.POST);

			request.AddParameter("launcher_pubkey", launcherPubkey);
			request.AddParameter("recipient_pubkey", recipientPubkey);
			request.AddParameter("deadline_timestamp", deadlineTimestamp);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);
			request.AddParameter("collateral_buls", collateralBuls);

			return await SendRequest<LaunchPackageData>(request, escrowPubkey, customSign: customSign);
		}

		public async Task<PackagesData> MyPackages(bool showInactive = false, DateTime? fromDate = null)
		{
			var request = PrepareRequest(apiVersion + "/my_packages", Method.POST);

			//request.AddParameter("show_inactive", showInactive);
			//if (fromDate.HasValue) {
			//	var ut = DateTimeHelper.ToUnixTime(fromDate.Value);
			//	request.AddParameter("from_date", ut.ToString());
			//}

			return await SendRequest<PackagesData>(request);
		}

		public async Task<PackageData> Package(string escrowPubkey)
		{
			var request = PrepareRequest(apiVersion + "/package", Method.POST);

			request.AddParameter("escrow_pubkey", escrowPubkey);

			return await SendRequest<PackageData>(request);
		}

		public async Task<RelayPackageData> RelayPackage(string paketId, string courierPubkey, int paymentBuls)
		{
			var request = PrepareRequest(apiVersion + "/relay_package", Method.POST);

			request.AddParameter("paket_id", paketId);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);

			return await SendRequest<RelayPackageData>(request);
		}

		#endregion Packages

		#endregion API Methods

		#region Client Methods

		private void SignRequest(RestRequest request, string pubkey, RestClient client, bool includePubkey = true, SignHandler customSign = null)
		{
			StringBuilder fingerprint = new StringBuilder();
			fingerprint.Append(System.IO.Path.Combine(client.BaseUrl.AbsoluteUri + request.Resource));

			foreach (var p in request.Parameters) {
				fingerprint.AppendFormat(",{0}={1}", p.Name, p.Value);
			}

			fingerprint.AppendFormat(",{0}", DateTimeHelper.ToMilliUnixTime(DateTime.UtcNow));
			request.AddHeader("Fingerprint", fingerprint.ToString());

			var signHandler = customSign ?? TrySign;
			var signature = signHandler?.Invoke(fingerprint.ToString());
			if (signature != null) request.AddHeader("Signature", signature);

			if (includePubkey) {
				pubkey = pubkey ?? TryGetPubKey?.Invoke();
				if (pubkey != null) request.AddHeader("Pubkey", pubkey);
			}
		}

		private RestRequest PrepareRequest(string url, Method method)
		{
			System.Diagnostics.Debug.WriteLine("Sending request to: {0}, Method: {1}", url, method);
			var request = new RestRequest(url);
			request.Method = method;
			request.JsonSerializer = _serializer;
			return request;
		}

		private async Task<T> SendRequest<T>(RestRequest request, string pubkey = null, bool signData = true, RestClient customClient = null, SignHandler customSign = null, RawBytes rb = null, System.IO.Stream responseStream = null, bool preferSSL = false, bool suppressUnAuthorized = false, bool suppressNoConnection = false, bool suppressServerErrors = false)
		{
			var client = customClient ?? restClient;

			if (signData) SignRequest(request, pubkey, client, customSign: customSign);

			try {
				IRestResponse<T> response;

				if (responseStream != null) {
					request.ResponseWriter = (obj) => {
						obj.CopyTo(responseStream);
						obj.Close();
					};
					response = await client.ExecuteTaskAsync<T>(request);
					responseStream.Dispose();
				} else {
					response = await client.ExecuteTaskAsync<T>(request);
					if (rb != null) {
						rb.Data = response.RawBytes;
					}
				}

				System.Diagnostics.Debug.WriteLine("Status: {0}, Content: {1}", response.StatusCode, response.Content);
				ServiceStackSerializer.HandleStatusCode(response);
				return response.Data;
			} catch (WebException e) {
				System.Diagnostics.Debug.WriteLine("SendRequest to: {0} Error: {1}", client.BaseUrl, e.ToString());
				if (!suppressNoConnection)
					App.Locator.Workspace.OnConnectionError(EventArgs.Empty);
			} catch (ServiceException e) {
				System.Diagnostics.Debug.WriteLine(e);
				if (!suppressServerErrors)
					App.Locator.Workspace.OnServiceError(new ServiceErrorEventArgs(e));
			} catch (UnAuthorizedException e) {
				//Profile.DeleteCredentials ();
				if (!suppressUnAuthorized)
					App.Locator.Workspace.OnAuthenticationRequired(e);
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e);
				//if (!suppressNoConnection)
					//_workspace.OnConnectionError(EventArgs.Empty);
			}

			return default(T);
		}

		#endregion Client Methods

		public class ServiceStackSerializer : RestSharp.Deserializers.IDeserializer, RestSharp.Serializers.ISerializer
		{
			public string RootElement { get; set; }
			public string Namespace { get; set; }
			public string DateFormat { get; set; }

			public string ContentType {
				get {
					return "application/json";
				}
				set {
					throw new NotImplementedException();
				}
			}

			public ServiceStackSerializer()
			{
				
			}

			public static void HandleStatusCode(IRestResponse response)
			{
				if (response.StatusCode == HttpStatusCode.Unauthorized)
					throw new UnAuthorizedException(response.Content);
				//if (response.StatusCode == HttpStatusCode.Forbidden)
					//throw new ServiceException((int)response.StatusCode, response.Content);
				if (response.StatusCode == HttpStatusCode.NotFound)
					throw new ServiceException((int)response.StatusCode, response.Content);//, ApiErrorCode.NotFound);
				if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created) {
					var error = JsonConvert.DeserializeObject<ErrorReply>(response.Content);
					if (error == null)// || error.Code == ApiErrorCode.Default)
						throw new ServiceException((int)response.StatusCode, response.Content);
					else
						throw new ServiceException((int)response.StatusCode, error.Message);//, error.Code);
				}
			}

			public T Deserialize<T>(IRestResponse response)
			{
				try {
					var result = JsonConvert.DeserializeObject<T>(response.Content);
					return result;
				} catch (Exception ex) {
					System.Diagnostics.Debug.WriteLine(ex);
					return default(T);
				}
			}

			public string Serialize(object obj)
			{
				var result = JsonConvert.SerializeObject(obj);
				return result;
			}
		}

		public class UnAuthorizedException : Exception
		{
			public UnAuthorizedException(String message) : base(message)
			{

			}
		}

		public class ForbiddenException : Exception
		{
			public ForbiddenException(String message) : base(message)
			{

			}
		}

		public class ServiceException : Exception
		{
			//public ApiErrorCode ApiErrorCode { get; set; }
			public int StatusCode { get; set; }

			//public ServiceException(int statusCode, String message, ApiErrorCode apiErrorCode) : base(message)
			//{
			//	StatusCode = statusCode;
			//	ApiErrorCode = apiErrorCode;
			//}

			public ServiceException(int statusCode, String message) : base(message)
			{
				StatusCode = statusCode;
			}

			public override string ToString()
			{
				return string.Format("[ServiceException: StatusCode={0} Message={1}]", StatusCode, Message);
			}
		}

		public class ServiceErrorEventArgs : EventArgs
		{
			public ServiceException ServiceException { get; set; }

			public ServiceErrorEventArgs(ServiceException e)
			{
				ServiceException = e;
			}
		}

		[DataContract]
		public class ErrorReplyWrapper
		{
			[DataMember(Name = "error")]
			public ErrorReply Error { get; set; }
		}

		[DataContract]
		public class ErrorReply
		{
			[DataMember(Name = "status")]
			public int Status { get; set; }
			[DataMember(Name = "error")]
			public string Message { get; set; }
			//[DataMember(Name = "errcode")]
			//public ApiErrorCode Code { get; set; }
		}
	}
}
