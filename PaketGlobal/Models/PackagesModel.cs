using System;
using System.Collections.Generic;

namespace PaketGlobal
{
	public class PackagesModel : BaseViewModel
	{
		private List<Package> packagesList = new List<Package>();

		public List<Package> PackagesList {
			get { return packagesList; }
			set { SetProperty(ref packagesList, value); }
		}

		public PackagesModel()
		{
			
		}

		public async System.Threading.Tasks.Task Load()
		{
			var result = await App.Locator.ServiceClient.MyPackages();
			if (result != null) {
				PackagesList = result.Packages;
			}
		}
	}
}
