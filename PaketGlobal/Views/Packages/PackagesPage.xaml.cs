using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;

using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PackagesPage : BasePage
    {
        private PackagesModel ViewModel
        {
            get
            {
                return BindingContext as PackagesModel;
            }
        }

        ICommand RefreshListCommand
        {
            get
            {
                return new Command(async () =>
                {
                    await LoadPackages();
                    PakagesView.IsRefreshing = false;
                });
            }
        }

        private float MaxOffset
        {
            get
            {
#if __IOS__
                if (App.Locator.DeviceService.IsIphoneX() == true)
                {
                    return 120.0f;
                }
                return 135.0f;
#else
                return 150.0f;
#endif
            }
        }

        public PackagesPage()
        {
            InitializeComponent();

            BindingContext = App.Locator.Packages;

            PakagesView.RefreshCommand = RefreshListCommand;

            if (MaxOffset <= 130.0f)
            {
                TitleLabel.TranslationY = 22;
                RightButtons.TranslationY = 22;
            }

#if __ANDROID__
            HeaderView.TranslationY = -30;
            TitleLabel.TranslationY = 0;
#endif

            App.Locator.DeviceService.setStausBarLight();
        }

        protected async override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                await LoadPackages();
            }

            App.Locator.DeviceService.setStausBarLight();

            if (ViewModel.PackagesList.Count>0)
            {
                PlacholderLabel.IsVisible = false;
            }
        }


        private void OnListViewScrolled(object sender, ScrolledEventArgs args)
        {
            var yOffset = args.ScrollY;

            if (yOffset < 0)
            {
                yOffset = 0;
            }
            else if (yOffset > MaxOffset)
            {
                yOffset = MaxOffset;
            }

            PakagesView.TranslateTo(0, (yOffset * (-1)));

            HeaderView.Opacity = 1 - (yOffset / MaxOffset);

            if (HeaderView.Opacity < 0.4f)
            {
                RightButtons.Opacity = TitleLabel.Opacity = (yOffset / MaxOffset);
            }
            else
            {
                RightButtons.Opacity = TitleLabel.Opacity = 0;
            }

            RelativeLayout.SetHeightConstraint(PakagesView, Constraint.RelativeToParent((parent) => { return parent.Height - MaxOffset + 30; }));
        }

        private async Task LoadPackages()
        {
            PlacholderLabel.IsVisible = false;

            await ViewModel.Load();

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
            PlacholderLabel.IsVisible = ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0;
        }


        private async void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) 
                return;

            App.ShowLoading(true);

            var pkgData = (Package)e.SelectedItem;

            PakagesView.SelectedItem = null;

            var package = await PackageHelper.GetPackageDetails(pkgData.PaketId);
            if (package != null)
            {
                var packagePage = new PackageDetailsPage(package);
                await Navigation.PushAsync(packagePage);
            }
            else
            {
                ShowMessage("Error retrieving package details");
            }


            App.ShowLoading(false);

        }

        #region Buttons Actions

        private void LaunchPackageClicked(object sender, EventArgs e)
        {
            var newPackage = new Package()
            {
                //CourierPubkey="GAIDWM24Q6KKCH5PG7Z24B6ODUMCO4NH2APL4ASLMV75INOTQRNMG2CK",
                //RecipientPubkey="GBP7DJE4MHR5UY22NYHIMQDAUOMCY5YMRMQUPX5TFIEM74O4B4EHJKMB"
            };


            var packagePage = new LaunchPackagePage(newPackage);
            Navigation.PushAsync(packagePage);
        }

        private void AcceptPackageClicked(object sender, EventArgs e)
        {
            var packagePage = new AcceptPackagePage();

            var navigationPage = new NavigationPage(packagePage); 

            Navigation.PushModalAsync(navigationPage);
        }

        #endregion
    }
}
