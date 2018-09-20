using System;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public class PackageHelper
	{
		public static async Task<Package> GetPackageDetails(string paketId)
		{
			var packageData = await App.Locator.RouteServiceClient.Package(paketId);
			if (packageData != null) {
                
                var launcherName = await App.Locator.IdentityServiceClient.GetUser(packageData.Package.LauncherPubkey,null);
                var recipientName = await App.Locator.IdentityServiceClient.GetUser(packageData.Package.RecipientPubkey, null);

                if(launcherName!=null)
                {
                    packageData.Package.LauncherName = launcherName.UserDetails.PaketUser;
                }

                if (recipientName != null)
                {
                    packageData.Package.RecipientName = recipientName.UserDetails.PaketUser;
                }

				var balanceData = await App.Locator.BridgeServiceClient.Balance(paketId);

				packageData.Package.DeliveryStatus = balanceData != null && balanceData.Account.BalanceBUL == 0 ?
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
