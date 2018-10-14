using System;
using System.Net;
using System.Text;
using System.Runtime.Serialization;

using Newtonsoft.Json;

using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;
using System.Linq;
using Plugin.Geolocator;
using System.Threading;

namespace PaketGlobal
{
    public class Escrow_Xdrs
    {
        public Kwargs escrow_xdrs;
    }

    public class Kwargs
    {
        public Kwargs()
        {
            
        }
        public string  set_options_transaction = "";
        public string  refund_transaction = "";
        public string  merge_transaction = "";
        public string  payment_transaction = "";
    }

	public class ServiceClient
	{
        public static readonly string[] IgnoreErrors = new string[] { "get_user", "bul_account"};

		public readonly string apiVersion;

		public delegate string PubKeyHandler();
		public delegate string SignHandler(string data);

		string restUrl;

		readonly RestClient restClient;

		ServiceStackSerializer _serializer;

		public PubKeyHandler TryGetPubKey { get; set; }
		public SignHandler TrySign { get; set; }

		public ServiceClient(string url, string version)
		{
			restUrl = url;
			apiVersion = version;

            _serializer = new ServiceStackSerializer();

			restClient = new RestClient(url);
			restClient.UserAgent = "PaketGlobal";
			restClient.Timeout = 60 * 1000;
			restClient.AddHandler("application/json", _serializer);
		}

		#region API Methods

		#region User

        public async Task<UserData> RegisterUser(string paketUser, string pubkey)
		{
			var request = PrepareRequest(apiVersion + "/create_user", Method.POST);

			request.AddParameter("call_sign", paketUser);

			return await SendRequest<UserData>(request, pubkey);
		}

		public async Task<VerifyData> SendVerification()
		{
			var request = PrepareRequest(apiVersion + "/request_verification_code", Method.POST);

			return await SendRequest<VerifyData>(request);
		}

        public async Task<VerifyData> VerifyCode(string code)
        {
            var request = PrepareRequest(apiVersion + "/verify_code", Method.POST);

            request.AddParameter("verification_code", code);

            return await SendRequest<VerifyData>(request);
        }

        public async Task<RatioData> GetWalletRatio(string currency)
        {
            var request = PrepareRequest(apiVersion + "/ratio", Method.POST);

            request.AddParameter("currency", currency);

            return await SendRequest<RatioData>(request);
        }

        public async Task<CallsignsData> GetCallsigns()
        {
            var request = PrepareRequest(apiVersion + "/callsigns", Method.POST);
            
            return await SendRequest<CallsignsData>(request);
        }

        public async Task<PackagePhotoData> GetPackagePhoto(string puckageId)
        {
            var request = PrepareRequest(apiVersion + "/package_photo", Method.POST);

            request.AddParameter("escrow_pubkey", puckageId);

            return await SendRequest<PackagePhotoData>(request, signData: false);
        }

        public async Task<UserData> GetUser(string pubkey, string call_sign)
		{
			var request = PrepareRequest(apiVersion + "/get_user", Method.POST);

            if (pubkey != null) {
                request.AddParameter("pubkey", pubkey);
            }
            else if(call_sign != null) {
                request.AddParameter("call_sign", call_sign);
            }

			return await SendRequest<UserData>(request, signData: false);
		}

		public async Task<UserData> UserInfos()
		{
			return await UserInfos(null, null, null);
		}

		public async Task<UserData> UserInfos(string fullName, string phoneNumber, string address, string pubkey = null)
		{
			var request = PrepareRequest(apiVersion + "/user_infos", Method.POST);

			if (fullName != null) request.AddParameter("full_name", fullName);
			if (phoneNumber != null) request.AddParameter("phone_number", phoneNumber);
			if (address != null) request.AddParameter("address", address);

			return await SendRequest<UserData>(request, pubkey);
		}

        public async Task<TrustData> PrepareTrust(string pubkey)
        {
            var request = PrepareRequest(apiVersion + "/prepare_trust", Method.POST);

            if (pubkey != null)
                request.AddParameter("from_pubkey", pubkey);

            return await SendRequest<TrustData>(request, pubkey);
        }

        public async Task<AddEventData> AddEvent(string eventType)
        {
            if(App.Locator.Profile.Pubkey==null)
            {
                return new AddEventData();
            }

            var hasPermission = await Utils.OnlyCheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);

            string location = null;

            if (hasPermission)
            {
                var locator = CrossGeolocator.Current;

                var position = await locator.GetPositionAsync();

                if (position != null)
                {
                    location = position.Latitude.ToString(System.Globalization.CultureInfo.InvariantCulture) + "," + position.Longitude.ToString(System.Globalization.CultureInfo.InvariantCulture);

                    if (location.Length > 24)
                    {
                        location = location.Substring(0, 24);
                    }
                }
            }

