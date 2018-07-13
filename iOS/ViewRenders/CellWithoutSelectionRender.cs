using System;

using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;

using UIKit;
using Foundation;

using PaketGlobal;
using PaketGlobal.iOS;

[assembly: ExportRenderer(typeof(CellWithoutSelection), typeof(CellWithoutSelectionRender))]
namespace PaketGlobal.iOS
{
    public class CellWithoutSelectionRender : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            var view = item as CellWithoutSelection;
            cell.SelectedBackgroundView = new UIView
            {
                BackgroundColor = UIColor.Clear
            };

            return cell;
        }

    }
}
