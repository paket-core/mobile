using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace PaketGlobal.ClientService
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
	public class UserData : BaseData
	{
		[DataMember(Name = "user_details")]
		public UserDetails UserDetails { get; set; }
	}

	[DataContract]
	public class UserDetails
	{
		[DataMember(Name = "full_name")]
		public string FullName { get; set; }

		[DataMember(Name = "paket_user")]
		public string PaketUser { get; set; }

		[DataMember(Name = "phone_number")]
		public string PhoneNumber { get; set; }

		[DataMember(Name = "pubkey")]
		public string Pubkey { get; set; }
	}

	//Wallet

	[DataContract]
	public class BalanceData : BaseData
	{
		[DataMember(Name = "balance")]
		public long Balance { get; set; }

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
	public class SubmitTransactionData : BaseData
	{
		[DataMember(Name = "transaction")]
		public TransactionData Transaction { get; set; }
	}

	[DataContract]
	public class TransactionData
	{
		[DataMember(Name = "envelope_xdr")]
		public TransactionData EnvelopeXdr { get; set; }

		[DataMember(Name = "hash")]
		public TransactionData Hash { get; set; }

		[DataMember(Name = "ledger")]
		public TransactionData Ledger { get; set; }

		[DataMember(Name = "result_meta_xdr")]
		public TransactionData ResultMetaXdr { get; set; }

		[DataMember(Name = "result_xdr")]
		public TransactionData ResultXdr { get; set; }

		[DataMember(Name = "_links")]
		public Dictionary<string, Dictionary<string, string>> Links { get; set; }
	}

	//Packages

	[DataContract]
	public class AcceptPackageData : BaseData
	{
		
	}

	[DataContract]
	public class LaunchPackageData : BaseData
	{
		[DataMember(Name = "escrow_address")]
		public string EscrowAddress { get; set; }

		[DataMember(Name = "payment_transaction")]
		public string PaymentTransaction { get; set; }

		[DataMember(Name = "refund_transaction")]
		public string RefundTransaction { get; set; }
	}

	[DataContract]
	public class BarcodePackageData
	{
		[DataMember(Name = "escrow_address")]
		public string EscrowAddress { get; set; }

		[DataMember(Name = "payment_transaction")]
		public string PaymentTransaction { get; set; }
	}

	[DataContract]
	public class PackagesData : BaseData
	{
		[DataMember(Name = "packages")]
		public List<Package> Packages { get; set; }
	}

	[DataContract]
	public class Package
	{
		[DataMember(Name = "paket_id")]
		public string PaketId { get; set; }

		[DataMember(Name = "paket_url")]
		public string PaketUrl { get; set; }

		[DataMember(Name = "launcher_pubkey")]
		public string LauncherPubkey { get; set; }

		[DataMember(Name = "my_role")]
		public string MyRole { get; set; }

		[DataMember(Name = "recipient_pubkey")]
		public string RecipientPubkey { get; set; }

		[DataMember(Name = "courier_pubkey")]
		public string CourierPubkey { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }

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

		[DataMember(Name = "send-timestamp")]
		public long SendTimestamp { get; set; }

		[DataMember(Name = "events")]
		public List<PackageEvent> Events { get; set; }

		[DataMember(Name = "payment_transaction")]
		public string PaymentTransaction { get; set; }

		public DateTime DeadlineDT {
			get { return DateTimeHelper.FromUnixTime(Deadline); }
		}

		public DateTime SendTimeDT {
			get { return DateTimeHelper.FromUnixTime(SendTimestamp); }
		}

		public string DeadlineString {
			get { return DeadlineDT.ToString("MM.dd.yyyy"); }
		}

		public string SendTimeString {
			get { return SendTimeDT.ToString("MM.dd.yyyy h:mm tt"); }
		}
	}

	[DataContract]
	public class PackageEvent
	{
		[DataMember(Name = "event_type")]
		public string EventType { get; set; }

		[DataMember(Name = "paket_user")]
		public string PaketUser { get; set; }

		[DataMember(Name = "timestamp")]
		public long Timestamp { get; set; }

		[DataMember(Name = "GPS")]
		public double[] GPS { get; set; }

		public DateTime TimestampDT {
			get { return DateTimeHelper.FromUnixTime(Timestamp); }
		}

		public string TimeString {
			get { return TimestampDT.ToString(); }
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