            var request = PrepareRequest(apiVersion + "/add_event", Method.POST);

            request.AddParameter("event_type", eventType);
           
            if (location != null)
            {
                request.AddParameter("location", location);
            }

            return await SendRequest<AddEventData>(request);
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

        public async Task<PurchaseTokensData> PurchaseBULs(double euroCents, PaymentCurrency paymentCurrency)
		{
			var request = PrepareRequest(apiVersion + "/purchase_bul", Method.POST);

			request.AddParameter("euro_cents", euroCents * 100);
			request.AddParameter("payment_currency", paymentCurrency.ToString());

			return await SendRequest<PurchaseTokensData>(request);
		}

        public async Task<PurchaseTokensData> PurchaseXLMs(double euroCents, PaymentCurrency paymentCurrency)
		{
			var request = PrepareRequest(apiVersion + "/purchase_xlm", Method.POST);

			request.AddParameter("euro_cents", euroCents * 100);
			request.AddParameter("payment_currency", paymentCurrency.ToString());

			return await SendRequest<PurchaseTokensData>(request);
		}


        public async Task<SendBulsData> PrepareSendBuls(string fromPubkey, string toPubkey, double amountBuls, bool disableConvertor = false)
		{
			var request = PrepareRequest(apiVersion + "/prepare_send_buls", Method.POST);

            var amount = amountBuls;

            if(!disableConvertor)
            {
                amount = StellarConverter.ConvertBULToStroops(amountBuls);
            }

            if(StellarConverter.IsValidBUL(amount)==false)
            {
                throw new ServiceException(400, "You can't specify more then 7 fractional digits");
            }

			var myBalance = await App.Locator.BridgeServiceClient.Balance(App.Locator.Profile.Pubkey);

            if (myBalance == null || myBalance.Account.BalanceBUL < amount)
            {
                throw new ServiceException(420,AppResources.InsufficientBULs);
            }

			request.AddParameter("from_pubkey", fromPubkey);
			request.AddParameter("to_pubkey", toPubkey);
            request.AddParameter("amount_buls", amount);

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
			var request = PrepareRequest(apiVersion + "/prepare_account", Method.POST);

			request.AddParameter("from_pubkey", fromPubkey);
			request.AddParameter("new_pubkey", newPubkey);
            request.AddParameter("starting_balance", startingBalance * 10000000);

			return await SendRequest<PrepareCreateAccountData>(request);
		}

		#endregion Wallet

		#region Packages

        public async Task<AcceptPackageData> AssignPackage(string escrowPubkey, string location)
        {
            if (location.Length > 24)
            {
                location = location.Substring(0, 24);
            }

            var request = PrepareRequest(apiVersion + "/confirm_couriering", Method.POST);

            request.AddParameter("escrow_pubkey", escrowPubkey);
            if (location != null)
            {
                request.AddParameter("location", location);
            }

            return await SendRequest<AcceptPackageData>(request);
        }

		public async Task<AcceptPackageData> AcceptPackage(string escrowPubkey, string location)
		{
			var request = PrepareRequest(apiVersion + "/accept_package", Method.POST);

			request.AddParameter("escrow_pubkey", escrowPubkey);
            if(location!=null)
            {
                request.AddParameter("location", location);
            }

			return await SendRequest<AcceptPackageData>(request);
		}

        public async Task<LaunchPackageData> PrepareEscrow(string escrowPubkey, string launcherPubkey, string recipientPubkey, long deadlineTimestamp, string courierPubkey, double paymentBuls, double collateralBuls, SignHandler customSign)
		{
			var request = PrepareRequest(apiVersion + "/prepare_escrow", Method.POST);

            //var payment = StellarConverter.ConvertBULToStroops(paymentBuls);

            //if (StellarConverter.IsValidBUL(payment)==false)
            //{
            //    throw new ServiceException(400, "You can't specify more then 7 fractional digits");
            //}

            //var collateral = StellarConverter.ConvertBULToStroops(collateralBuls);

            //if (StellarConverter.IsValidBUL(collateral)==false)
            //{
            //    throw new ServiceException(400, "You can't specify more then 7 fractional digits");
            //}

			request.AddParameter("launcher_pubkey", launcherPubkey);
			request.AddParameter("recipient_pubkey", recipientPubkey);
			request.AddParameter("deadline_timestamp", deadlineTimestamp);
			request.AddParameter("courier_pubkey", courierPubkey);
            request.AddParameter("payment_buls", paymentBuls);
            request.AddParameter("collateral_buls", collateralBuls);

			return await SendRequest<LaunchPackageData>(request, escrowPubkey, customSign: customSign);
		}

