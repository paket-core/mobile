[assembly: Xamarin.Forms.ExportRenderer(
    typeof(XF.Controls.Views.ListView),
    typeof(XF.Controls.iOS.Renderers.ListViewRenderer))]

namespace XF.Controls.iOS.Renderers
{
    using UIKit;
    using Xamarin.Forms;
    using PortableListView = Views.ListView;

    /// <summary>
    /// iOS renderer for <see cref="Views.ListView"/>.
    /// </summary>
    public class ListViewRenderer : Xamarin.Forms.Platform.iOS.ListViewRenderer
    {
        #region Protected Methods

        /// <summary>
        /// Called when the portable element changes.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        protected override void OnElementChanged(Xamarin.Forms.Platform.iOS.ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                // Stock XF renderer uses a combined UITableViewSource to configure both UITableViewDataSource and UITableViewDelegate.
                // However, as this class is internal within the XF renderer we can't extend it. So we do this...
                // By overriding just the delegate, we can intercept what we wish and route the rest back to the UITableViewSource in the XF renderer.
                Control.Delegate = new ListViewTableViewDelegate(this);
            }
        }

        #endregion

        #region Private Classes

        /// <summary>
        /// Custom <see cref="UITableViewDelegate"/> to use when <see cref="ListView.HasUnevenRows"/> is false.
        /// Note: This allows us to intercept base renderer lifecycle methods i.e. capture scrolling.
        /// </summary>
        private class ListViewTableViewDelegate : UITableViewDelegate
        {
            #region Fields

            private ListView _element;
            private UITableViewSource _source;

            #endregion

            #region Ctor

            /// <summary>
            /// Initializes a new instance of the <see cref="ListViewTableViewDelegate"/> class.
            /// </summary>
            /// <param name="renderer">The <see cref="ListViewRenderer"/>.</param> 
            public ListViewTableViewDelegate(ListViewRenderer renderer)
            {
                _element = renderer.Element;
                _source = renderer.Control.Source;
            }

            #endregion

            #region Public Methods

            /// <summary>
            /// Handle the dragging ended behavior.
            /// </summary>
            /// <param name="scrollView">The scroll view.</param>
            /// <param name="willDecelerate">If set to <c>true</c> will decelerate.</param>
            public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
            {
                _source.DraggingEnded(scrollView, willDecelerate);
            }

            /// <summary>
            /// Handle the dragging started behavior.
            /// </summary>
            /// <param name="scrollView">The scroll view.</param>
            public override void DraggingStarted(UIScrollView scrollView)
            {
                _source.DraggingStarted(scrollView);
            }

            /// <summary>
            /// Gets the height for a header.
            /// </summary>
            /// <returns>The height for the header.</returns>
            /// <param name="tableView">The <see cref="UITableView"/>.</param>
            /// <param name="section">The section index.</param>
            public override System.nfloat GetHeightForHeader(UITableView tableView, System.nint section)
            {
                return _source.GetHeightForHeader(tableView, section);
            }

            /// <summary>
            /// Gets the view for header.
            /// </summary>
            /// <returns>The view for header.</returns>
            /// <param name="tableView">The <see cref="UITableView"/>.</param>
            /// <param name="section">The section index.</param>
            public override UIView GetViewForHeader(UITableView tableView, System.nint section)
            {
                return _source.GetViewForHeader(tableView, section);
            }

            /// <summary>
            /// Handle the row deselected behavior.
            /// </summary>
            /// <param name="tableView">The <see cref="UITableView"/>.</param>
            /// <param name="indexPath">The index path for the row.</param>
            public override void RowDeselected(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                _source.RowDeselected(tableView, indexPath);
            }

            /// <summary>
            /// Handle the row selected behavior.
            /// </summary>
            /// <param name="tableView">The <see cref="UITableView"/>.</param>
            /// <param name="indexPath">The index path for the row.</param>
            public override void RowSelected(UITableView tableView, Foundation.NSIndexPath indexPath)
            {
                _source.RowSelected(tableView, indexPath);
            }

            /// <summary>
            /// Handle the scroll behavior.
            /// </summary>
            /// <param name="scrollView">The scroll view.</param>
            public override void Scrolled(UIScrollView scrollView)
            {
                _source.Scrolled(scrollView);
                SendScrollEvent(scrollView.ContentOffset.Y);
            }

            #endregion

            #region Private Methods

            /// <summary>
            /// Send the scrolled event to the portable event handler.
            /// </summary>
            /// <param name="y">The vertical content offset.</param>
            private void SendScrollEvent(double y)
            {
                var element = _element as PortableListView;
                var args = new ScrolledEventArgs(0, y);
                element?.OnScrolled(args);
            }

            #endregion
        }

        #endregion
    }
}