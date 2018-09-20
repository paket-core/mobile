using System;
using System.Collections.Generic;
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
		bool MnemonicGenerated { get; set; }
		void SetCredentials(string userName, string fullName, string phoneNumber, string address, string seed, string mnemonic);
		void DeleteCredentials();
        bool ShowNotifications { get; set; }
        string ActivationAddress { get; set; }
        void SavePackages(List<Package> packages);
	}

	public interface IAppInfoService
	{
		string OSVersion { get; }
		string AppVersion { get; }
		string PackageName { get; }
        string GitCommit { get; }
	}

	public interface INotificationService
	{
        void ShowErrorMessage(string text, bool lengthLong = false, EventHandler eventHandler = null);
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

        string CountryCode();

        void ShowProgress();
        void HideProgress();

        bool IsNeedAlertDialogToClose { get; set; }
    }

    public interface ILocationSharedService
    {
        void StartUpdateLocation();
        void StopUpdateLocation();
    }

    public interface IEventSharedService
    {
        void StartUseEvent();
        void StopUseEvent();
    }
}
