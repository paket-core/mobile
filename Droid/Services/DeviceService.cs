﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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
   

        public bool IsNeedAlertDialogToClose { 
            get{
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
                    if(iso.Length>0)
                    {
                        return iso.ToUpper();
                    }
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

        public Task<string> OpenAddressBook()
        {
            var task = new TaskCompletionSource<string>();
            try
            {
                IntentHelper.OpenContactPicker((path) =>
                {
                    if(path != null)
                    {
                        task.SetResult(path);
                    }
                  
                });
            }
            catch (Exception ex)
            {
                task.SetException(ex);
            }
            return task.Task;
        }

        public void StartJobService()
        {
            MainActivity.Instance.ScheduleJob();
        }
    }
}
