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
            MessagingCenter.Unsubscribe<string, string>("MyApp", "DidClickPackageNotification");
            MessagingCenter.Unsubscribe<Workspace, bool>(this, "Logout");
            MessagingCenter.Unsubscribe<string, string>("MyApp", "DidClickWalletNotification");
        }

        private void OpenWallet()
        {
            
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
