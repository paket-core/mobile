using System;
namespace PaketGlobal
{
	public enum StellarOperationResult
	{
		Success = 0,
		FailSendBuls,
		FailCreateAccount,
		FailSubmitCreateAccount,
		FailAddTrust,
		LowBULsLauncher,
		LowBULsCourier,
		FailedLaunchPackage,
		FaileSubmitOptions,
		IncositentBalance,
		FailAcceptPackage,
		FailSendCollateral
	}

	public enum PaketRole
	{
		Launcher = 0,
		Courier,
		Recipient
	}

	public enum DeliveryStatus
	{
		InTransit = 0,
		Delivered,
		DeadlineExpired,
		Closed,
		None
	}

	public enum PaymentCurrency
	{
		ETH = 0,
		BTC
	}

	public enum SpendCurrency
	{
		BUL = 0,
		XLM
	}
}
