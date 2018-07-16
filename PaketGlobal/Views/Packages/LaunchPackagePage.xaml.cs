using System;
using System.Collections.Generic;
using Acr.UserDialogs;
using stellar_dotnetcore_sdk;
using Xamarin.Forms;

namespace PaketGlobal
{
	public partial class LaunchPackagePage : BasePage
	{
     
		private Package ViewModel {
			get {
				return BindingContext as Package;
			}
		}

		public LaunchPackagePage(Package package)
		{
			InitializeComponent();

            BindingContext = package;

#if __IOS__
            if (App.Locator.DeviceService.IsIphoneX() == true)
            {
                TitleLabel.TranslationY = 35;
                BackButton.TranslationY = 10;
            }
            else{
                TitleLabel.TranslationY = 24;
            }
#elif __ANDROID__
            TitleLabel.TranslationY = 5;
            BackButton.TranslationY = -18;
            BackButton.TranslationX = -30;
#endif
        }

        private void OnBack(object sender, System.EventArgs e)
        {
            Navigation.PopAsync();
        }

        private void PickerFocused(object sender, Xamarin.Forms.FocusEventArgs e)
        {
            EntryDeadline.Unfocus();
            Unfocus();

            var dpc = new DatePromptConfig();
            dpc.OkText = "OK";
            dpc.CancelText = "Cancel";
            dpc.IsCancellable = true;
            dpc.MinimumDate = DateTime.Today.AddDays(1);
            dpc.SelectedDate = ViewModel.DeadlineDT.Date;
            dpc.Title = "Please select a Deadline Date";
            dpc.OnAction = dateResult =>
            {
                if (dateResult.Ok)
                {
                    var date = dateResult.SelectedDate.Date;
                    date = date.AddSeconds(86399);//23:59.59 of selected day                    
                    ViewModel.Deadline = DateTimeHelper.ToUnixTime(date.ToUniversalTime());

                    EntryDeadline.Text = ViewModel.DeadlineString;

                    SelectButton(CustomDateButton);
                }
            };

            UserDialogs.Instance.DatePrompt(dpc);
        }

        private void DateButtonClicked(object sender, System.EventArgs e)
        {
            int addedDays = 0;

            if(sender == CustomDateButton) {
                PickerFocused(null, null);
            }
            else if (sender==DayButton){
                addedDays = 1;
            }
            else if (sender==OneWeekButton){
                addedDays = 7;
            }
            else if (sender == TwoWeekButton)
            {
                addedDays = 14;
            }

            if (addedDays>0){
                SelectButton(sender as Button);

                var date = DateTime.Today.Date.AddDays(addedDays);
                date = date.AddSeconds(86399);//23:59.59 of selected day

                ViewModel.Deadline = DateTimeHelper.ToUnixTime(date.ToUniversalTime());

                EntryDeadline.Text = ViewModel.DeadlineString;
            }
        }

        private void SelectButton(Button btn) {
            OneWeekButton.BackgroundColor = Color.White;
            TwoWeekButton.BackgroundColor = Color.White;
            CustomDateButton.BackgroundColor = Color.White;
            DayButton.BackgroundColor = Color.White;

            OneWeekButton.TextColor = Color.FromHex("#4D64E8");
            TwoWeekButton.TextColor = Color.FromHex("#4D64E8");
            CustomDateButton.TextColor = Color.FromHex("#4D64E8");
            DayButton.TextColor = Color.FromHex("#4D64E8");

            btn.BackgroundColor = Color.FromHex("#4D64E8");
            btn.TextColor = Color.White;
        }

        private async void CreateClicked(object sender, System.EventArgs e)
        {
            if (IsValid())
            {
                Unfocus();

                await WithProgressButton(LaunchButton, async () =>
                {
                    var vm = ViewModel;

                    var escrowKP = KeyPair.Random();

                    var recipientPubkey = vm.RecipientPubkey;
                    var courierPubkey = vm.CourierPubkey;

                    //get recipient pubkey if user entered callsign
                    if (recipientPubkey.Length != 56)
                    {
                        var recipientResult = await App.Locator.FundServiceClient.GetUser(null, recipientPubkey);
                        if (recipientResult == null)
                        {
                            ShowMessage("Recipient not found");
                            return;
                        }
                        else
                        {
                            recipientPubkey = recipientResult.UserDetails.Pubkey;
                        }
                    }

                    //get courier pubkey if user entered callsign
                    if (courierPubkey.Length != 56)
                    {
                        var courierResult = await App.Locator.FundServiceClient.GetUser(null, courierPubkey);
                        if (courierResult == null)
                        {
                            ShowMessage("Courier not found");
                            return;
                        }
                        else
                        {
                            courierPubkey = courierResult.UserDetails.Pubkey;
                        }
                    }

 
                    long payment = Convert.ToInt64(Convert.ToDecimal(EntryPayment.Text));
                    long collateral = Convert.ToInt64(Convert.ToDecimal(EntryCollateral.Text));

                    var result = await StellarHelper.LaunchPackage(escrowKP, recipientPubkey, vm.Deadline, courierPubkey, payment, collateral);

                    if (result == StellarOperationResult.Success)
                    {
                        await System.Threading.Tasks.Task.Delay(2000);
                        await App.Locator.Packages.Load();

                        OnBack(BackButton, null);
                    }
                    else
                    {
                        ShowError(result);
                    }

                    App.ShowLoading(false);
                });
            }
        }

        private void FieldCompleted(object sender, System.EventArgs e)
        {
            
        }

        protected override bool IsValid()
        {
            if (!ValidationHelper.ValidateTextField(EntryRecepient.Text))
            {
                EntryRecepient.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(EntryCourier.Text))
            {
                EntryCourier.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateTextField(EntryDeadline.Text))
            {
                ShowMessage("Please select deadline date");
                return false;
            }
            else if (!ValidationHelper.ValidateNumber(EntryPayment.Text))
            {
                EntryPayment.Focus();
                return false;
            }
            else if (!ValidationHelper.ValidateNumber(EntryCollateral.Text))
            {
                EntryCollateral.Focus();
                return false;
            }
    
            return true;
        }

        protected override void ToggleLayout(bool enabled)
        {
            BackButton.IsEnabled = enabled;
            EntryCourier.IsEnabled = enabled;
            EntryPayment.IsEnabled = enabled;
            EntryDeadline.IsEnabled = enabled;
            EntryCollateral.IsEnabled = enabled;
            EntryRecepient.IsEnabled = enabled;
        }
	}
}
