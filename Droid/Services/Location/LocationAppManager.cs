using System;
using System.Threading.Tasks;

using Android.Content;

using PaketGlobal.Droid;


namespace PaketGlobal.Droid
{
    public class LocationAppManager
    {
        public static bool IsNeedRequestPackages = false;
        public static bool isServiceStarted = false;

        public event EventHandler<ServiceConnectedEventArgs> LocationServiceConnected = delegate { };

        protected static LocationServiceConnection locationServiceConnection;

        public static LocationAppManager Current
        {
            get { return current; }
        }
        private static LocationAppManager current;

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

        static LocationAppManager()
        {
            current = new LocationAppManager();
        }

        protected LocationAppManager()
        {
            locationServiceConnection = new LocationServiceConnection(null);

            locationServiceConnection.ServiceConnected += (object sender, ServiceConnectedEventArgs e) => {

                this.LocationServiceConnected(this, e);
            };
        }

        public static void StartLocationService()
        {
            if(isServiceStarted==false)
            {
                isServiceStarted = true;

                new Task(() => {
                    Intent serviceIntent = new Android.Content.Intent(Android.App.Application.Context, typeof(LocationService));
                    Android.App.Application.Context.StartService(serviceIntent);

                    Intent locationServiceIntent = new Intent(Android.App.Application.Context, typeof(LocationService));
                    Android.App.Application.Context.BindService(locationServiceIntent, locationServiceConnection, Bind.AutoCreate);
                }).Start(); 
            }
     
        }

        public static void StopLocationService()
        {

            if(isServiceStarted)
            {
                isServiceStarted = false;

                try
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
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        #endregion

    }
}


