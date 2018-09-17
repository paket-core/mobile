using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace PaketGlobal
{
    public partial class PhotoFullScreenPage : BasePage
    {
        public PhotoFullScreenPage(string photoBase64, string barcodeValue = null)
        {
            InitializeComponent();

            var backCommand = new Command(() =>
            {
                Navigation.PopModalAsync(true);
            });


            BarcodeImage.BarcodeOptions.Width = 300;
            BarcodeImage.BarcodeOptions.Height = 300;
            BarcodeImage.BarcodeOptions.Margin = 1;
            BarcodeImage.BarcodeValue = "1";


            var back = new ToolbarItem
            {
                Text = AppResources.Done,
                Priority = 0,
                Command = backCommand
            };

            ToolbarItems.Add(back);

            if(photoBase64!=null)
            {
                PhotoImage.IsVisible = true;
                PhotoImage.Source = ImageSource.FromStream(
                             () => new MemoryStream(Convert.FromBase64String(photoBase64)));
            }
            else if(barcodeValue!=null)
            {
                BarcodeImage.BarcodeValue = barcodeValue;
                BarcodeImage.IsVisible = true;
            }
           
        }
    }
}
