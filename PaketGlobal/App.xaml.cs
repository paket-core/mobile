﻿using System;

using Xamarin.Forms;
using Acr.UserDialogs;

using stellar_dotnetcore_sdk;

namespace PaketGlobal
{
	public partial class App : Application
	{
		private static Locator _locator;
		public static Locator Locator { get { return _locator ?? (_locator = new Locator()); } }

		public static string AppName {
			get { return Locator.AppInfoService.PackageName; }
		}

		public App()
		{
            XamEffects.Effects.Init();

            InitializeComponent();

			Network.UseTestNetwork();//TODO for test porposals

			Locator.ServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
			Locator.ServiceClient.TrySign = Locator.Profile.SignData;
			Locator.FundServiceClient.TryGetPubKey = () => Locator.Profile.Pubkey;
			Locator.FundServiceClient.TrySign = Locator.Profile.SignData;

			if (Locator.Profile.Activated) {
				MainPage = new MainPage();
			} else {
                var navPage = new RegistrationPage();
				MainPage = navPage;
			}

			MessagingCenter.Subscribe<Workspace, bool>(this, "Logout", (sender, arg) => {
                var navPage = new RegistrationPage();
				MainPage = navPage;
			});
		}

		/// <summary>
		/// Shows the loading indicator.
		/// </summary>
		/// <param name="isRunning">If set to <c>true</c> is running.</param>
		/// <param name = "isCancel">If set to <c>true</c> user can cancel the loading event (just uses PopModalAync here)</param>
		public static void ShowLoading(bool isRunning, bool isCancel = false)
		{
			if (isRunning == true) {
				if (isCancel == true) {
					UserDialogs.Instance.Loading("Loading", new Action(async () => {
						if (Application.Current.MainPage.Navigation.ModalStack.Count > 1) {
							await Application.Current.MainPage.Navigation.PopModalAsync();
						} else {
							System.Diagnostics.Debug.WriteLine("Navigation: Can't pop modal without any modals pushed");
						}
						UserDialogs.Instance.Loading().Hide();
					}));
				} else {
					UserDialogs.Instance.Loading(null);
				}
			} else {
				UserDialogs.Instance.Loading().Hide();
			}
		}
	}
}
