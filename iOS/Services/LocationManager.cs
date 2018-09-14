using System;

using CoreLocation;
using UIKit;
using Foundation;

namespace PaketGlobal.iOS
{
	public class LocationManager
	{
		protected CLLocationManager locMgr;
		// event for the location changing
		public event EventHandler<LocationUpdatedEventArgs> LocationUpdated = delegate { };

        private bool IsLocationUpdates = false;
        private CLLocation oldLocation;

		public LocationManager ()
		{
			this.locMgr = new CLLocationManager ();

			this.locMgr.PausesLocationUpdatesAutomatically = false; 

			if (UIDevice.CurrentDevice.CheckSystemVersion (8, 0)) {
				locMgr.RequestAlwaysAuthorization ();
			}

			if (UIDevice.CurrentDevice.CheckSystemVersion (9, 0)) {
				locMgr.AllowsBackgroundLocationUpdates = true;
			}
			LocationUpdated += UpdateLocation;
		}

		public CLLocationManager LocMgr {
			get { return this.locMgr; }
		}

		
		public void StartLocationUpdates ()
		{
            if(IsLocationUpdates==false)
            {
                IsLocationUpdates = true;

                if (CLLocationManager.LocationServicesEnabled)
                {
                    LocMgr.DistanceFilter = 100;

                    if (UIDevice.CurrentDevice.CheckSystemVersion(6, 0))
                    {
                        LocMgr.LocationsUpdated += (object sender, CLLocationsUpdatedEventArgs e) => 
                        {
                            this.LocationUpdated(this, new LocationUpdatedEventArgs(e.Locations[e.Locations.Length - 1]));
                        };

                    }
                    else
                    {
                        LocMgr.UpdatedLocation += (object sender, CLLocationUpdatedEventArgs e) => {
                            this.LocationUpdated(this, new LocationUpdatedEventArgs(e.NewLocation));
                        };
                    }

                    LocMgr.StartMonitoringSignificantLocationChanges();

                    LocMgr.Failed += (object sender, NSErrorEventArgs e) => {
                        Console.WriteLine(e.Error);
                    };
                }
                else
                {
                    Console.WriteLine("Location services not enabled, please enable this in your Settings");
                }
            }
		}

        public void StopLocationUpdates()
        {
            if(IsLocationUpdates)
            {
                IsLocationUpdates = false;

                if (LocMgr != null)
                {
                    LocMgr.StopUpdatingLocation();
                    locMgr.StopMonitoringSignificantLocationChanges();
                }
            }
        }

		//This will keep going in the background and the foreground
        private void UpdateLocation (object sender, LocationUpdatedEventArgs e)
		{
            if(oldLocation!=null)
            {
                var distance = oldLocation.DistanceFrom(e.Location);
                if(distance>=100)
                {
                    oldLocation = e.Location;
                    this.OnLocationChangedAsync(oldLocation); 
                }
            }
            else{
                CLLocation location = e.Location;
                oldLocation = location;
                this.OnLocationChangedAsync(location); 
            }

            //var monitores = locMgr.MonitoredRegions.ToArray<CLRegion>();

            //foreach(CLRegion rg in monitores)
            //{
            //    locMgr.StopMonitoring(rg);
            //}

            //var region = new CLRegion(location.Coordinate, 100, "region");
            //region.NotifyOnExit = true;
            //region.NotifyOnEntry = false;

            //locMgr.StartMonitoring(region);
		}

        //private async void ExitRegion(CLRegion region)
        //{
        //    Console.WriteLine(region);
        //}

        public async void OnLocationChangedAsync(CLLocation location)
        {
			var result = await App.Locator.RouteServiceClient.MyPackages();

            if (result.Packages != null)
            {
                var packages = result.Packages;
                var myPubkey = App.Locator.Profile.Pubkey;

                foreach (Package package in packages)
                {
                    var myRole = myPubkey == package.LauncherPubkey ? PaketRole.Launcher :
                                                    (myPubkey == package.RecipientPubkey ? PaketRole.Recipient : PaketRole.Courier);

                    if (myRole == PaketRole.Courier)
                    {
                        var locationString = location.Coordinate.Latitude.ToString() + "," + location.Coordinate.Longitude.ToString();
                        if(locationString.Length>24)
                        {
                            locationString = locationString.Substring(0, 24);
                        }
						await App.Locator.RouteServiceClient.ChangeLocation(package.PaketId, locationString);
                    }
                }
            }
        }
		
	}
}