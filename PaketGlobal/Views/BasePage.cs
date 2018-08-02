using System;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using System.Threading.Tasks;

namespace PaketGlobal
{
	public class BasePage : ContentPage
	{
		protected bool firstLoad = true;

		public Action<bool> ForegroundChanged { get; set; }

		public BasePage()
		{
			
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			if (ForegroundChanged != null) {
				ForegroundChanged(true);
			}
			App.Locator.Workspace.AuthenticationRequired += WorkspaceAuthenticationError;
			App.Locator.Workspace.ConnectionError += WorkspaceConnectionError;
			App.Locator.Workspace.NetworkConnected += WorkspaceNetworkConnected;
			App.Locator.Workspace.ServiceError += WorkspaceServiceError;
			App.Locator.Workspace.LoggedOut += WorkspaceLoggedOut;
			if (firstLoad) {
				firstLoad = false;
			}
		}

		protected override void OnDisappearing()
		{
			App.Locator.Workspace.AuthenticationRequired -= WorkspaceAuthenticationError;
			App.Locator.Workspace.ConnectionError -= WorkspaceConnectionError;
			App.Locator.Workspace.NetworkConnected -= WorkspaceNetworkConnected;
			App.Locator.Workspace.ServiceError -= WorkspaceServiceError;
			App.Locator.Workspace.LoggedOut -= WorkspaceLoggedOut;
			if (ForegroundChanged != null) {
				ForegroundChanged(false);
			}
			base.OnDisappearing();
		}

		protected virtual void WorkspaceAuthenticationError(object sender, ServiceClient.UnAuthorizedException args)
		{
			ShowMessage(args.Message);//TODO for Debug only

			System.Diagnostics.Debug.WriteLine(args);//TODO for Debug only
		}

		protected virtual void WorkspaceConnectionError(object sender, EventArgs e)
		{
            ShowMessage(AppResources.ConnectionError);
		}

		protected virtual void WorkspaceNetworkConnected(object sender, EventArgs e)
		{

		}

		protected virtual void WorkspaceServiceError(object sender, ServiceClient.ServiceErrorEventArgs e)
		{
			ShowMessage(e.ServiceException.Message);

			System.Diagnostics.Debug.WriteLine(e.ServiceException);
		}

		protected void ShowError(StellarOperationResult error, bool lengthLong = false)
		{
            var message = StellarOperationResultMethods.GetString(error);

			ShowMessage(message);
		}

        protected virtual void WorkspaceLoggedOut(object sender, EventArgs e)
        {
            var navPage = App.Locator.NavigationService.Initialize(new RestoreKeyPage());
            Application.Current.MainPage = navPage;
        }

		protected void ShowMessage(string error, bool lengthLong = false)
		{
			App.Locator.NotificationService.ShowMessage(error, lengthLong);
		}

		#region Virtual methods

		protected virtual void SetupUserInterface()
		{

		}

		protected virtual void SetupEventHandlers()
		{

		}

		protected virtual void SetupBindings()
		{

		}

		protected virtual void CleanUp()
		{

		}

		protected virtual bool IsValid()
		{
			return true;
		}

		protected virtual void HandleNext()
		{

		}

		#endregion Virtual Methods

		public T GetInstance<T>()
		{
			return SimpleIoc.Default.GetInstance<T>();
		}

		public async Task WithProgress(ActivityIndicator indicator, Func<Task> action)
		{
			await action();
		}

        public async Task WithProgressButton(PaketButton button, Func<Task> action)
        {
            button.IsBusy = true;

            ToggleLayout(false);

            await action();

            ToggleLayout(true);

            button.IsBusy = false;
        }

		protected virtual void ToggleLayout(bool enabled)
		{

		}
	}
}
