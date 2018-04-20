using Xamarin.Forms;

namespace PaketGlobal
{
	public class MenuItemCell : ViewCell
	{
		private Image itemImage = null;
		private Label itemLabel = null;
		private ContentView separatorLayout = null;
		
		public MenuItemCell ()
		{
			SetupView ();
			SetupBindings ();
		}

		private void SetupView ()
		{
            
			//this.BackgroundColor = Color.FromHex("#00796B");

			var cellWrapper = new StackLayout {
				Orientation = StackOrientation.Vertical
			};

			var cellContent = new StackLayout {
				Orientation = StackOrientation.Horizontal,
				Padding = 20,
				Spacing = 30
			};
			cellWrapper.Children.Add (cellContent);

			itemImage = new Image {
				Aspect = Aspect.AspectFit,
				WidthRequest = 35,
				HeightRequest = 35,
				HorizontalOptions = LayoutOptions.Start,
				VerticalOptions = LayoutOptions.Center
			};
			cellContent.Children.Add (itemImage);

			itemLabel = new Label {
                TextColor = Color.White,
				FontSize = DependencyService.Get<IScreenScale> ().GetScreenScale () > 1 ? 22 : 26,
				LineBreakMode = LineBreakMode.TailTruncation,
				HorizontalTextAlignment = TextAlignment.Start,
				VerticalTextAlignment = TextAlignment.Center,
				HorizontalOptions = LayoutOptions.Fill,
				VerticalOptions = LayoutOptions.Center
			};
			cellContent.Children.Add (itemLabel);

			separatorLayout = new ContentView {
				HeightRequest = Device.OnPlatform (0, 1f, 0),
				BackgroundColor = Color.FromHex ("#00695C"),
				HorizontalOptions = LayoutOptions.Fill
			};
			cellWrapper.Children.Add (separatorLayout);

			this.View = cellWrapper;
		}

		private void SetupBindings ()
		{
			//itemImage.SetBinding<MenuItem> (Image.SourceProperty, vm => vm.IconName, converter: new ElementToImageSourceConverter ());
			itemLabel.SetBinding<MenuItem> (Label.TextProperty, vm => vm.Text);
			separatorLayout.SetBinding<MenuItem> (AbsoluteLayout.IsVisibleProperty, vm => vm.HasSeparator);
		}
	}
}

