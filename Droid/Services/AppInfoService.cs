namespace PaketGlobal.Droid
{
	public class AppInfoService : IAppInfoService
	{
		public string OSVersion {
			get {
				var d = XLabs.Platform.Device.AndroidDevice.CurrentDevice;
				return string.Format ("{0} {1}, {2} {3}", d.Manufacturer, d.Name, Xamarin.Forms.Device.RuntimePlatform, d.FirmwareVersion);
			}
		}

		public string AppVersion {
			get {
				var p = MainActivity.Instance.PackageManager.GetPackageInfo (PackageName, 0);
				return string.Format ("{0} (build {1})", p.VersionName, p.VersionCode);
			}
		}

		public string PackageName {
			get { return MainActivity.Instance.PackageName; }
		}
	}
}
