using System;
using UIKit;

namespace PaketGlobal.iOS
{
    public class DeviceService : IDeviceService
    {
        public bool IsIphoneX() {
            return UIScreen.MainScreen.Bounds.Size.Height == 812;
        }

        public void setStausBarLight()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.LightContent,false);
        }


        public void setStausBarBlack()
        {
            UIApplication.SharedApplication.SetStatusBarStyle(UIStatusBarStyle.BlackOpaque,false);
        }
    }
}
