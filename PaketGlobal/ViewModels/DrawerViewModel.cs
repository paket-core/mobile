using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;

using GalaSoft.MvvmLight.Views;

//using InventoryInspection.Graphics;

namespace PaketGlobal
{
	public class DrawerViewModel : BaseViewModel
	{
		private readonly INavigationService _navigationService;
		
		private string userImageName;
		private string userName;

		private bool isLogged;

		private ObservableCollection<MenuItem> menuItems = new ObservableCollection<MenuItem> ();

		public Action<Page> PageSelected { get; set; }

		public string UserImageName {
			get { return userImageName; }
			set { userImageName = value; OnPropertyChanged ("UserImageName"); }
		}

		public string UserName {
			get { return userName; }
			set { userName = value; OnPropertyChanged ("UserName"); }
		}

		public bool IsLogged {
			get { return isLogged; }
			set {
				isLogged = value;
				OnPropertyChanged ("IsLogged");
				UpdateMenuItems ();
			}
		}

		public ObservableCollection<MenuItem> MenuItems {
			get { return menuItems; }
			set { menuItems = value; OnPropertyChanged ("MenuItems"); }
		}

		public DrawerViewModel (INavigationService navigationService)
		{
			if (navigationService == null) throw new ArgumentNullException("navigationService");
			_navigationService = navigationService;

			//IsLogged = App.SocialLoggedStatus;
			UpdateUserProfile ();

			//MessagingCenter.Subscribe<object, bool> (this, MessagingCenterConstants.OnSocialLoginStatusChangedMessage, (s, e) => {
			//	IsLogged = e;
			//	UpdateUserProfile ();
			//	if (e) {
			//		LeaderboardHelper.SyncLocalScores ();
			//	} else {
			//		LeaderboardHelper.ClearLocalScores ();
			//	}
			//});

			MessagingCenter.Subscribe<object> (this, MessagingCenterConstants.OnSocialProfileRecievedMessage, (s) => {
				UpdateUserProfile ();
			});

			UpdateMenuItems ();
		}

		private void UpdateUserProfile ()
		{
			UserName = "User Name";//!string.IsNullOrEmpty (App.UserName) ? App.UserName : AppResources.UserNameLabel;
			//UserImageName = Images.UserIconDefault; //!string.IsNullOrEmpty (App.UserImage) ? App.UserImage : Images.UserIconDefault;
		}

		public void UpdateMenuItems()
		{
			MenuItems.Clear();

			//MenuItems.Add (new MenuItem {
			//	IconName = Elements.Sampler,
			//	Text = "Plan Sample",
			//	OnItemTouched = () => NavigateTo (new MobileInspectorPage ()),
			//	HasSeparator = true
			//});

			MenuItems.Add(new MenuItem {
				//IconName = Elements.Notifications,
				Text = "Packages",
				OnItemTouched = () => NavigateTo(new PackagesPage()),
				HasSeparator = true
			});

			MenuItems.Add(new MenuItem {
				//IconName = Elements.Add,
				Text = "Wallet",
				OnItemTouched = () => NavigateTo(new WalletPage())
			});

			MenuItems.Add(new MenuItem {
				//IconName = Elements.Dices,
				Text = "Profile",//IsLogged ? AppResources.LogoutLabel : AppResources.LoginLabel,
				OnItemTouched = () => {
					var p = App.Locator.Profile;
					var user = new ClientService.UserDetails() {
						Pubkey = p.Pubkey,
						PaketUser = p.UserName,
						FullName = p.FullName,
						PhoneNumber = p.PhoneNumber
					};
					NavigateTo(new ProfilePage(App.Locator.Profile));
				},
				HasSeparator = true
			});

			MenuItems.Add(new MenuItem {
				//IconName = Elements.Logout,
				Text = "Logout",
				OnItemTouched = () => Workspace.Logout()
			});
		}

		private void NavigateTo (Page page)
		{
			if (PageSelected != null) {
				PageSelected (page);
			}
		}

		private void NavigateTo (string pageName, object parameter = null)
		{
			if (_navigationService.CurrentPageKey != pageName) {
				_navigationService.NavigateTo (pageName, parameter);
			}
		}

		private void LoginItemSelected ()
		{
			//if (!IsLogged) {
			//	NavigateTo (Locator.LoginPage);
			//} else {
			//	Logout ();
			//}
		}

		private void Logout ()
		{
			//DependencyService.Get<ISocialServices> ().SignOut ();
		}
	}
}

