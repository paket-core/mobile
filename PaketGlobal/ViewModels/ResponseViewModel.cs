﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.NBitcoin;
using Xamarin.Forms;

namespace PaketGlobal
{
    //Basic

    public class RawBytes
    {
        public byte[] Data { get; set; }
    }

    [DataContract]
    public class BaseData
    {
        [DataMember(Name = "status")]
        public int Status { get; set; }
    }

    //User


    [DataContract]
    public class VerifyData : BaseData
    {
       
    }

	[DataContract]
	public class UserData : BaseData
	{
		[DataMember(Name = "user_details")]
		private UserDetails UserDetailsOne { get; set; }

		[DataMember(Name = "user")]
		private UserDetails UserDetailsTwo { get; set; }

		[IgnoreDataMember]
		public UserDetails UserDetails {
			get {
				if (UserDetailsOne != null) {
					return UserDetailsOne;
				} else if (UserDetailsTwo != null) {
					return UserDetailsTwo;
				} else {
					return null;
				}
			}
		}
	}

	[DataContract]
	public class UserDetails
	{
		[DataMember(Name = "full_name")]
		public string FullName { get; set; }

		[DataMember(Name = "call_sign")]
		public string PaketUser { get; set; }

		[DataMember(Name = "phone_number")]
		public string PhoneNumber { get; set; }

		[DataMember(Name = "address")]
		public string Address { get; set; }

		[DataMember(Name = "pubkey")]
		public string Pubkey { get; set; }
	}

	[DataContract]
	public class PrefundData : BaseData
	{
		[DataMember(Name = "hash")]
		public string Hash { get; set; }

		[DataMember(Name = "ledger")]
		public int Ledger { get; set; }

		[DataMember(Name = "envelope_xdr")]
		public string EnvelopeXdr { get; set; }

		[DataMember(Name = "result_xdr")]
		public string ResultXdr { get; set; }

