﻿using System;
using System.Threading.Tasks;
using Firebase.Xamarin.Database;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Xamarin.Forms;

namespace PaketGlobal
{
	public class Locator
	{
		public const string MainPage = "MainPage";
		public const string ProfilePage = "ProfilePage";
		public const string PackagesPage = "PackagesPage";
		public const string PackageDetailsPage = "PackageDetailsDetail";
		public const string LaunchPackagePage = "LaunchPackagePage";
		public const string AcceptPackagePage = "AcceptPackagePage";
		public const string WalletPage = "WalletPage";
        public const string RegistrationPage = "RegistrationPage";
        public const string ActivationPage = "ActivationPage";


		public T GetInstance<T>(string key = null)
		{
            try{
                return key != null ? SimpleIoc.Default.GetInstance<T>(key) : SimpleIoc.Default.GetInstance<T>();
                }
            catch (Exception ex){
                return default(T);
            }
		}

		public Profile Profile {
			get { return Workspace.Profile; }
		}

		public Workspace Workspace {
			get { return GetInstance<Workspace>(); }
		}

        public ServiceClient BridgeServiceClient
        {
            get { return GetInstance<ServiceClient>(Config.BridgeService); }
        }

        public ServiceClient RouteServiceClient
        {
            get { return GetInstance<ServiceClient>(Config.RouteService); }
        }

        public ServiceClient IdentityServiceClient
        {
            get { return GetInstance<ServiceClient>(Config.IdentityService); }
        }

		public NavigationService NavigationService {
			get { return (NavigationService)GetInstance<INavigationService>(); }
		}

        public ProfileModel ProfileModel
        {
            get { return SimpleIoc.Default.GetInstance<ProfileModel>(); }
        }

		public PackagesModel Packages {
			get { return SimpleIoc.Default.GetInstance<PackagesModel>(); }
		}

		public WalletModel Wallet {
			get { return SimpleIoc.Default.GetInstance<WalletModel>(); }
		}

        public FriendlyService FriendlyService
        {
            get { return SimpleIoc.Default.GetInstance<FriendlyService>(); }
        }

        public LocationHelper LocationHelper
        {
            get { return GetInstance<LocationHelper>(); }
        }

		public IAccountService AccountService {
			get { return GetInstance<IAccountService>(); }
		}

		public IAppInfoService AppInfoService {
			get { return GetInstance<IAppInfoService>(); }
		}

		public INotificationService NotificationService {
			get { return GetInstance<INotificationService>(); }
		}

        public IClipboardService ClipboardService
        {
            get { return GetInstance<IClipboardService>(); }
        }

        public IDeviceService DeviceService
        {
            get { return GetInstance<IDeviceService>(); }
        }

        public ILocationSharedService LocationService
        {
            get { return GetInstance<ILocationSharedService>(); }
        }

        public IEventSharedService EventService
        {
            get { return GetInstance<IEventSharedService>(); }
        }


       
		/// <summary>
		/// Register all the used ViewModels, Services et. al. with the IoC Container
		/// </summary>
		public Locator()
		{
           // Task.Run(async () => { await DownloadConfig(); }).Wait();

            // Models
            if (!SimpleIoc.Default.IsRegistered<ProfileModel>())
            {
                SimpleIoc.Default.Register<ProfileModel>();
            }

			if (!SimpleIoc.Default.IsRegistered<PackagesModel>()) {
				SimpleIoc.Default.Register<PackagesModel>();
			}

			if (!SimpleIoc.Default.IsRegistered<WalletModel>()) {
				SimpleIoc.Default.Register<WalletModel>();
			}

            if (!SimpleIoc.Default.IsRegistered<FriendlyService>())
            {
                SimpleIoc.Default.Register<FriendlyService>();
            }

            if (!SimpleIoc.Default.IsRegistered<Workspace>()) {
				SimpleIoc.Default.Register<Workspace>(() => new Workspace());
			}

            if (!SimpleIoc.Default.IsRegistered<ServiceClient>(Config.BridgeService))
            {
                SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.BridgeServerUrl,
                                                                                  Config.BridgeServerVersion),
                                                          Config.BridgeService);
            }

            if (!SimpleIoc.Default.IsRegistered<ServiceClient>(Config.RouteService))
            {
                SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.RouteServerUrl,
                                                                                  Config.RouteServerVersion),
                                                          Config.RouteService);
            }

            if (!SimpleIoc.Default.IsRegistered<ServiceClient>(Config.IdentityService))
            {
                SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.IdentityServerUrl,
                                                                                  Config.IdentityServerVersion),
                                                          Config.IdentityService);
            }

            if (!SimpleIoc.Default.IsRegistered<LocationHelper>())
            {
                SimpleIoc.Default.Register<LocationHelper>();
            }

			if (!SimpleIoc.Default.IsRegistered<INavigationService>()) {
				var navigationService = new NavigationService();
				navigationService.Configure(Locator.MainPage, typeof(MainPage));
				navigationService.Configure(Locator.PackagesPage, typeof(PackagesPage));
				navigationService.Configure(Locator.LaunchPackagePage, typeof(LaunchPackagePage));
				navigationService.Configure(Locator.AcceptPackagePage, typeof(AcceptPackagePage));
				navigationService.Configure(Locator.ProfilePage, typeof(ProfilePage));
				navigationService.Configure(Locator.WalletPage, typeof(WalletPage));
                navigationService.Configure(Locator.RegistrationPage, typeof(RegistrationPage));
                navigationService.Configure(Locator.ActivationPage, typeof(ActivationPage));

				SimpleIoc.Default.Register<INavigationService>(() => navigationService);
			}
		}
	}
}

