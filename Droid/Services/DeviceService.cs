using System;
using System.Collections.Generic;
using Android.Content;
using Android.Content.Res;
using Android.Telephony;
using Java.Util;
using LY.Count.Android.Sdk;
using Xamarin.Forms;
using Application = Android.App.Application;

namespace PaketGlobal.Droid
{
    public class DeviceService : IDeviceService
    {
        private bool isNeedAlertDialogToClose = false;

        public bool IsNeedAlertDialogToClose { 
            get{
                return isNeedAlertDialogToClose;
            }
            set
            {
                isNeedAlertDialogToClose = value;
            } 
        }

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

        public void ShowProgress()
        {
            MainActivity.Instance.ShowProgressDialog();
        }

        public void HideProgress()
        {
            MainActivity.Instance.HideProgressDialog();
        }

        public string CountryCode()
        {
            var manager = (TelephonyManager)Application.Context.GetSystemService(Context.TelephonyService);

            if(manager!=null)
            {
                
                var iso = manager.SimCountryIso;

                if(iso!=null)
                {
					return iso.ToUpper();
                }
            }

            return LocaleCountryCode();
        }

        public string CountryName()
        {
            var code = this.CountryCode();

            return Locale.Default.DisplayCountry;
        }

        private string LocaleCountryCode()
        {
            var countryCode = Locale.Default.ISO3Country;
            return countryCode;
        }

        public void SendErrorEvent(string errorMessage, string method)
        {
            Dictionary<String, String> segmentation = new Dictionary<String, String>();
            segmentation.Add("error", errorMessage);
            segmentation.Add("method", method);

            Countly.SharedInstance().RecordEvent("Show_Generic_Error", segmentation, 1);
        }
    }
}
