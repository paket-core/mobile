using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Geolocator;
using Xamarin.Forms;

namespace PaketGlobal
{
    public enum PackagesMode
    {
        All,
        Available
    }

    public class PackagesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AllPackages { get; set; }

        public DataTemplate AvailablePackages { get; set; }

        public DataTemplate FilterPackages { get; set; }

        public DataTemplate NotFoundPackage { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is NotFoundPackage)
                return NotFoundPackage;
            else if (item is FilterPackages)
                return FilterPackages;
            else if (item is AvaiablePackage)
                return AvailablePackages;

            return AllPackages;
        }
    }

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
                    if(Mode==PackagesMode.All)
                    {
                        await LoadPackages();
                    }
                    else{
                        await LoadAvailablePackages();
                    }

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

        private PackagesMode Mode = PackagesMode.All;
        private FilterPackages FilterPackage = new FilterPackages();
        private CancellationTokenSource cancellationTokenSource;  

        public PackagesPage()
        {
            InitializeComponent();

            BindingContext = App.Locator.Packages;

            FilterPackage.Radius = 20;

            PakagesView.RefreshCommand = RefreshListCommand;

            if (MaxOffset <= 130.0f)
            {
                TitleLabel.TranslationY = 22;
                RightButtons.TranslationY = 22;
            }

#if __ANDROID__
            HeaderView.TranslationY = -30;
            TitleLabel.TranslationY = 0;
#else
            if(App.Locator.DeviceService.ScreenWidth()==320)
            {
                PakagesView.TranslationY = 20;
            }
            else{
                PakagesView.TranslationY = 3;
            }
#endif
            AvailableButton.TextColor = Color.LightGray;

            App.Locator.DeviceService.setStausBarLight();

            MessagingCenter.Subscribe<PackageDetailsPage, string>(this, Constants.PACKAGE_ASSIGN, (sender, arg) =>
            {
                AllClicked(AllButton, EventArgs.Empty);
            });
        }

        protected async override void OnAppearing()
        {
            var fl = firstLoad;

            App.Locator.DeviceService.IsNeedAlertDialogToClose = true;

            base.OnAppearing();

            App.Locator.DeviceService.setStausBarLight();

            if (fl)
            {
                await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Location);
                await LoadPackages();
                await App.Locator.Wallet.Load();
                await App.Locator.ProfileModel.Load();
                await App.Locator.RouteServiceClient.AddEvent(Constants.EVENT_APP_START);

                App.Locator.EventService.StartUseEvent();            
            }

            App.Locator.DeviceService.setStausBarLight();

            if (ViewModel.PackagesList.Count>0)
            {
                PlacholderLabel.IsVisible = false;
            }


            ViewModel.CurrentDisplayPackageId = "";
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            App.Locator.DeviceService.IsNeedAlertDialogToClose = false;
        }


        private void OnListViewScrolled(object sender, ScrolledEventArgs args)
        {
            //var yOffset = args.ScrollY;

            //if (yOffset < 0)
            //{
            //    yOffset = 0;
            //}
            //else if (yOffset > MaxOffset)
            //{
            //    yOffset = MaxOffset;
            //}

            //PakagesView.TranslateTo(0, (yOffset * (-1)));

            //HeaderView.Opacity = 1 - (yOffset / MaxOffset);

            //if (HeaderView.Opacity < 0.4f)
            //{
            //    RightButtons.Opacity = TitleLabel.Opacity = (yOffset / MaxOffset);
            //}
            //else
            //{
            //    RightButtons.Opacity = TitleLabel.Opacity = 0;
            //}

            //RelativeLayout.SetHeightConstraint(PakagesView, Constraint.RelativeToParent((parent) => { return parent.Height - MaxOffset + 30; }));
        }

        private async Task LoadPackages()
        {
            PlacholderLabel.IsVisible = false;

            await ViewModel.Load();

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
            PlacholderLabel.IsVisible = ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0;
        }

        private async Task LoadAvailablePackages()
        {
            FilterPackage.IsAvailableRunning = true;
            FilterPackage.IsAvailableCompleted = false;

            if (cancellationTokenSource != null)  {  
                cancellationTokenSource.Cancel();  
            }  

            cancellationTokenSource = new CancellationTokenSource();  

            PlacholderLabel.IsVisible = false;

            if(!ViewModel.AvailablePackagesList.Contains(FilterPackage) && ViewModel.AvailablePackagesList.Count!=0)
            {
                ViewModel.AvailablePackagesList.Insert(0, FilterPackage);
            }

            await ViewModel.LoadAvailable(Convert.ToInt32(FilterPackage.Radius),cancellationTokenSource);

            if (Mode == PackagesMode.Available)
            {
                var list = ViewModel.AvailablePackagesList;
                list.Insert(0, FilterPackage);

                if(list.Count==1)
                {
                    list.Add(new NotFoundPackage());
                }
            }

            if(!cancellationTokenSource.IsCancellationRequested)
            {
                FilterPackage.IsAvailableRunning = false;
                FilterPackage.IsAvailableCompleted = true;  
            }        
        }


        private async void PackageItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null) 
                return;

            PakagesView.SelectedItem = null;

            var pkgData = (Package)PakagesView.SelectedItem;

            if(pkgData.PaketId==null)
            {
                return;
            }

            App.ShowLoading(true);

            var package = await PackageHelper.GetPackageDetails(pkgData.PaketId);
           
            if (package != null)
            {
                ViewModel.CurrentDisplayPackageId = pkgData.PaketId;

                var packagePage = new PackageDetailsPage(package);

                var mainPage = App.Current.MainPage;

                await mainPage.Navigation.PushAsync(packagePage);
            }
            else
            {
                ShowErrorMessage(AppResources.ErrorGetPackage);
            }

            App.ShowLoading(false);
        }

        private async void OnResetFilterClicked(object sender, EventArgs e)
        {
            FilterPackage.Radius = 20;

            await LoadAvailablePackages();
        }

        private void OnSliderValueChanged(object sender, ValueChangedEventArgs args)
        {
            //FilterPackage.Radius = args.NewValue;
           // await LoadAvailablePackages(); 
        }

        private async void OnSliderTouchUp(object sender, System.EventArgs e)
        {
            await LoadAvailablePackages(); 
        }

        private async void AvaliableClicked(object sender, EventArgs e)
        {
            if(Mode==PackagesMode.Available)
            {
                return;
            }

            PakagesView.IsPullToRefreshEnabled = false;

            Mode = PackagesMode.Available;

            AvailableButton.TextColor = Color.White;
            AllButton.TextColor = Color.LightGray;

            AvailableLine.BackgroundColor = Color.FromHex("#53C5C7");
            AllLine.BackgroundColor = Color.Transparent;

            PakagesView.RowHeight = 170;
            PakagesView.SetBinding(ListView.ItemsSourceProperty, "AvailablePackagesList");

            if(ViewModel.AvailablePackagesList == null || ViewModel.AvailablePackagesList.Count == 0)
            {
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsVisible = true; 
            }

            await LoadAvailablePackages();

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
        }

        private async void AllClicked(object sender, EventArgs e)
        {
            if (Mode == PackagesMode.All)
            {
                return;
            }

            PakagesView.IsPullToRefreshEnabled = true;

            Mode = PackagesMode.All;

            AvailableButton.TextColor = Color.LightGray;
            AllButton.TextColor = Color.White;

            AllLine.BackgroundColor = Color.FromHex("#53C5C7");
            AvailableLine.BackgroundColor = Color.Transparent;

            PakagesView.RowHeight = 150;
            PakagesView.SetBinding(ListView.ItemsSourceProperty, "PackagesList");

            if (ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0)
            {
                ActivityIndicator.IsRunning = true;
                ActivityIndicator.IsVisible = true;
            }

            await ViewModel.Load();

            if (Mode == PackagesMode.All)
            {
                PakagesView.ItemsSource = ViewModel.PackagesList;;
            }

            PlacholderLabel.IsVisible = ViewModel.PackagesList == null || ViewModel.PackagesList.Count == 0;

            ActivityIndicator.IsRunning = false;
            ActivityIndicator.IsVisible = false;
        }

        private async void LaunchPackageClicked(object sender, EventArgs e)
        {
            var newPackage = new Package()
            {
            //  CourierPubkey="GAIDWM24Q6KKCH5PG7Z24B6ODUMCO4NH2APL4ASLMV75INOTQRNMG2CK",
            //  RecipientPubkey="GBP7DJE4MHR5UY22NYHIMQDAUOMCY5YMRMQUPX5TFIEM74O4B4EHJKMB"
            };

            var packagePage = new LaunchPackagePage(newPackage);
          
            var mainPage = App.Current.MainPage;

            await mainPage.Navigation.PushAsync(packagePage);
        }

        private async void AcceptPackageClicked(object sender, EventArgs e)
        {
            try{
                var hasPermission = await Utils.CheckPermissions(Plugin.Permissions.Abstractions.Permission.Camera);

                if (hasPermission)
                {
                    var packagePage = new AcceptPackagePage();

                    var mainPage = App.Current.MainPage;

                    await mainPage.Navigation.PushAsync(packagePage);
                }
                else
                {
                    ShowMessage(AppResources.CameraAccessNotGranted);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

    }
}
