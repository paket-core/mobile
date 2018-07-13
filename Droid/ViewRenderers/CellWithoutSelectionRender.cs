using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

using Android.Graphics.Drawables;
using Android.Content;
using Android.Views;

using PaketGlobal;
using PaketGlobal.Droid;

using System.ComponentModel;

[assembly: ExportRenderer(typeof(CellWithoutSelection), typeof(CellWithoutSelectionRender))]

namespace PaketGlobal.Droid
{
    public class CellWithoutSelectionRender : ViewCellRenderer
    {

        private Android.Views.View _cellCore;
        private Drawable _unselectedBackground;
        private bool _selected;

        protected override Android.Views.View GetCellCore(Cell item,
                                                          Android.Views.View convertView,
                                                          ViewGroup parent,
                                                          Context context)
        {
            _cellCore = base.GetCellCore(item, convertView, parent, context);

            _selected = false;
            _unselectedBackground = _cellCore.Background;

            return _cellCore;
        }

        protected override void OnCellPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            base.OnCellPropertyChanged(sender, args);

            if (args.PropertyName == "IsSelected")
            {
                _selected = !_selected;

                if (_selected)
                {
                    _cellCore.SetBackgroundColor(Color.White.ToAndroid());
                }
                else
                {
                    _cellCore.SetBackground(_unselectedBackground);
                }
            }
        }
    }

}