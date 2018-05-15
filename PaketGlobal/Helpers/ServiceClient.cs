﻿using System;
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
		public delegate string PubKeyHandler();
		public delegate string SignHandler(string data);

		string restUrl;
		string customUrl;

		readonly RestClient restClient;

		ServiceStackSerializer _serializer;

		public PubKeyHandler TryGetPubKey { get; set; }
		public SignHandler TrySign { get; set; }

		public ServiceClient(string url, string custom = null)
		{
			restUrl = url;
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

			var request = PrepareRequest("v2/register_user", Method.POST);

			request.AddParameter("paket_user", paketUser);
			request.AddParameter("full_name", fullName);
			request.AddParameter("phone_number", phoneNumber);

			return await SendRequest<UserData>(request, pubkey);
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

		public async Task<UserData> RecoverUser(string pubkey)
		{
			//pubkey = "debug";//TODO for Debug purposes

			var request = PrepareRequest("v2/recover_user", Method.POST);

			return await SendRequest<UserData>(request, pubkey);
		}

		#endregion User

		#region Wallet

		public async Task<BalanceData> Balance()
		{
			var request = PrepareRequest("v2/bul_account", Method.POST);

			var pubkey = TryGetPubKey?.Invoke();
			if (pubkey != null) request.AddParameter("queried_pubkey", pubkey);

			return await SendRequest<BalanceData>(request, signData: false);
		}

		public async Task<PriceData> Price()
		{
			var request = PrepareRequest("v2/price", Method.POST);

			return await SendRequest<PriceData>(request, signData: false);
		}

		public async Task<SubmitTransactionData> SendBuls(string toPubkey, long amountBuls)
		{
			var request = PrepareRequest("v2/send_buls", Method.POST);

			request.AddParameter("to_pubkey", toPubkey);
			request.AddParameter("amount_buls", amountBuls);

			return await SendRequest<SubmitTransactionData>(request);
		}

		public async Task<SendBulsData> PrepareSendBuls(string fromPubkey, string toPubkey, long amountBuls)
		{
			var request = PrepareRequest("v2/prepare_send_buls", Method.POST);

			request.AddParameter("from_pubkey", fromPubkey);
			request.AddParameter("to_pubkey", toPubkey);
			request.AddParameter("amount_buls", amountBuls);

			return await SendRequest<SendBulsData>(request);
		}

		public async Task<SubmitTransactionData> SubmitTransaction(string signedTrans)
		{
			var request = PrepareRequest("v2/submit_transaction", Method.POST);

			request.AddParameter("transaction", signedTrans);

			return await SendRequest<SubmitTransactionData>(request);
		}

		public async Task<WalletPubkeyData> WalletPubkey()
		{
			var request = PrepareRequest("v2/wallet_pubkey", Method.POST);

			return await SendRequest<WalletPubkeyData>(request);
		}

		#endregion Wallet

		#region Packages

		public async Task<AcceptPackageData> AcceptPackage(string paketId, string transaction = null)
		{
			var request = PrepareRequest("v2/accept_package", Method.POST);

			request.AddParameter("paket_id", paketId);
			if (transaction != null) request.AddParameter("payment_transaction", transaction);

			return await SendRequest<AcceptPackageData>(request);
		}

		public async Task<LaunchPackageData> LaunchPackage(string recipientPubkey, long deadlineTimestamp, string courierPubkey, long paymentBuls, long collateralBuls)
		{
			var request = PrepareRequest("v2/launch_package", Method.POST);

			request.AddParameter("recipient_pubkey", recipientPubkey);
			request.AddParameter("deadline_timestamp", deadlineTimestamp);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);
			request.AddParameter("collateral_buls", collateralBuls);

			return await SendRequest<LaunchPackageData>(request);
		}

		public async Task<PackagesData> MyPackages(bool showInactive = false, DateTime? fromDate = null)
		{
			var request = PrepareRequest("v2/my_packages", Method.POST);

			request.AddParameter("show_inactive", showInactive);
			if (fromDate.HasValue) {
				var ut = DateTimeHelper.ToUnixTime(fromDate.Value);
				request.AddParameter("from_date", ut.ToString());
			}

			return await SendRequest<PackagesData>(request);
		}

		public async Task<PackageData> Package(string paketId)
		{
			var request = PrepareRequest("v2/package", Method.POST);

			request.AddParameter("paket_id", paketId);

			return await SendRequest<PackageData>(request);
		}

		public async Task<RelayPackageData> RelayPackage(string paketId, string courierPubkey, int paymentBuls)
		{
			var request = PrepareRequest("v2/relay_package", Method.POST);

			request.AddParameter("paket_id", paketId);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);

			return await SendRequest<RelayPackageData>(request);
		}

		#endregion Packages

		#endregion API Methods

		#region Client Methods

		private void SignRequest(RestRequest request, string pubkey, RestClient client, bool includePubkey = true)
		{
			StringBuilder fingerprint = new StringBuilder();
			fingerprint.Append(System.IO.Path.Combine(client.BaseUrl.AbsoluteUri + request.Resource));

			foreach (var p in request.Parameters) {
				fingerprint.AppendFormat(",{0}={1}", p.Name, p.Value);
			}

			fingerprint.AppendFormat(",{0}", DateTimeHelper.ToMilliUnixTime(DateTime.Now));
			request.AddHeader("Fingerprint", fingerprint.ToString());

			var signature = TrySign?.Invoke(fingerprint.ToString());
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

		private async Task<T> SendRequest<T>(RestRequest request, string pubkey = null, bool signData = true, RestClient customClient = null, RawBytes rb = null, System.IO.Stream responseStream = null, bool preferSSL = false, bool suppressUnAuthorized = false, bool suppressNoConnection = false, bool suppressServerErrors = false)
		{
			var client = customClient ?? restClient;

			if (signData) SignRequest(request, pubkey, client);

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