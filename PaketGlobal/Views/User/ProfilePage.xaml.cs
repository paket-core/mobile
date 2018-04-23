using System;
using System.Collections.Generic;
using PaketGlobal.ClientService;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class ProfilePage : BasePage
	{
		public ProfilePage(UserDetails user)
		{
			InitializeComponent();

			Title = "Profile";

			BindingContext = user;
		}
	}
}
