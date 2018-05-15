using System;

namespace PaketGlobal
{
	public class WalletModel : BaseViewModel
	{
		private BalanceData balance;
		private PriceData price;

		public BalanceData Balance {
			get { return balance; }
			set { SetProperty(ref balance, value); }
		}

		public PriceData Price {
			get { return price; }
			set { SetProperty(ref price, value); }
		}

		public WalletModel()
		{

		}

		public async System.Threading.Tasks.Task Load()
		{
			var bal = await App.Locator.ServiceClient.Balance();
			if (bal != null) {
				Balance = bal;
			}

			var prc = await App.Locator.ServiceClient.Price();
			if (prc != null) {
				Price = prc;
			}
		}
	}
}
