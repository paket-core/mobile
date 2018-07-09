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

            PakagesView.ItemsSource = new List<string>() { "item 1", "item 2", "Item 3" };
		}		
	}
}
