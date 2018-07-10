using System;

using System.Collections.Generic;
using System.Windows.Input;

using Xamarin.Forms;
using XFGloss;

namespace PaketGlobal
{
	public partial class PackagesPage : BasePage
	{
		public PackagesPage()
		{
            InitializeComponent();

            PakagesView.ItemsSource = new List<string>() { "item 1", "item 2", "Item 3" , "Item 3", "Item 3", "Item 3", "Item 3", "Item 3", "Item 3" };
		}


        private void OnListViewScrolled(object sender, ScrolledEventArgs args)
        {
            Console.WriteLine($"Offset = {args.ScrollY}");

            var yOffset = args.ScrollY;

            if(yOffset<0)
            {
                yOffset = 0;
            }
            else if(yOffset>150)
            {
                yOffset = 150;
            }

            var opacity = 1 - (yOffset / 150.0f);

            HeaderView.Opacity = opacity;
          //  HeaderView.TranslateTo(0, (yOffset * (-1)));
            PakagesView.TranslateTo(0, (yOffset * (-1)));
        }

	}
}