		public async Task<PackageData> CreatePackage(string escrowPubkey, string recipientPubkey, string launcherPhone, string recipientPhone, long deadlineTimestamp, double paymentBuls, double collateralBuls,
                                                     string description, string fromAddress, string toAddress, string fromLocation, string toLocation, string eventLocation, byte[] packagePhoto, SignHandler customSign)
		{
			var request = PrepareRequest(apiVersion + "/create_package", Method.POST);

			var payment = StellarConverter.ConvertBULToStroops(paymentBuls);

			if (StellarConverter.IsValidBUL(payment) == false) {
				throw new ServiceException(400, "You can't specify more then 7 fractional digits");
			}

			var collateral = StellarConverter.ConvertBULToStroops(collateralBuls);

			if (StellarConverter.IsValidBUL(collateral) == false) {
				throw new ServiceException(400, "You can't specify more then 7 fractional digits");
			}

			request.AddParameter("escrow_pubkey", escrowPubkey);
			request.AddParameter("recipient_pubkey", recipientPubkey);
			request.AddParameter("launcher_phone_number", launcherPhone);
			request.AddParameter("recipient_phone_number", recipientPhone);
			request.AddParameter("deadline_timestamp", deadlineTimestamp);
			request.AddParameter("payment_buls", payment);
			request.AddParameter("collateral_buls", collateral);
			request.AddParameter("description", description);
			request.AddParameter("to_address", toAddress);
			request.AddParameter("from_address", fromAddress);
			request.AddParameter("from_location", fromLocation);
			request.AddParameter("to_location", toLocation);
			request.AddParameter("event_location", eventLocation);
			if (packagePhoto != null) request.AddFile("photo", packagePhoto, "photo.jpg");

			return await SendRequest<PackageData>(request, customSign: customSign);
		}

