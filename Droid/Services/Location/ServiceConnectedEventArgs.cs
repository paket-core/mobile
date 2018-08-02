using System;
using Android.OS;

namespace PaketGlobal.Droid
{
	public class ServiceConnectedEventArgs : EventArgs
	{
		public IBinder Binder { get; set; }
	}
}