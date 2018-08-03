using System;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using UIKit;

namespace PaketGlobal.iOS
{
    public class LocationSharedService : ILocationSharedService
    {
        public static LocationManager Manager = null;

        public void StartUpdateLocation()
        {
            if(Manager==null)
            {
                Manager = new LocationManager();
                Manager.StartLocationUpdates();
            }
        }

        public void StopUpdateLocation()
        {
            if(Manager != null)
            {
                Manager.StopLocationUpdates();
            }
        }
    }
}
    