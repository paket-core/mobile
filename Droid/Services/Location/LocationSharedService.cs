using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Plugin.Geolocator;
using Plugin.Geolocator.Abstractions;

namespace PaketGlobal.Droid
{
    
    public class LocationSharedService : ILocationSharedService
    {
        private bool IsRunning = false;

        public void StartUpdateLocation()
        {
            if (!IsRunning)
            {
                IsRunning = true;

                MainActivity.Instance.StartLocationUpdate();
            }
        }

        public void StopUpdateLocation()
        {
            MainActivity.Instance.StopLocationUpdate();
        }

        public async Task<string> GetCurrentLocation()
        {
            return null;
        }
    }

}