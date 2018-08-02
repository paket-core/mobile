using System;

using Xamarin.Forms;

using Acr.UserDialogs;

using stellar_dotnetcore_sdk;

using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

using Plugin.DeviceInfo;

namespace PaketGlobal
{
	public partial class App : Xamarin.Forms.Application
    {
		private static Locator _locator;
		public static Locator Locator { get { return _locator ?? (_locator = new Locator()); } }

		public static string AppName {
			get { return Locator.AppInfoService.PackageName; }
		}

		public App()
        {
            InitializeComponent();

            Xamarin.Forms.Application.Current.On<Xamarin.Forms.PlatformConfiguration.Android>().UseWindowSoftInputModeAdjust(WindowSoftInputModeAdjust.Resize | WindowSoftInputModeAdjust.Pan);              XamEffects.Effects.Init();

			Network.UseTestNetwork();//TODO for test porposals

			Locator.ServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
			Locator.ServiceClient.TrySign = Locator.Profile.SignData;
			Locator.FundServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
			Locator.FundServiceClient.TrySign = Locator.Profile.SignData;

			if (Locator.Profile.Activated) {
                var navigationPage = new NavigationPage(new MainPage()); 

                MainPage = navigationPage;
			} else {
                var navPage = Locator.NavigationService.Initialize(new RestoreKeyPage());
				MainPage = navPage;
			}


			MessagingCenter.Subscribe<Workspace, bool>(this,Constants.LOGOUT, (sender, arg) => {
                var navPage = Locator.NavigationService.Initialize(new RestoreKeyPage());
				MainPage = navPage;
			});
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
                if(isRunning)
                {
                    App.Locator.DeviceService.ShowProgress();  
                }
                else{
                    App.Locator.DeviceService.HideProgress();
                }
            }
            else{
                if (isRunning == true)
                {
                    if (isCancel == true)
                    {
                        UserDialogs.Instance.Loading("", new Action(async () => {
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
    }
}
