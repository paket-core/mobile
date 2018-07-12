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
                    return 130.0f;
                }
                return 150.0f;
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
                TitleLabel.TranslationY = 18;
                RightButtons.TranslationY = 18;
            }

#if __ANDROID__
            HeaderView.TranslationY = -30;
            TitleLabel.TranslationY = 0;
#endif
        }

        protected async override void OnAppearing()
        {
            var fl = firstLoad;

            base.OnAppearing();

            if (fl)
            {
                await LoadPackages();
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

            RelativeLayout.SetHeightConstraint(PakagesView, Constraint.RelativeToParent((parent) => { return parent.Height - 60; }));
        }

        private async Task LoadPackages()
        {
            PlacholderLabel.IsVisible = false;

            await ViewModel.Load();

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
            PlacholderLabel.IsVisible = ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0;
        }


        private void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) 
                return;

            var packagePage = new PackageDetailsPage(PakagesView.SelectedItem as Package);
            Navigation.PushAsync(packagePage);

            PakagesView.SelectedItem = null;
        }

        #region Buttons Actions

        private void LaunchPackageClicked(object sender, EventArgs e)
        {
            var newPackage = new Package()
            {
                //CourierPubkey="SBZVFQY5TUX3EA2MWF2V2KGJJQ2LVQACIOAUTETMVHVTFN7VLG7WUXVH",
                //RecipientPubkey="SBZVFQY5TUX3EA2MWF2V2KGJJQ2LVQACIOAUTETMVHVTFN7VLG7WUXVH"
            };

            var packagePage = new LaunchPackagePage(newPackage);
            Navigation.PushAsync(packagePage);
        }

        private void AcceptPackageClicked(object sender, EventArgs e)
        {
            App.Locator.NavigationService.NavigateTo(Locator.AcceptPackagePage);
        }

        #endregion
    }
}
