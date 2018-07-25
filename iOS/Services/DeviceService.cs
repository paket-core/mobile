using System;
using UIKit;

namespace PaketGlobal.iOS
{
    public class DeviceService : IDeviceService
    {
        public bool IsIphoneX() {
            return UIScreen.MainScreen.Bounds.Size.Height == 812;
        }

        public bool IsIphonePlus()
        {
            return UIScreen.MainScreen.Bounds.Size.Width == 414;
        }


        public void setStausBarLight()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent,false);
        }


        public void setStausBarBlack()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.Default,false);
        }

        public int ScreenHeight()
        {
            return (int)(UIScreen.MainScreen.Bounds.Size.Height);
        }

        public int ScreenWidth()
        {
            return (int)(UIScreen.MainScreen.Bounds.Size.Width);

        }
    }
}
