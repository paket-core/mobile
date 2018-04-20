using System;
using Xamarin.Forms;
using GalaSoft.MvvmLight.Ioc;
using PaketGlobal.ClientService;
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
			//App.Locator.Workspace.ValidationError += WorkspaceValidationError;
			App.Locator.Workspace.LoggedOut += WorkspaceLoggedOut;
			if (firstLoad) {
				firstLoad = false;
			}
		}

		protected override void OnDisappearing()
		{
			//MessagingCenter.Unsubscribe<object> (this, MessagingCenterConstants.OnApplicationSleepMessage);
			CleanUp();
			App.Locator.Workspace.AuthenticationRequired -= WorkspaceAuthenticationError;
			App.Locator.Workspace.ConnectionError -= WorkspaceConnectionError;
			App.Locator.Workspace.NetworkConnected -= WorkspaceNetworkConnected;
			App.Locator.Workspace.ServiceError -= WorkspaceServiceError;
			//App.Locator.Workspace.ValidationError -= WorkspaceValidationError;
			App.Locator.Workspace.LoggedOut -= WorkspaceLoggedOut;
			if (ForegroundChanged != null) {
				ForegroundChanged(false);
			}
			base.OnDisappearing();
		}

		protected virtual void WorkspaceAuthenticationError(object sender, ServiceClient.UnAuthorizedException args)
		{
			//var serializer = SimpleIoc.Default.GetInstance<ISerializer>();
			//var error = serializer.FromString<ErrorReplyWrapper>(args.Message);

			//if (error != null) {
			//	string message = error.Error;

			//	//switch (error.Error.Code) {
			//	//	case ApiErrorCode.Default:
			//	//		message = "Unexpected service error, please try again later.";
			//	//		break;
			//	//	default:
			//	//		message = "Unexpected service error, please try again later.";
			//	//		break;
			//	//}

			//	ShowError(message);

			//	System.Diagnostics.Debug.WriteLine(args);
			//}

			ShowError(args.Message);//TODO for Debug only

			System.Diagnostics.Debug.WriteLine(args);//TODO for Debug only
		}

		protected virtual void WorkspaceConnectionError(object sender, EventArgs e)
		{
			ShowError("Connection error has occured.");
		}

		protected virtual void WorkspaceNetworkConnected(object sender, EventArgs e)
		{

		}

		protected virtual void WorkspaceServiceError(object sender, ServiceClient.ServiceErrorEventArgs e)
		{
			//string message;

			//switch (e.ServiceException.ApiErrorCode) {
			//	default:
			//		message = "Unexpected service error, please try again later.";
			//		break;
			//}

			//ShowError(message);

			ShowError(e.ServiceException.Message);//TODO for Debug only

			System.Diagnostics.Debug.WriteLine(e.ServiceException);
		}

		protected virtual void WorkspaceLoggedOut(object sender, EventArgs e)
		{
			var navPage = App.Locator.NavigationService.Initialize(new LoginPage());
			Application.Current.MainPage = navPage;
		}

		protected void ShowError(string error, bool lengthLong = false)
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
			ViewHelper.SwitchActivityIndicator(indicator, true);
			ToggleLayout(false);

			await action();

			ToggleLayout(true);
			ViewHelper.SwitchActivityIndicator(indicator, false);
		}

		protected virtual void ToggleLayout(bool enabled)
		{

		}
	}
}
