using System;
using System.Threading.Tasks;

using Android.Content;

using PaketGlobal.Droid;


namespace PaketGlobal.Droid
{
    public class LocationManager
    {
        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        protected static LocationServiceConnection locationServiceConnection;

        public static LocationManager Current
        {
            get { return current; }
        }
        private static LocationManager current;

        public LocationService LocationService
        {
            get
            {
                if (locationServiceConnection.Binder == null)
                {
                    return null;
                }
                return locationServiceConnection.Binder.Service;
            }
        }

        #region Application context

        static LocationManager()
        {
            current = new LocationManager();
        }

        protected LocationManager()
        {
            locationServiceConnection = new LocationServiceConnection(null);

            locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {

                this.LocationServiceConnected(this, e);
            };
        }

        public static void StartLocationService()
        {
            new Task(() => {
                Intent serviceIntent = new Android.Content.Intent(Android.App.Application.Context, typeof(LocationService));
                Android.App.Application.Context.StartService(serviceIntent);

                Intent locationServiceIntent = new Intent(Android.App.Application.Context, typeof(LocationService));
                Android.App.Application.Context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
            }).Start();
        }

        public static void StopLocationService()
        {
            if (locationServiceConnection != null)
            {
                Android.App.Application.Context.UnbindService(locationServiceConnection);
            }

            if (Current.LocationService != null)
            {
                Current.LocationService.StopSelf();
            }
        }

        #endregion

    }
}


