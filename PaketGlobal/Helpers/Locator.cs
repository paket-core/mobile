﻿using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;

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
			return key != null ? SimpleIoc.Default.GetInstance<T>(key) : SimpleIoc.Default.GetInstance<T>();
		}

		public Profile Profile {
			get { return Workspace.Profile; }
		}

		public Workspace Workspace {
			get { return GetInstance<Workspace>(); }
		}

		public ServiceClient ServiceClient {
			get { return GetInstance<ServiceClient>("PackageService"); }
		}

		public ServiceClient FundServiceClient {
			get { return GetInstance<ServiceClient>("FundService"); }
		}

		public NavigationService NavigationService {
			get { return (NavigationService)GetInstance<INavigationService>(); }
		}

		public PackagesModel Packages {
			get { return SimpleIoc.Default.GetInstance<PackagesModel>(); }
		}

		public WalletModel Wallet {
			get { return SimpleIoc.Default.GetInstance<WalletModel>(); }
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

		/// <summary>
		/// Register all the used ViewModels, Services et. al. with the IoC Container
		/// </summary>
		public Locator()
		{
			// Models

			if (!SimpleIoc.Default.IsRegistered<PackagesModel>()) {
				SimpleIoc.Default.Register<PackagesModel>();
			}

			if (!SimpleIoc.Default.IsRegistered<WalletModel>()) {
				SimpleIoc.Default.Register<WalletModel>();
			}

			if (!SimpleIoc.Default.IsRegistered<Workspace>()) {
				SimpleIoc.Default.Register<Workspace>(() => new Workspace());
			}

			if (!SimpleIoc.Default.IsRegistered<ServiceClient>("PackageService")) {
				SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.ServerUrl,
																				  Config.ServerVersion,
																				  Config.PrefundTestUrl),
														  "PackageService");
			}

			if (!SimpleIoc.Default.IsRegistered<ServiceClient>("FundService")) {
				SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.FundServerUrl,
																				  Config.FundServerVersion,
																				  Config.PrefundTestUrl),
														  "FundService");
			}

			if (!SimpleIoc.Default.IsRegistered<INavigationService>()) {
				var navigationService = new NavigationService();
				navigationService.Configure(Locator.MainPage, typeof(MainPage));
				navigationService.Configure(Locator.PackagesPage, typeof(PackagesPage));
				navigationService.Configure(Locator.PackageDetailsPage, typeof(PackageDetailsPage));
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

