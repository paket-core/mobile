namespace PaketGlobal.iOS
{
	public class AppInfoService : IAppInfoService
	{
		public string OSVersion {
			get {
				var d = XLabs.Platform.Device.AppleDevice.CurrentDevice;
				return string.Format ("{0} {1}, {2} {3}", d.Manufacturer, d.Name, Xamarin.Forms.Device.RuntimePlatform, d.FirmwareVersion);
				//return UIKit.UIDevice.CurrentDevice.SystemVersion;
			}
		}

		public string AppVersion {
			get { return Foundation.NSBundle.MainBundle.ObjectForInfoDictionary ("CFBundleVersion").ToString () + "." + System.Reflection.Assembly.GetExecutingAssembly ().GetName ().Version.Build; }
		}

		public string PackageName {
			get { return Foundation.NSBundle.MainBundle.BundleIdentifier; }
		}

        public string GitCommit
        {
            get
            {
                return ThisAssembly.Git.Commit;
            }
        }
	}
}
