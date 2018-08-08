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

    public class EventSharedService : IEventSharedService
    {

        public void StartUseEvent()
        {
            MainActivity.Instance.StartEventsService();
        }

        public void StopUseEvent()
        {
            MainActivity.Instance.StopEventsService();
        }
    }

}