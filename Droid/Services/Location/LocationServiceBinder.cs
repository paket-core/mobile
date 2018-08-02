using System;
using Android.OS;

namespace PaketGlobal.Droid
{
    public class LocationServiceBinder : Binder
    {
        public LocationService Service;

        public LocationServiceBinder(LocationService service)
        {
            this.Service = service;
        }

        public LocationService GetLocationServiceBinder()
        {
            return Service;
        }
    }
}
