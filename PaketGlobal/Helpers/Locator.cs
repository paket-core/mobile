using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using PaketGlobal.ClientService;

namespace PaketGlobal
{
	public class Locator
	{
		public const string MainPage = "MainPage";
		public const string LoginPage = "LoginPage";
		public const string ProfilePage = "ProfilePage";
		public const string PackagesPage = "PackagesPage";
		public const string WalletPage = "WalletPage";

		public T GetInstance<T>()
		{
			return SimpleIoc.Default.GetInstance<T>();
		}

		public Profile Profile {
			get { return Workspace.Profile; }
		}

		public Workspace Workspace {
			get { return GetInstance<Workspace>(); }
		}

		public ServiceClient ServiceClient {
			get { return GetInstance<ServiceClient>(); }
		}

		public NavigationService NavigationService {
			get { return (NavigationService)GetInstance<INavigationService>(); }
		}

		public PackagesModel Packages {
			get { return SimpleIoc.Default.GetInstanceWithoutCaching<PackagesModel>(); }
		}

		public DrawerViewModel DrawerViewModel {
			get { return SimpleIoc.Default.GetInstance<DrawerViewModel>(); }
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

		/// <summary>
		/// Register all the used ViewModels, Services et. al. with the IoC Container
		/// </summary>
		public Locator ()
		{
			// Models
			SimpleIoc.Default.Register<DrawerViewModel>();
			SimpleIoc.Default.Register<PackagesModel> ();

			if (!SimpleIoc.Default.IsRegistered<Workspace>()) {
				SimpleIoc.Default.Register<Workspace>(() => new Workspace());
			}

			if (!SimpleIoc.Default.IsRegistered<ServiceClient>()) {
				SimpleIoc.Default.Register<ServiceClient>(() => new ServiceClient(Config.ServerUrl));
			}

			if (!SimpleIoc.Default.IsRegistered<INavigationService>()) {
				var navigationService = new NavigationService();
				navigationService.Configure(Locator.MainPage, typeof(MainPage));
				//navigationService.Configure(Locator.LoginPage, typeof(LoginPage));
				//navigationService.Configure(Locator.PackagesPage, typeof(PackagesPage));
				//navigationService.Configure(Locator.ProfilePage, typeof(ProfilePage));
				//navigationService.Configure(Locator.WalletPage, typeof(WalletPage));

				SimpleIoc.Default.Register<INavigationService>(() => navigationService);
			}
		}
	}
}

