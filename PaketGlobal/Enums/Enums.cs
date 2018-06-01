using System;
namespace PaketGlobal
{
	public enum StellarOperationResult
	{
		Success,
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
}
