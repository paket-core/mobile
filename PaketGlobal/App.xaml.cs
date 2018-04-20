using System;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using PaketGlobal.ClientService;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class App : Application
	{
		public static bool UseMockDataStore = true;
		public static string BackendUrl = "https://localhost:5000";

		private static Locator _locator;
		public static Locator Locator { get { return _locator ?? (_locator = new Locator()); } }

		public static string AppName {
			get { return Locator.AppInfoService.PackageName; }
		}

		public App()
		{
			InitializeComponent();

			//if (UseMockDataStore)
			//	DependencyService.Register<MockDataStore>();
			//else
				//DependencyService.Register<CloudDataStore>();

			//if (Device.RuntimePlatform == Device.iOS)
			//	MainPage = new MainPage();
			//else
				//MainPage = new NavigationPage(new MainPage());

			if (String.IsNullOrWhiteSpace(Locator.Profile.Pubkey)) {
				var navPage = Locator.NavigationService.Initialize(new LoginPage());
				MainPage = navPage;
			} else {
				MainPage = new MainPage();
			}

			MessagingCenter.Subscribe<Workspace, bool>(this, "Logout", (sender, arg) => {
				var navPage = Locator.NavigationService.Initialize(new LoginPage());
				MainPage = navPage;
			});
		}
	}
}
