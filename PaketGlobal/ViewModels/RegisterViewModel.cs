using System;
namespace PaketGlobal
{
	public class RegisterViewModel : BaseViewModel
	{
		private string userName;
		private string fullName;
		private string phoneNumber;
		private PaymentCurrency? paymentCurrency;

		public string UserName {
			get { return userName; }
			set { SetProperty(ref userName, value); }
		}

		public string FullName {
			get { return fullName; }
			set { SetProperty(ref fullName, value); }
		}

		public string PhoneNumber {
			get { return phoneNumber; }
			set { SetProperty(ref phoneNumber, value); }
		}

		public PaymentCurrency? PaymentCurrency {
			get { return paymentCurrency; }
			set { SetProperty(ref paymentCurrency, value); }
		}

		public override void Reset()
		{
			base.Reset();

			UserName = null;
			FullName = null;
			PhoneNumber = null;
			PaymentCurrency = null;
		}
	}
}
