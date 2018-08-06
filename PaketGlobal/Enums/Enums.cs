using System;
namespace PaketGlobal
{
    public enum ApiErrorCode
    {
        
    }

    public static class ApiErrorCodeMethods
    {

        public static String GetString(this ApiErrorCode result)
        {
            return "";
        }
    }

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

    public static class StellarOperationResultMethods
    {

        public static String GetString(this StellarOperationResult result)
        {
            string message = null;

            switch (result)
            {
                case StellarOperationResult.FailSendBuls:
                    message = AppResources.FailSendBuls;
                    break;
                case StellarOperationResult.FailCreateAccount:
                    message = AppResources.FailCreateAccount;
                    break;
                case StellarOperationResult.FailSubmitCreateAccount:
                    message = AppResources.FailSubmitCreateAccount;
                    break;
                case StellarOperationResult.FailAddTrust:
                    message = AppResources.FailAddTrust;
                    break;
                case StellarOperationResult.LowBULsLauncher:
                    message = AppResources.LowBULsLauncher;
                    break;
                case StellarOperationResult.LowBULsCourier:
                    message = AppResources.LowBULsCourier;
                    break;
                case StellarOperationResult.FailedLaunchPackage:
                    message = AppResources.FailedLaunchPackage;
                    break;
                case StellarOperationResult.FaileSubmitOptions:
                    message = AppResources.FaileSubmitOptions;
                    break;
                case StellarOperationResult.IncositentBalance:
                    message = AppResources.IncositentBalance;
                    break;
                case StellarOperationResult.FailAcceptPackage:
                    message = AppResources.FailAcceptPackage;
                    break;
                case StellarOperationResult.FailSendCollateral:
                    message = AppResources.FailSendCollateral;
                    break;
                default:
                    message = AppResources.UnknownError;
                    break;
            }

            return message;
        }
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

	public enum SwipeDirection
	{
		Top = 0,
		Left,
		Right
	}
}
