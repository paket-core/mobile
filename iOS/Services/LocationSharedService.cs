using System;
using System.Threading.Tasks;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;
using UIKit;

namespace PaketGlobal.iOS
{
    public class LocationSharedService : ILocationSharedService
    {
        public void StartUpdateLocation()
        {
        }

        public void StopUpdateLocation()
        {
        }

        public async Task<string> GetCurrentLocation()
        {
            return null;
        }
    }
}
    