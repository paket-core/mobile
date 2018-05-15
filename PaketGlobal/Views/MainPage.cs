using System;

using Xamarin.Forms;

namespace PaketGlobal
{
	public class MainPage : MasterDetailPage
	{
		//public MainPage()
		//{
			//Page itemsPage, aboutPage = null;

			//switch (Device.RuntimePlatform) {
			//	case Device.iOS:
			//		itemsPage = new NavigationPage(new ItemsPage()) {
			//			Title = "Browse"
			//		};

			//		aboutPage = new NavigationPage(new AboutPage()) {
			//			Title = "About"
			//		};
			//		itemsPage.Icon = "tab_feed.png";
			//		aboutPage.Icon = "tab_about.png";
			//		break;
			//	default:
			//		itemsPage = new ItemsPage() {
			//			Title = "Browse"
			//		};

			//		aboutPage = new AboutPage() {
			//			Title = "About"
			//		};
			//		break;
			//}

			//Children.Add(itemsPage);
			//Children.Add(aboutPage);

			//Title = Children[0].Title;
		//}

		//protected override void OnCurrentPageChanged()
		//{
		//	base.OnCurrentPageChanged();
		//	Title = CurrentPage?.Title ?? string.Empty;
		//}

		public MainPage()
		{
			SetupUserInterface();
		}

		private void SetupUserInterface()
		{
			this.Master = new DrawerPage(this);


			InitNavigation();
		}

		private void InitNavigation()
		{
			SetDetailPage(new PackagesPage ());
		}

		public void SetDetailPage(Page page)
		{
			var navPage = App.Locator.NavigationService.Initialize(page);

			this.Detail = navPage;
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();

			MessagingCenter.Subscribe<object, bool>(this, MessagingCenterConstants.OnDrawerGestureStateChangeMessage, (s, e) => {
				this.IsGestureEnabled = e;
			});
			MessagingCenter.Subscribe<object>(this, MessagingCenterConstants.OnHideDrawerMessage, (s) => {
				this.IsPresented = false;
			});
		}

		protected override void OnDisappearing()
		{
			MessagingCenter.Unsubscribe<object, bool>(this, MessagingCenterConstants.OnDrawerGestureStateChangeMessage);
			MessagingCenter.Unsubscribe<object>(this, MessagingCenterConstants.OnHideDrawerMessage);

			base.OnDisappearing();
		}
	}
}
