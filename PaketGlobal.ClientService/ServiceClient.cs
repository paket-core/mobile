﻿using System;
using System.Net;
using System.Text;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using RestSharp.Portable;
using RestSharp.Portable.HttpClient;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace PaketGlobal.ClientService
{
	public class ServiceClient
	{
		public delegate string PubKeyHandler();

		string _url;

		readonly RestClient _client;

		ServiceStackSerializer _serializer;

		public PubKeyHandler TryGetPubKey { get; set; }

		public ServiceClient(string url)
		{
			_url = url;
			_serializer = new ServiceStackSerializer();

			_client = new RestClient(url);
			_client.UserAgent = "MyLobby";
			_client.Timeout = TimeSpan.FromSeconds(20.0);
			_client.AddHandler("application/json", _serializer);
		}

		#region API Methods

		#region User

		public async Task<UserData> RegisterUser(string paketUser, string fullName, string phoneNumber, string pubkey)
		{
			var request = PrepareRequest("/v1/register_user", Method.POST, pubkey);

			request.AddParameter("paket_user", paketUser);
			request.AddParameter("full_name", fullName);
			request.AddParameter("phone_number", phoneNumber);

			return await SendRequest<UserData>(request);
		}

		public async Task<UserData> RecoverUser(string pubkey)
		{
			var request = PrepareRequest("/v1/recover_user", Method.POST, pubkey);

			return await SendRequest<UserData>(request);
		}

		#endregion User

		#region Wallet

		public async Task<BalanceData> Balance()
		{
			var request = PrepareRequest("/v1/balance", Method.POST);

			return await SendRequest<BalanceData>(request);
		}

		public async Task<PriceData> Price()
		{
			var request = PrepareRequest("/v1/price", Method.POST);

			return await SendRequest<PriceData>(request);
		}

		public async Task<SendBulsData> SendBuls(string toPubkey, long amountBuls)
		{
			var request = PrepareRequest("/v1/send_buls", Method.POST);

			request.AddParameter("to_pubkey", toPubkey);
			request.AddParameter("amount_buls", amountBuls);

			return await SendRequest<SendBulsData>(request);
		}

		public async Task<WalletPubkeyData> WalletPubkey()
		{
			var request = PrepareRequest("/v1/wallet_pubkey", Method.POST);

			return await SendRequest<WalletPubkeyData>(request);
		}

		#endregion Wallet

		#region Packages

		public async Task<AcceptPackageData> AcceptPackage(string paketId)
		{
			var request = PrepareRequest("/v1/accept_package", Method.POST);

			request.AddParameter("paket_id", paketId);

			return await SendRequest<AcceptPackageData>(request);
		}

		public async Task<LaunchPackageData> LaunchPackage(string recipientPubkey, long deadlineTimestamp, string courierPubkey, long paymentBuls, long collateralBuls)
		{
			var request = PrepareRequest("/v1/launch_package", Method.POST);

			request.AddParameter("recipient_pubkey", recipientPubkey);
			request.AddParameter("deadline_timestamp", deadlineTimestamp);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);
			request.AddParameter("collateral_buls", collateralBuls);

			return await SendRequest<LaunchPackageData>(request);
		}

		public async Task<PackagesData> MyPackages(bool showInactive = false, DateTime? fromDate = null)
		{
			var request = PrepareRequest("/v1/my_packages", Method.POST);

			request.AddParameter("show_inactive", showInactive);
			if (fromDate.HasValue) {
				var ut = DateTimeHelper.ToUnixTime(fromDate.Value);
				request.AddParameter("from_date", ut.ToString());
			}

			return await SendRequest<PackagesData>(request);
		}

		public async Task<Package> Package(string paketId)
		{
			var request = PrepareRequest("/v1/package", Method.POST);

			request.AddParameter("paket_id", paketId);

			return await SendRequest<Package>(request);
		}

		public async Task<RelayPackageData> RelayPackage(string paketId, string courierPubkey, int paymentBuls)
		{
			var request = PrepareRequest("/v1/relay_package", Method.POST);

			request.AddParameter("paket_id", paketId);
			request.AddParameter("courier_pubkey", courierPubkey);
			request.AddParameter("payment_buls", paymentBuls);

			return await SendRequest<RelayPackageData>(request);
		}

		#endregion Packages

		#endregion API Methods

		#region Client Methods

		private RestRequest PrepareRequest(string url, Method method, string pubkey = null)
		{
			//System.Diagnostics.Debug.WriteLine("Request URL: {0}, Token: {1}", url, Profile.Token);

			pubkey = "owner";

			var request = new RestRequest(url);
			pubkey = pubkey ?? TryGetPubKey?.Invoke();
			if (pubkey != null) {
				request.AddHeader("Pubkey", pubkey);
			}
			//request.RequestFormat = DataFormat.Json;
			request.Method = method;
			request.Serializer = _serializer;
			return request;
		}

		private async Task<T> SendRequest<T>(RestRequest request, RawBytes rb = null, System.IO.Stream responseStream = null, bool preferSSL = false, bool suppressUnAuthorized = false, bool suppressNoConnection = false, bool suppressServerErrors = false)
		{
			try {
				IRestResponse<T> response;

				if (responseStream != null) {
					//request.ResponseWriter = (obj) => {
					//	obj.CopyTo(responseStream);
					//	obj.Close();
					//};
					response = await _client.Execute<T>(request);
					responseStream.Dispose();
				} else {
					response = await _client.Execute<T>(request);
					if (rb != null) {
						rb.Data = response.RawBytes;
					}
				}

				System.Diagnostics.Debug.WriteLine("Status: {0}, Content: {1}", response.StatusCode, response.Content);
				ServiceStackSerializer.HandleStatusCode(response);
				return response.Data;
			} catch (WebException e) {
				System.Diagnostics.Debug.WriteLine("SendRequest to: {0} Error: {1}", _client.BaseUrl, e.ToString());
				//if (!suppressNoConnection)
					//_workspace.OnConnectionError(EventArgs.Empty);
			} catch (ServiceException e) {
				System.Diagnostics.Debug.WriteLine(e);
				//if (!suppressServerErrors)
					//_workspace.OnServiceError(new ServiceErrorEventArgs(e));
			} catch (UnAuthorizedException e) {
				//Profile.DeleteCredentials ();
				//if (!suppressUnAuthorized)
					//_workspace.OnAuthenticationRequired(e);
			} catch (Exception e) {
				System.Diagnostics.Debug.WriteLine(e);
				//if (!suppressNoConnection)
					//_workspace.OnConnectionError(EventArgs.Empty);
			}

			return default(T);
		}

		#endregion Client Methods

		public class ServiceStackSerializer : RestSharp.Portable.IDeserializer, RestSharp.Portable.ISerializer
		{
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
				if (response.StatusCode == HttpStatusCode.Forbidden)
					throw new ForbiddenException(response.Content);
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
				return JsonConvert.DeserializeObject<T>(response.Content);
			}

			public byte[] Serialize(object obj)
			{
				return Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
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
			[DataMember(Name = "message")]
			public string Message { get; set; }
			//[DataMember(Name = "errcode")]
			//public ApiErrorCode Code { get; set; }
		}
	}
}