		[DataMember(Name = "result_meta_xdr")]
		public string ResultMetaXdr { get; set; }


		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "title")]
		public string Title { get; set; }

		[DataMember(Name = "detail")]
		public string Detail { get; set; }

		[DataMember(Name = "extras")]
		public Dictionary<string, string> Extras { get; set; }
	}

	//Wallet

	[DataContract]
	public class BalanceData : BaseData
	{
		[DataMember(Name = "account")]
		public AccountData Account { get; set; }

        public string FormattedBalanceBUL {
            get {
                return StellarConverter.ConvertValueToString(Account.BalanceBUL);
            }
        }

        public string FormattedBalanceXLM
        {
            get
            {
                return StellarConverter.ConvertValueToString(Account.BalanceXLM);
            }
        }

        //Binding: 'FormattedBalanceBULEURO' property not found on 'PaketGlobal.BalanceData', target property: 'Xamarin.Forms.Label.Text'
        //Binding: 'FormattedBalanceXLMEURO'

        public string FormattedBalanceXLMEURO
        {
            get
            {
                return "€" + StellarConverter.ConvertValueToString(Account.BalanceXLM/1200);
            }
        }

        public string FormattedBalanceBULEURO
        {
            get
            {
                return "€" + StellarConverter.ConvertValueToString(Account.BalanceBUL/30);
            }
        }
	}

	[DataContract]
	public class AccountData
	{
		[DataMember(Name = "bul_balance")]
		public long BalanceBUL { get; set; }

		[DataMember(Name = "xlm_balance")]
		public long BalanceXLM { get; set; }

		[DataMember(Name = "sequence")]
		public string Sequence { get; set; }

		[DataMember(Name = "signers")]
		public List<SignerData> Signers { get; set; }

		[DataMember(Name = "thresholds")]
		public ThresholdData Thresholds { get; set; }
	}

	[DataContract]
	public class SignerData
	{
		[DataMember(Name = "key")]
		public string Key { get; set; }

		[DataMember(Name = "public_key")]
		public string PublicKey { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "weight")]
		public int Weight { get; set; }
	}

	[DataContract]
	public class ThresholdData
	{
		[DataMember(Name = "high_threshold")]
		public int HighThreshold { get; set; }

		[DataMember(Name = "low_threshold")]
		public int LowThreshold { get; set; }

		[DataMember(Name = "med_threshold")]
		public int MedThreshold { get; set; }
	}

	[DataContract]
	public class PriceData : BaseData
	{
		[DataMember(Name = "buy_price")]
		public int BuyPrice { get; set; }

		[DataMember(Name = "sell_price")]
		public int SellPrice { get; set; }
	}

	[DataContract]
	public class WalletPubkeyData : BaseData
	{
		[DataMember(Name = "pubkey")]
		public string Pubkey { get; set; }
	}

	[DataContract]
	public class SendBulsData : BaseData
	{
		[DataMember(Name = "transaction")]
		public string Transaction { get; set; }
	}

	[DataContract]
	public class PurchaseTokensData : BaseData
	{
		[DataMember(Name = "payment_amount")]
		public int PaymentAmount { get; set; }

		[DataMember(Name = "payment_currency")]
		public string PaymentCurrency { get; set; }

		[DataMember(Name = "payment_pubkey")]
		public string PaymentAddress { get; set; }
	}

	[DataContract]
	public class SubmitTransactionData : BaseData
	{
		[DataMember(Name = "transaction")]
		public TransactionData Transaction { get; set; }
	}

	[DataContract]
	public class TransactionData
	{
		[DataMember(Name = "envelope_xdr")]
		public string EnvelopeXdr { get; set; }

		[DataMember(Name = "hash")]
		public string Hash { get; set; }

		[DataMember(Name = "ledger")]
		public long Ledger { get; set; }

		[DataMember(Name = "result_meta_xdr")]
		public string ResultMetaXdr { get; set; }

		[DataMember(Name = "result_xdr")]
		public string ResultXdr { get; set; }

		[DataMember(Name = "_links")]
		public Dictionary<string, Dictionary<string, string>> Links { get; set; }
	}

	[DataContract]
	public class PrepareCreateAccountData : BaseData
	{
		[DataMember(Name = "transaction")]
		public string CreateTransaction { get; set; }
	}

	//Packages

	[DataContract]
	public class AcceptPackageData : BaseData
	{
		
	}

    [DataContract]
    public class ChangeLocationData : BaseData
    {

    }

    [DataContract]
    public class AddEventData : BaseData
    {

    }


	[DataContract]
	public class LaunchPackageData : BaseData
	{
		[DataMember(Name = "paket_id")]//"escrow_address")]
		public string EscrowAddress { get; set; }

		[DataMember(Name = "set_options_transaction")]
		public string SetOptionsTransaction { get; set; }

		[DataMember(Name = "payment_transaction")]
		public string PaymentTransaction { get; set; }

		[DataMember(Name = "refund_transaction")]
		public string RefundTransaction { get; set; }

		[DataMember(Name = "merge_transaction")]
		public string MergeTransaction { get; set; }
	}

	[DataContract]
	public class BarcodePackageData
	{
		[DataMember(Name = "escrow_address")]
		public string EscrowAddress { get; set; }
	}

	[DataContract]
	public class PackagesData : BaseData
	{
		[DataMember(Name = "packages")]
		public List<Package> Packages { get; set; }
	}

    [DataContract]
    public class AvailablePackagesData : BaseData
    {
        [DataMember(Name = "packages")]
        public List<AvaiablePackage> Packages { get; set; }
    }

	[DataContract]
	public class PackageData : BaseData
	{
		[DataMember(Name = "package")]
		public Package Package	 { get; set; }
	}

    public class AvaiablePackage : Package
    {
    }

    public class NotFoundPackage : FilterPackages
    {
    }

    public class FilterPackages : AvaiablePackage
    {
        private double radius { get; set; }
        private string radiusString { get; set; }

        public string RadiusString
        {
            get
            {
                return Convert.ToInt32(Radius).ToString() + " km";
            }
            set
            {
                radiusString = value;
            }
        }

        public double Radius
        {
            get
            {
                return radius;
            }
            set
            {
                radius = value;
                OnPropertyChanged("Radius");
                OnPropertyChanged("RadiusString");
            }
        }
    }

	[DataContract]
    public class Package : BaseViewModel
	{
        private string recipientPhoneCode;
        private string recipientPhoneNumber;
        private string launcherPhoneNumber;
        private string launcherPhoneCode;
		private string fromLocationGPS { get; set; }
		private string toLocationGPS { get; set; }
		private string fromLocationAddress { get; set; }
		private string toLocationAddress { get; set; }
		private string status { get; set; }

        public bool isNewPackage { get; set; }

		[DataMember(Name = "escrow_pubkey")]
		public string PaketId { get; set; }

		[DataMember(Name = "paket_url")]
		public string PaketUrl { get; set; }

		[DataMember(Name = "launcher_pubkey")]
		public string LauncherPubkey { get; set; }

		public PaketRole MyRole { get; set; }

		[DataMember(Name = "recipient_pubkey")]
		public string RecipientPubkey { get; set; }

		///[DataMember(Name = "courier_pubkey")]
        public string CourierPubkey
        {
            get{
                if(Events != null)
                {
                    foreach (PackageEvent ev in Events)
                    {
                        if (ev.EventType == "assign package")
                        {
                            return ev.UserPubKey;
                        }
                    }
                }

                return null;
            }

        }

        public string DistanceToPickup
        {
            get{
                if (!App.Locator.LocationHelper.lat.Equals(0.0))
                {
                    var helper = new MapHelper();

                    double from_lat = Convert.ToDouble(fromLocationGPS.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
                    double from_lng = Convert.ToDouble(fromLocationGPS.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);

                    double distance = helper.distance(from_lat, from_lng, App.Locator.LocationHelper.lat, App.Locator.LocationHelper.lng);

                    return String.Format("{0:0.00} KM", distance);
                }

                return "0 KM"; 
            }
        }

        public string Distance
        {
            get
            {
                if (fromLocationGPS == null || toLocationGPS==null)
                {
                    return "0 KM";
                }

                var helper = new MapHelper();

				double from_lat = Convert.ToDouble(fromLocationGPS.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
				double from_lng = Convert.ToDouble(fromLocationGPS.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);

				double to_lat = Convert.ToDouble(toLocationGPS.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
				double to_lng = Convert.ToDouble(toLocationGPS.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);

                double distance = helper.distance(from_lat, from_lng, to_lat, to_lng);

                return String.Format("{0:0.00} KM", distance);
            }
        }

		[DataMember(Name = "from_location")]
        public string FromLocationGPS
        {
            get
            {
                return fromLocationGPS;
            }
            set
            {
                fromLocationGPS = value;
                OnPropertyChanged("FromLocationGPS");
                OnPropertyChanged("Distance");
            }
        }

		[DataMember(Name = "to_location")]
        public string ToLocationGPS
        {
            get
            {
                return toLocationGPS;
            }
            set
            {
                toLocationGPS = value;
                OnPropertyChanged("ToLocationGPS");
                OnPropertyChanged("Distance");
            }
        }

        public string ToImage
        {
            get
            {
                var helper = new MapHelper();

                double to_lat = Convert.ToDouble(toLocationGPS.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
                double to_lng = Convert.ToDouble(toLocationGPS.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);

                var s = helper.GetStaticMapUri(to_lat, to_lng, 14, 280, 280);

                return s;
            }
        }

        public string FromImage{
            get{
                var helper = new MapHelper();

                double from_lat = Convert.ToDouble(fromLocationGPS.Split(',')[0], System.Globalization.CultureInfo.InvariantCulture);
                double from_lng = Convert.ToDouble(fromLocationGPS.Split(',')[1], System.Globalization.CultureInfo.InvariantCulture);

                var s =  helper.GetStaticMapUri(from_lat, from_lng, 14, 280, 280);

                return s;
            }
        }


		[DataMember(Name = "from_address")]
        public string FromLocationAddress
        {
            get
            {
                if (fromLocationGPS == null)
                {
                    return AppResources.SelectLocation;
                }

                return HttpUtility.UrlDecode(fromLocationAddress);
                //return fromLocationAddress;
            }
            set
            {
                fromLocationAddress = value;
                OnPropertyChanged("FromLocationAddress");
            }
        }

		[DataMember(Name = "to_address")]
        public string ToLocationAddress 
        { 
            get{
                if (toLocationGPS == null)
                {
                    return AppResources.SelectLocation;
                }

                return HttpUtility.UrlDecode(toLocationAddress);
            } 
            set{
                toLocationAddress = value;
                OnPropertyChanged("ToLocationAddress");
            } 
        }

        public string LauncherName { get; set; }
        public string RecipientName { get; set; }
        public string CourierName { get; set; }

        public string FullFormattedStatus{
            get{
                return ShortEscrow + " " + FormattedStatus;
            }
        }

        public string ShortEscrow
        {
            get
            {
                return PaketId.Substring(0,3);
            }
        }

		[DataMember(Name = "status")]
        public string Status
        {
            get
            {
                return status;
            }

            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged("Status");
                }
            }
        }

		[DataMember(Name = "blockchain_url")]
		public string BlockchainUrl { get; set; }

		[DataMember(Name = "custodian_pubkey")]
		public string CustodianPubkey { get; set; }

		[DataMember(Name = "deadline")]
		public long Deadline { get; set; }

		[DataMember(Name = "payment")]
		public long Payment { get; set; }

		[DataMember(Name = "collateral")]
		public long Collateral { get; set; }

        [DataMember(Name = "launch_date")]
        public string SendTimestamp { get; set; }

		[DataMember(Name = "events")]
		public List<PackageEvent> Events { get; set; }

		[DataMember(Name = "payment_transaction")]
		public string PaymentTransaction { get; set; }

        [DataMember(Name = "refund_transaction")]
        public string RefundTransaction { get; set; }

        [DataMember(Name = "merge_transaction")]
        public string MergeTransaction { get; set; }


		public DeliveryStatus DeliveryStatus { get; set; }

		public DeliveryStatus DeliveryStatusPrivate {
			get { return MyRole == PaketRole.Launcher ? DeliveryStatus : DeliveryStatus.None; }
		}

		public DateTime DeadlineDT {
			get { return DateTimeHelper.FromUnixTime(Deadline).ToLocalTime(); }
		}

		public DateTime SendTimeDT {
            get{
                return DateTime.Parse(SendTimestamp);
            }
		}

		public string DeadlineString {
			get { 
                if(DeadlineDT.Year==1970) {
                    return "";
                }
                return DeadlineDT.ToString("dd.MM.yyyy");
            }
		}

		public string SendTimeString {
			get { return SendTimeDT.ToString("dd.MM.yyyy"); }
		}

        public string StatusIconWithText
        {
            get
            {
                if (Status == "waiting pickup")
                {
                    return "waiting_status_icon.png";
                }
                else if (Status == "delivered")
                {
                    return "delivered_status_icon.png";
                }
                return "in_transit_status.png";
            }  
        }

        public string StatusIcon
        {
            get { 
                if (Status=="waiting pickup") {
                    return "waiting_pickup.png";  
                }
                else if (Status == "delivered")
                {
                    return "delivered_icon.png";
                }
                return "in_transit.png";
            }
        }

        public string FormattedStatus
        {
            get
            {
                return Status.ToUpperInvariant();
            }
        }

        public float Progress
        {
            get { 
                if (Status == "waiting pickup")
                {
                    return 0.1f;
                }
                else if (Status == "delivered")
                {
                    return 1.0f;
                }

                return 0.5f;
            }
        }

        public string ProgressIcon
        {
            get
            {
                if (Status == "delivered")
                {

                    return "point_0";
                }

                return "point_1";
            }
        }

        public string RoleString
        {
            get
            {
                if (MyRole == PaketRole.Courier)
                {
                    return "Courier";
                }
                else if (MyRole == PaketRole.Recipient)
                {
                    return "Recipient";
                }
                else{
                    return "Launcher";
                }
            }
        }
               
        public string NameFromKey(string key)
        {
            if(key == RecipientPubkey)
            {
                return (RecipientName != null) ? RecipientName : "";
            }
            else if(key == LauncherPubkey)
            {
                return (LauncherName != null) ? LauncherName : "";
            }
            else if(key == CourierPubkey)
            {
                return (CourierName != null) ? CourierName : "";
            }
            else{
                return "";
            }
        }

        public string FormattedCollateral
        {
            get
            {
                return StellarConverter.ConvertValueToString(Collateral);  
            }
        }

        public string FormattedPayment
        {
            get
            {
                return StellarConverter.ConvertValueToString(Payment);  
            }
        }


        public string LauncherPhoneNumber
        {
            get { return launcherPhoneNumber; }
            set { SetProperty(ref launcherPhoneNumber, value); }
        }

        public string LauncherPhoneCode
        {
            get
            {
                if (launcherPhoneCode == null)
                {
                    launcherPhoneCode = ISO3166.GetCurrentCallingCode();
                }
                return launcherPhoneCode;
            }
            set { SetProperty(ref launcherPhoneCode, value); }
        }

        public string LauncherFullPhoneNumber
        {
            get
            {
                if (launcherPhoneNumber == null || launcherPhoneCode == null)
                {
                    return "";
                }
                return launcherPhoneCode + launcherPhoneNumber;
            }
        }

        public string RecipientPhoneNumber
        {
            get { return recipientPhoneNumber; }
            set { SetProperty(ref recipientPhoneNumber, value); }
        }

        public string RecipientPhoneCode
        {
            get
            {
                if (recipientPhoneCode == null)
                {
                    recipientPhoneCode = ISO3166.GetCurrentCallingCode();
                }
                return recipientPhoneCode;
            }
            set { SetProperty(ref recipientPhoneCode, value); }
        }

        public string RecipientFullPhoneNumber
        {
            get
            {
                if (recipientPhoneNumber == null || recipientPhoneCode == null)
                {
                    return "";
                }
                return recipientPhoneCode + recipientPhoneNumber;
            }
        }


	}

	[DataContract]
	public class PackageEvent
	{
		[DataMember(Name = "event_type")]
		public string EventType { get; set; }

        public string PaketUser
        {
            get{
                return UserPubKey;
            }
        }

		[DataMember(Name = "timestamp")]
        public string Timestamp { get; set; }

        [DataMember(Name = "user_pubkey")]
        public string UserPubKey { get; set; }


		[DataMember(Name = "GPS")]
		public double[] GPS { get; set; }

		public DateTime TimestampDT {
            get{
                DateTime parsedDate = DateTime.Parse(Timestamp);

                return parsedDate;
            }
		}

		public string GPSString {
			get { return String.Join(",", GPS); }
		}
	}

	[DataContract]
	public class RelayPackageData : BaseData
	{
		[DataMember(Name = "promise")]
		public string Promise { get; set; }
	}
}
