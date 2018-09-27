using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.AndroidSpecific;

namespace PaketGlobal
{
    public partial class MainPage : BottomBarPage
    {
        public MainPage()
        {
            InitializeComponent(); 

            this.On<Xamarin.Forms.PlatformConfiguration.Android>().SetIsSwipePagingEnabled(true);

            Unsubscribe();
            Subscribe();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        private void Logout()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            MessagingCenter.Subscribe<string, string>(Constants.NOTIFICATION, Constants.CLICK_PACKAGE_NOTIFICATION, (sender, arg) =>
            {
                OpenPackage(arg);
            });


            MessagingCenter.Subscribe<string, string>(Constants.NOTIFICATION, Constants.CLICK_WALLET_NOTIFICATION, (sender, arg) =>
            {
                OpenWallet();
            });

            MessagingCenter.Subscribe<Workspace, bool>(this,Constants.LOGOUT, (sender, arg) => {
                Logout();
            });

            MessagingCenter.Subscribe<string, string>(Constants.NOTIFICATION, Constants.APP_LAUNCHED_FROM_DEEP_LINK, (sender, arg) =>
            {
                OpenPackageFromDeepLink(arg);
            });
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.CLICK_PACKAGE_NOTIFICATION);
            MessagingCenter.Unsubscribe<Workspace, bool>(this,Constants.LOGOUT);
            MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.CLICK_WALLET_NOTIFICATION);
            MessagingCenter.Unsubscribe<string, string>(Constants.NOTIFICATION, Constants.APP_LAUNCHED_FROM_DEEP_LINK);
        }


        private void OpenWallet()
        {
            
        }

        private async void OpenPackageFromDeepLink(string packageid)
        {
            App.ShowLoading(true);

            var package = await PackageHelper.GetPackageDetails(packageid);
            if (package != null)
            {
                var packagePage = new NewPackageDetailPage(package);
                packagePage.ShouldDismiss = true;

                var mainPage = App.Current.MainPage;

                if(mainPage.Navigation.ModalStack.Count==0)
                {
                    await mainPage.Navigation.PushModalAsync(packagePage, true);
                }
            }
            else
            {
                App.Locator.NotificationService.ShowMessage(AppResources.ErrorGetPackage, false);
            }

            App.ShowLoading(false);
        }

        private void OpenPackage(string packageid)
        {
            //if (App.Locator.Packages.CurrentDisplayPackageId != packageid)
            //{
            //    var currentNavigationPage = (NavigationPage)this.CurrentPage;
            //    var title = currentNavigationPage.Title;
            //    var childrens = this.Children;

            //    if (title == "Packages")
            //    {
            //        this.Navigation.PopToRootAsync(false);
            //    }
            //    else
            //    {
            //        this.CurrentPage = childrens[0];
            //    }
            //}        
        }
    }
}
