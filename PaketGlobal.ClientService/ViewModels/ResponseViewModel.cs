using System;
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
		[DataMember(Name = "available_buls")]
		public int AvailableBuls { get; set; }
	}

	[DataContract]
	public class PriceData : BaseData
	{
		[DataMember(Name = "buy_price")]
		public int AvailableBuls { get; set; }

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
		[DataMember(Name = "promise")]
		public string Promise { get; set; }
	}

	//Packages

	[DataContract]
	public class AcceptPackageData : BaseData
	{
		
	}

	[DataContract]
	public class LaunchPackageData : BaseData
	{
		[DataMember(Name = "creation_promise")]
		public string CreationPromise { get; set; }

		[DataMember(Name = "paket_id")]
		public string PaketId { get; set; }

		[DataMember(Name = "payment_promise")]
		public string PaymentPromise { get; set; }
	}

	[DataContract]
	public class Package
	{
		[DataMember(Name = "PKT-id")]
		public string PKTId { get; set; }

		[DataMember(Name = "Recipient-id")]
		public string RecipientId { get; set; }

		[DataMember(Name = "collateral")]
		public long Collateral { get; set; }

		[DataMember(Name = "cost")]
		public long Cost { get; set; }

		[DataMember(Name = "deadline-timestamp")]
		public long DeadlineTimestamp { get; set; }

		[DataMember(Name = "send-timestamp")]
		public long SendTimestamp { get; set; }

		[DataMember(Name = "status")]
		public string Status { get; set; }
	}

	[DataContract]
	public class RelayPackageData : BaseData
	{
		[DataMember(Name = "promise")]
		public string Promise { get; set; }
	}
}
