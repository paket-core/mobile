using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class MainPage : BottomBarPage
    {
        public MainPage()
        {
            InitializeComponent(); 

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
            MessagingCenter.Subscribe<string, string>("MyApp", "DidClickPackageNotification", (sender, arg) =>
            {
                OpenPackage(arg);
            });


            MessagingCenter.Subscribe<string, string>("MyApp", "DidClickWalletNotification", (sender, arg) =>
            {
                OpenWallet();
            });

            MessagingCenter.Subscribe<Workspace, bool>(this, "Logout", (sender, arg) => {
                Logout();
            });

            MessagingCenter.Subscribe<string, string>("MyApp", "AppLaunchedFromDeepLink", (sender, arg) =>
            {
                OpenPackageFromDeepLink(arg);
            });
        }

        private void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<string, string>("MyApp", "DidClickPackageNotification");
            MessagingCenter.Unsubscribe<Workspace, bool>(this, "Logout");
            MessagingCenter.Unsubscribe<string, string>("MyApp", "DidClickWalletNotification");
            MessagingCenter.Unsubscribe<string, string>("MyApp", "AppLaunchedFromDeepLink");
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
                var packagePage = new PackageDetailsPage(package);
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
