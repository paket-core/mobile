using System;

using Android.Content;
using Android.Content.Res;
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

        public int ScreenHeight()
        {
            return (int)(Android.App.Application.Context.Resources.DisplayMetrics.HeightPixels /Android.App.Application.Context.Resources.DisplayMetrics.Density);
        }

        public int ScreenWidth()
        {
            return (int)(Android.App.Application.Context.Resources.DisplayMetrics.WidthPixels / Android.App.Application.Context.Resources.DisplayMetrics.Density);

        }
    }
}
