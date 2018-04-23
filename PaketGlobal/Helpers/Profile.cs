using System;

namespace PaketGlobal
{
	public class Profile
	{
		public event EventHandler<EventArgs> Changed;

		public string UserName {
			get { return App.Locator.AccountService.UserName; }
		}

		public string FullName {
			get { return App.Locator.AccountService.FullName; }
		}

		public string PhoneNumber {
			get { return App.Locator.AccountService.PhoneNumber; }
		}

		public string Pubkey {
			get { return App.Locator.AccountService.Pubkey; }
		}

		public void SetCredentials (string userName, string fullName, string phoneNumber, string pubkey)
		{
			App.Locator.AccountService.SetCredentials(userName, fullName, phoneNumber, pubkey);
		}

		public void DeleteCredentials ()
		{
			App.Locator.AccountService.DeleteCredentials ();
		}

		protected virtual void OnChanged (EventArgs e)
		{
			if (Changed != null)
				Changed (this, e);
		}
	}
}
