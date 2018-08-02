using System;

using Android.Content;
using Android.OS;
using Android.Util;

namespace PaketGlobal.Droid
{
	public class LocationServiceConnection : Java.Lang.Object, IServiceConnection
	{
		public event EventHandler<ServiceConnectedEventArgs> ServiceConnected = delegate {};

        public LocationServiceBinder Binder
		{
			get { return this.binder; }
			set { this.binder = value; }
		}
        protected LocationServiceBinder binder;

        public LocationServiceConnection (LocationServiceBinder binder)
		{
			if (binder != null) {
				this.binder = binder;
			}
		}

		public void OnServiceConnected (ComponentName name, IBinder service)
		{
            LocationServiceBinder serviceBinder = service as LocationServiceBinder;
			if (serviceBinder != null) {
				this.binder = serviceBinder;
				// raise the service connected event
				this.ServiceConnected(this, new ServiceConnectedEventArgs () { Binder = service } );
			}
		}
			
		public void OnServiceDisconnected (ComponentName name)
		{
		}
	}
}

