﻿using System;
using Xamarin.Forms;
using static PaketGlobal.ServiceClient;

namespace PaketGlobal
{
	public class Workspace
	{
		public event EventHandler<EventArgs> NetworkConnected;
		public virtual void OnNetworkConnected (EventArgs e)
		{
			if (NetworkConnected != null)
				NetworkConnected (this, e);
		}

		public event EventHandler<EventArgs> ConnectionError;
		protected internal virtual void OnConnectionError (EventArgs e)
		{
			if (ConnectionError != null)
				ConnectionError (this, e);
		}

		public event EventHandler<UnAuthorizedException> AuthenticationRequired;
		protected internal virtual void OnAuthenticationRequired (UnAuthorizedException e)
		{
			if (AuthenticationRequired != null)
				AuthenticationRequired (this, e);
		}

		public event EventHandler<ServiceErrorEventArgs> ServiceError;
		protected internal virtual void OnServiceError (ServiceErrorEventArgs e)
		{
			if (ServiceError != null)
				ServiceError (this, e);
		}

		public event EventHandler LoggedOut;
		protected internal virtual void OnLoggedOut ()
		{
			LoggedOut?.Invoke (this, EventArgs.Empty);
		}

		public Profile Profile {
			get {
				return _profile;
			}
		}
		Profile _profile;

		public Workspace ()
		{
			_profile = new Profile();
		}

		public void Logout ()
		{
            App.Locator.EventService.StopUseEvent();

            App.Locator.LocationService.StopUpdateLocation();

            App.Locator.AccountService.ActivationAddress = "";
            
			Profile.DeleteCredentials ();

			MessagingCenter.Send(this,Constants.LOGOUT, true);
		}
	}
}
