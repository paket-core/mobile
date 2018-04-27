using System;

namespace PaketGlobal
{
	public interface IAccountService
	{
		string UserName { get; }
		string FullName { get; }
		string PhoneNumber { get; }
		string Pubkey { get; }
		string Mnemonic { get; }
		string Transactions { get; set; }
		void SetCredentials(string userName, string fullName, string phoneNumber, string pubkey, string mnemonic);
		void DeleteCredentials();
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
	}

	public interface IScreenScale
	{
		float GetScreenScale();
	}
}
