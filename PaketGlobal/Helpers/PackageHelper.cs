using System;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public class PackageHelper
	{
		public static async Task<Package> GetPackageDetails(string paketId)
		{
			var packageData = await App.Locator.ServiceClient.Package(paketId);
			if (packageData != null) {
				var balanceData = await App.Locator.ServiceClient.Balance(paketId);

				packageData.Package.DeliveryStatus = balanceData != null && balanceData.BalanceBUL == 0 ?
					DeliveryStatus.Delivered :
					(balanceData == null ? DeliveryStatus.Closed : (DateTime.Now > packageData.Package.DeadlineDT ? DeliveryStatus.DeadlineExpired : DeliveryStatus.InTransit));

				var myPubkey = App.Locator.Profile.Pubkey;
				packageData.Package.MyRole = myPubkey == packageData.Package.LauncherPubkey ? PaketRole.Launcher :
					(myPubkey == packageData.Package.RecipientPubkey ? PaketRole.Recipient : PaketRole.Courier);

				return packageData.Package;
			}

			return null;
		}
	}
}
