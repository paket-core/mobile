using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class ProfilePage : BasePage
	{
		public ProfilePage(Profile user)
		{
			InitializeComponent();

			Title = "Profile";

			BindingContext = user;

			ToolbarItems.Add(new ToolbarItem("Logout", null, OnLogoutClicked));
		}

		void OnLogoutClicked()
		{
			App.Locator.Workspace.Logout();
		}

		void ShowClicked(object sender, System.EventArgs e)
		{
			btnShow.IsVisible = false;
			entryMnemonic.IsVisible = true;
		}
	}
}
