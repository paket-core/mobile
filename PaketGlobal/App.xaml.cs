using System;

using Xamarin.Forms;

using Acr.UserDialogs;

using stellar_dotnetcore_sdk;

using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Plugin.DeviceInfo;
using Newtonsoft.Json;
using Firebase.Xamarin.Database;
using System.Collections.Generic;

namespace PaketGlobal
{
    class FierbaseResponse{
        public int time_interval = 0;
    }

    public partial class App : Xamarin.Forms.Application
    {
        private static Locator _locator;
        public static Locator Locator { get { return _locator ?? (_locator = new Locator()); } }

        public static string AppName
        {
            get { return Locator.AppInfoService.PackageName; }
        }

        public App()
        {
            // This lookup NOT required for Windows platforms - the Culture will be automatically set
            if (Device.RuntimePlatform == Device.iOS || Device.RuntimePlatform == Device.Android)
            {
                // determine the correct, supported .NET culture
                var ci = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
                AppResources.Culture = ci; // set the RESX for resource localization
                DependencyService.Get<ILocalize>().SetLocale(ci); // set the Thread for locale-aware methods
            }

            try
            {
                if (Xamarin.Forms.Application.Current.Properties.ContainsKey(Config.BridgeService))
                {
                    object bridgeService;
                    object fundService;
                    object routeService;

                    Xamarin.Forms.Application.Current.Properties.TryGetValue(Config.BridgeService, out bridgeService);
                    Xamarin.Forms.Application.Current.Properties.TryGetValue(Config.IdentityService, out fundService);
                    Xamarin.Forms.Application.Current.Properties.TryGetValue(Config.RouteService, out routeService);

                    Config.BridgeServerUrl = (bridgeService as string);
                    Config.IdentityServerUrl = (fundService as string);
                    Config.RouteServerUrl = (routeService as string);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            try
            {
                if (Xamarin.Forms.Application.Current.Properties.ContainsKey(Config.UpdateTimeIntervalKey))
                {
                    object interval;

                    Xamarin.Forms.Application.Current.Properties.TryGetValue(Config.UpdateTimeIntervalKey, out interval);

                    if (interval != null)
                    {
                        Config.UpdateTimeInterval = Convert.ToInt32(interval as string);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            InitializeComponent();

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize | WindowSoftInputModeAdjust.Pan);              XamEffects.Effects.Init();

            Network.UseTestNetwork();//TODO for test porposals

            Locator.IdentityServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
            Locator.IdentityServiceClient.TrySign = Locator.Profile.SignData;
            Locator.RouteServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
            Locator.RouteServiceClient.TrySign = Locator.Profile.SignData;
            Locator.BridgeServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
            Locator.BridgeServiceClient.TrySign = Locator.Profile.SignData;

            if (Locator.Profile.Activated)
            {
                var navigationPage = new NavigationPage(new MainPage());

                App.Locator.AccountService.SetPubKey(App.Locator.Profile.Pubkey);

                MainPage = navigationPage;
            }
            else
            {
                var navPage = Locator.NavigationService.Initialize(new WellcomePage());
                MainPage = navPage;
            }


            MessagingCenter.Subscribe<Workspace, bool>(this, Constants.LOGOUT, (sender, arg) =>
            {
                var navPage = Locator.NavigationService.Initialize(new WellcomePage());
                MainPage = navPage;
            });

            MessagingCenter.Subscribe<Workspace, bool>(this, Constants.CHANGE_LANGUAGE, (sender, arg) =>
            {
                var navigationPage = new NavigationPage(new MainPage());
                MainPage = navigationPage;
            });

            DownloadConfig();
        }

        /// <summary>
        /// Shows the loading indicator.
        /// </summary>
        /// <param name="isRunning">If set to <c>true</c> is running.</param>
        /// <param name = "isCancel">If set to <c>true</c> user can cancel the loading event (just uses PopModalAync here)</param>
        public static void ShowLoading(bool isRunning, bool isCancel = false)
        {
            if (CrossDeviceInfo.Current.Model.ToLower().Contains("htc_m10h"))
            {
                if (isRunning)
                {
                    App.Locator.DeviceService.ShowProgress();
                }
                else
                {
                    App.Locator.DeviceService.HideProgress();
                }
            }
            else
            {
                if (isRunning == true)
                {
                    if (isCancel == true)
                    {
                        UserDialogs.Instance.Loading("", new Action(async () =>
                        {
                            if (Xamarin.Forms.Application.Current.MainPage.Navigation.ModalStack.Count > 1)
                            {
                                await Xamarin.Forms.Application.Current.MainPage.Navigation.PopModalAsync();
                            }
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("Navigation: Can't pop modal without any modals pushed");
                            }
                            UserDialogs.Instance.Loading().Hide();
                        }));
                    }
                    else
                    {
                        UserDialogs.Instance.Loading("");
                    }
                }
                else
                {
                    UserDialogs.Instance.Loading().Hide();
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.START_APP, "");
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.STOP_APP, "");
        }

        protected override void OnStart()
        {
            base.OnStart();

            MessagingCenter.Send<string, string>(Constants.NOTIFICATION, Constants.START_APP, "");
        }

        #region Firebase config

        async void DownloadConfig()
        {
            try{
                var firebase = new FirebaseClient(Config.GoogleFirebase);

                var items = await firebase
                    .Child("config")
                    .OnceAsync<FierbaseResponse>();

                if (items != null)
                {
                    foreach (var item in items)
                    {
                        if(item.Object.time_interval != 0)
                        {
                            Config.UpdateTimeInterval = item.Object.time_interval;

                            Xamarin.Forms.Application.Current.Properties[Config.UpdateTimeIntervalKey] = Convert.ToString(Config.UpdateTimeInterval);
                            await  Xamarin.Forms.Application.Current.SavePropertiesAsync();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            App.Locator.DeviceService.StartJobService();
        }


        #endregion
    }
}
