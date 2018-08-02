using System;
using System.Threading.Tasks;
using Plugin.Geolocator.Abstractions;

namespace PaketGlobal
{
	public interface IAccountService
	{
		string UserName { get; }
		string FullName { get; }
		string PhoneNumber { get; }
		string Seed { get; }
		string Mnemonic { get; }
		string Transactions { get; set; }
		bool Activated { get; set; }
		void SetCredentials(string userName, string fullName, string phoneNumber, string pubkey, string mnemonic);
		void DeleteCredentials();
        bool ShowNotifications { get; set; }
        string ActivationAddress { get; set; }
	}

	public interface IAppInfoService
	{
		string OSVersion { get; }
		string AppVersion { get; }
		string PackageName { get; }
	}

	public interface INotificationService
	{
		void ShowMessage(string text, bool lengthLong = false);
        void ShowPackageNotification(Package package, Action<string> callback);
        void ShowWalletNotification(string title, string subTitle, Action<string> callback);
	}


    public interface IClipboardService
    {
        string GetTextFromClipboard();
        void SendTextToClipboard(string text);
    }

    public interface IDeviceService
    {
        bool IsIphoneX();
        bool IsIphonePlus();

        void setStausBarLight();
        void setStausBarBlack();

        int ScreenHeight();
        int ScreenWidth();

        void ShowProgress();
        void HideProgress();
    }

    public interface ILocationSharedService
    {
        void StartUpdateLocation();
        void StopUpdateLocation();
        Task<string> GetCurrentLocation();
    }
}