		public async Task<BaseData> FinalizePackage(string location, string escrowPubkey, string setOptionsTrans, string refundTrans, string mergeTrans, string paymentTrans)
		{
			var request = PrepareRequest(apiVersion + "/assign_xdrs", Method.POST);

            if (location.Length > 24)
            {
                location = location.Substring(0, 24);
            }


            var kwargs = new Kwargs();
            kwargs.merge_transaction = mergeTrans;
            kwargs.set_options_transaction = setOptionsTrans;
            kwargs.refund_transaction = refundTrans;
            kwargs.payment_transaction = paymentTrans;

            var escrow_xdrs = new Escrow_Xdrs();
            escrow_xdrs.escrow_xdrs = kwargs;

            var json = JsonConvert.SerializeObject(escrow_xdrs);

			request.AddParameter("escrow_pubkey", escrowPubkey);
            request.AddParameter("kwargs", json);

            if(location != null)
            {
                request.AddParameter("location", location);

            }

            return await SendRequest<BaseData>(request);
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

        public async Task<AvailablePackagesData> AvailablePackages(string location, int radius, CancellationTokenSource cancellationTokenSource)
        {
            if (location.Length > 24)
            {
                location = location.Substring(0, 24);
            }

            var request = PrepareRequest(apiVersion + "/available_packages", Method.POST);

            request.AddParameter("location", location);
            request.AddParameter("radius_num", radius);

            return await SendRequest<AvailablePackagesData>(request,null,true,null,null,null,null,false,false,false,false,cancellationTokenSource);
        }

		public async Task<PackageData> Package(string escrowPubkey)
		{
			var request = PrepareRequest(apiVersion + "/package", Method.POST);

			request.AddParameter("escrow_pubkey", escrowPubkey);

			return await SendRequest<PackageData>(request);
		}

        public async Task<ChangeLocationData> ChangeLocation(string escrowPubkey, string location)
        {
            var request = PrepareRequest(apiVersion + "/changed_location", Method.POST);

            request.AddParameter("escrow_pubkey", escrowPubkey);
            request.AddParameter("location", location);

            return await SendRequest<ChangeLocationData>(request);
        }


		#endregion Packages

		#endregion API Methods

		#region Client Methods

        private void SignRequest(RestRequest request, string pubkey, RestClient client, bool includePubkey = true, SignHandler customSign = null)
        {
            StringBuilder fingerprint = new StringBuilder();
            fingerprint.Append(System.IO.Path.Combine(client.BaseUrl.AbsoluteUri + request.Resource));

            foreach (var p in request.Parameters)
            {
                fingerprint.AppendFormat(",{0}={1}", p.Name, p.Value);
            }

            fingerprint.AppendFormat(",{0}", DateTimeHelper.ToMilliUnixTime(DateTime.UtcNow));
            request.AddHeader("Fingerprint", fingerprint.ToString());

            var signHandler = customSign ?? TrySign;
            var signature = signHandler?.Invoke(fingerprint.ToString());
            if (signature != null) request.AddHeader("Signature", signature);

            if (includePubkey)
            {
                pubkey = pubkey ?? TryGetPubKey?.Invoke();
                if (pubkey != null){
                    request.AddHeader("Pubkey", pubkey);
                }
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

        private async Task<T> SendRequest<T>(RestRequest request, string pubkey = null, bool signData = true, RestClient customClient = null, SignHandler customSign = null, RawBytes rb = null, System.IO.Stream responseStream = null, bool preferSSL = false, bool suppressUnAuthorized = false, bool suppressNoConnection = false, bool suppressServerErrors = false, CancellationTokenSource cancellationTokenSource = null)
		{
			var client = customClient ?? restClient;

			if (signData) 
                SignRequest(request, pubkey, client, customSign: customSign);

            try
            {
                IRestResponse<T> response;

                if (responseStream != null)
                {
                    request.ResponseWriter = (obj) =>
                    {
                        obj.CopyTo(responseStream);
                        obj.Close();
                    };

                    if(cancellationTokenSource != null)
                    {
                        response = await client.ExecuteTaskAsync<T>(request, cancellationTokenSource.Token);  
                    }
                    else{
                        response = await client.ExecuteTaskAsync<T>(request);  
                    }

                    responseStream.Dispose();
                }
                else
                {
                    if (cancellationTokenSource != null)
                    {
                        response = await client.ExecuteTaskAsync<T>(request,cancellationTokenSource.Token);
                    }
                    else{
                        response = await client.ExecuteTaskAsync<T>(request);
                    }

                    if (rb != null)
                    {
                        rb.Data = response.RawBytes;
                    }
                }

                if(response.ResponseUri!=null)
                {
                    if (!response.ResponseUri.ToString().Contains("package_photo"))
                    {
                        System.Diagnostics.Debug.WriteLine("Status: {0}, Content: {1}", response.StatusCode, response.Content);
                    }
                }
           

                ServiceStackSerializer.HandleStatusCode(response);
                return response.Data;
            }
            catch (WebException e)
            {
                System.Diagnostics.Debug.WriteLine("SendRequest to: {0} Error: {1}", client.BaseUrl, e.ToString());
                if (!suppressNoConnection)
                    App.Locator.Workspace.OnConnectionError(EventArgs.Empty);
            }
            catch (ServiceException e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                if (!suppressServerErrors){
                    App.Locator.Workspace.OnServiceError(new ServiceErrorEventArgs(e));
                }
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
                {
                    throw new UnAuthorizedException(response.Content);
                }
                else if ((response.StatusCode == HttpStatusCode.NotFound) || (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created))
                {
                    if(response.Content.Length==0 && response.StatusCode == 0)
                    {
                        App.Locator.Workspace.OnConnectionError(EventArgs.Empty);
                    }
                    else{
                        var error = JsonConvert.DeserializeObject<ErrorReply>(response.Content);

                        if(error != null)
                        {
                            if(response.StatusCode==HttpStatusCode.InternalServerError)
                            {
                                var method = response.ResponseUri.Segments.Last();

                                App.Locator.DeviceService.SendErrorEvent(response.Content,method);
                            }
                        }

                        if (error == null)
                        {
                            throw new ServiceException((int)response.StatusCode, response.Content);
                        }
                        else{
                            var method = response.ResponseUri.Segments.Last();
                            if(!ServiceClient.IgnoreErrors.Contains<string>(method))
                            {
                                throw new ServiceException((int)response.StatusCode, error.Error.Message);
                            }
                            else{
                                throw new ServiceException((int)response.StatusCode, ""); 
                            }
                        }
                    } 
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
			public ApiErrorCode ApiErrorCode { get; set; }
			public int StatusCode { get; set; }

			public ServiceException(int statusCode, String message, ApiErrorCode apiErrorCode) : base(message)
			{
				StatusCode = statusCode;
				ApiErrorCode = apiErrorCode;
			}

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
        public class Error
        {
            [DataMember(Name = "error_code")]
            public int ErrorCode { get; set; }
            [DataMember(Name = "message")]
            public string ErrorMessage { get; set; }
            [DataMember(Name = "debug")]
            public string DebugMessage { get; set; }

            public string Message
            {
                get{
                    if(DebugMessage!=null)
                    {
                        return DebugMessage;
                    }

                    return ErrorMessage;
                }
            }
        }

		[DataContract]
		public class ErrorReply
		{
			[DataMember(Name = "status")]
			public int Status { get; set; }
			[DataMember(Name = "error")]
            public Error Error { get; set; }
		}
	}
}
