using System;

using Android.Content;

using Xamarin.Forms;

namespace PaketGlobal.Droid
{
    public class DeviceService : IDeviceService
    {
        public bool IsIphoneX()
        {
            return false;
        }

        public bool IsIphonePlus()
        {
            return false;
        }

        public void setStausBarLight()
        {
        }


        public void setStausBarBlack()
        {
        }
    }
}
