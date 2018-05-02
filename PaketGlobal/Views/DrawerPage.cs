using System;
//using InventoryInspection.Graphics;
using Xamarin.Forms;

using XLabs.Forms.Controls;

namespace PaketGlobal
{
	public class DrawerPage : ContentPage
	{
		private WeakReference wMasterPage;
		private CircleImage headerUserImage = null;
		private Label headerUserNameLabel = null;
		private ListView menuItemsListView = null;

		public MainPage MasterPage {
			get { return wMasterPage != null ? wMasterPage.Target as MainPage : null; }
			set { wMasterPage = new WeakReference (value); }
		}
		
		public DrawerPage (MainPage master)
		{
			MasterPage = master;

			Title = "Paket Global";

#if __IOS__
			Icon = "hamburger.png";
#endif

			var vm = App.Locator.DrawerViewModel;
			vm.PageSelected = OnPageSelected;
			BindingContext = vm;

			SetupUserInterface ();
			SetupEventHandlers ();
			SetupBindings ();
		}

		void OnPageSelected (Page page)
		{
			MasterPage.SetDetailPage (page);
		}

		private void SetupUserInterface ()
		{
			this.BackgroundColor = Color.FromHex ("#00796B");

			var contentLayout = new StackLayout {
				Spacing = 0,
				Padding = 0
			};
			this.Content = contentLayout;

			var headerLayout = new RelativeLayout {
				IsClippedToBounds = true,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Start,
				HeightRequest = 100
			};
			headerLayout.BindingContext = App.Locator.Profile;
			contentLayout.Children.Add (headerLayout);

			var headerImageBackground = new Image {
				//Source = ImageSource.FromFile (Images.DrawerHeaderBackground),
				IsOpaque = false,
				Aspect = Aspect.Fill
			};
			headerLayout.Children.Add (headerImageBackground, 
				Constraint.Constant (0),
				Constraint.Constant (0),
				Constraint.RelativeToParent (p => p.Width),
				Constraint.RelativeToParent (p => p.Height)
			);

			headerUserImage = new CircleImage {
				Aspect = Aspect.AspectFit,
				WidthRequest = 90,
				HeightRequest = 90
			};
			headerLayout.Children.Add (headerUserImage,
				Constraint.Constant (30),
				Constraint.Constant (30),
				Constraint.Constant (headerUserImage.WidthRequest),
				Constraint.Constant (headerUserImage.HeightRequest)
			);

			headerUserNameLabel = new Label {
				TextColor = Color.White,
				FontSize = DependencyService.Get<IScreenScale> ().GetScreenScale () > 1 ? 22 : 26,
				LineBreakMode = LineBreakMode.TailTruncation,
				XAlign = TextAlignment.Start,
				YAlign = TextAlignment.Center,
			};
			headerLayout.Children.Add (headerUserNameLabel,
				Constraint.Constant (30),
				Constraint.RelativeToView (headerUserImage, (p, s) => s.Y + s.Height),
				Constraint.RelativeToParent (p => p.Width - 40),
				Constraint.RelativeToView (headerUserImage, (p, s) => p.Height - (s.Y + s.Height))
			);
			
			menuItemsListView = new ListView {
				Header = new Label { HeightRequest = 0 },
				Footer = new Label { HeightRequest = 0 },
				ItemTemplate = new DataTemplate (typeof(MenuItemCell)),
				HasUnevenRows = true,
				SeparatorVisibility = SeparatorVisibility.None,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.FillAndExpand,
				BackgroundColor = Color.Transparent,
			};
			contentLayout.Children.Add (menuItemsListView);
		}

		private void SetupEventHandlers ()
		{
			menuItemsListView.ItemSelected += (sender, e) => {;
				menuItemsListView.SelectedItem = null;
				if(e.SelectedItem != null) {
					var menuItem = e.SelectedItem as MenuItem;
					if(menuItem != null) {
						if(menuItem.OnItemTouched != null) {
							MessagingCenter.Send<object> (this, MessagingCenterConstants.OnHideDrawerMessage);
							menuItem.OnItemTouched ();
						}
					}
				}
			};
		}

		private void SetupBindings ()
		{
			//headerUserImage.SetBinding<DrawerViewModel> (Image.SourceProperty, vm => vm.UserImageName);
			headerUserNameLabel.SetBinding<Profile> (Label.TextProperty, vm => vm.UserName);
			menuItemsListView.SetBinding<DrawerViewModel> (ListView.ItemsSourceProperty, vm => vm.MenuItems);
		}
	}
}

