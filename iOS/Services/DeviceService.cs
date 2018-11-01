using System;
using System.Threading.Tasks;
using CoreTelephony;
using CountlySDK;
using Foundation;
using UIKit;

namespace PaketGlobal.iOS
{
    public class DeviceService : IDeviceService
    {
        private bool isNeedAlertDialogToClose = false;
        private bool isNeedAlertDialogToCloseLaunchPackage = false;

        private string token = null;

        public string FCMToken
        {
            get
            {
                return token;
            }
            set
            {
                token = value;
            }
        }

        public bool IsNeedAlertDialogToClose
        {
            get
            {
                return isNeedAlertDialogToClose;
            }
            set
            {
                isNeedAlertDialogToClose = value;
            }
        }

        public bool IsNeedAlertDialogToCloseLaunchPackage
        {
            get
            {
                return isNeedAlertDialogToCloseLaunchPackage;
            }
            set
            {
                isNeedAlertDialogToCloseLaunchPackage = value;
            }
        }

        public bool IsIphoneX() {
            return UIScreen.MainScreen.Bounds.Size.Height >= 812;
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

        public void ShowProgress()
        {
        }

        public void HideProgress()
        {
        }

        public string CountryName()
        {
            var locale = NSLocale.CurrentLocale;
            var country =  locale.GetCountryCodeDisplayName(locale.CountryCode);
            return country;
        }

        public string CountryCode()
        {
            var network_Info = new CTTelephonyNetworkInfo();
            var carrier = network_Info.SubscriberCellularProvider;

            if(carrier!=null)
            {
                try
                {
                    if (carrier.MobileCountryCode != null)
                    {
                        return carrier.MobileCountryCode;
                    }
                }
                catch
                {
                    return LocaleCountryCode();
                }  
            }
        

            return LocaleCountryCode();
        }

        private string LocaleCountryCode()
        {
            var currentLocale = NSLocale.CurrentLocale;
            var countryCode = currentLocale.CountryCode;

            return countryCode;
        }

        public void SendErrorEvent(string errorMessage, string method)
        {
            var dict = new NSDictionary("error", errorMessage, "method",method);
            Countly.SharedInstance().RecordEvent("Show_Generic_Error", dict);
        }

        public Task<string> OpenAddressBook(){
            var task = new TaskCompletionSource<string>();
            try
            {
                ContactPicker.Select(GetController(), (obj) =>
                {
                    if (obj == null)
                    {
                        task.SetResult(null);
                    }
                    else
                    {
                        var values = obj.GetPhones().GetValues();
                        task.SetResult(values.Length > 0 ? values[0] : null);
                    }
                });
            }
            catch (Exception ex)
            {
                task.SetException(ex);
            }
            return task.Task;
        }

        private UIViewController GetController()
        {
            return UIApplication.SharedApplication.KeyWindow.RootViewController;
        }

        public void StartJobService()
        {
        }
    }
}
