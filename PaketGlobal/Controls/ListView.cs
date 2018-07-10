namespace XF.Controls.Views
{
    using System;
    using Xamarin.Forms;

    /// <summary>
    /// An extended <see cref="ListView"/> with scroll position tracking.
    /// </summary>
    public class ListView : Xamarin.Forms.ListView
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ListView"/> class.
        /// </summary>
        public ListView() : base(ListViewCachingStrategy.RetainElement)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ListView"/> class.
        /// </summary>
        /// <param name="strategy">The caching strategy to use.</param>
        public ListView(ListViewCachingStrategy strategy) : base(strategy)
        {
        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Event that is raised after a scroll completes.
        /// </summary>
        public event EventHandler<ScrolledEventArgs> Scrolled;

        #endregion

        #region Public Methods

        /// <summary>
        /// Method to be called after a scroll completes.
        /// </summary>
        /// <param name="args">The scroll event arguments.</param>
        public void OnScrolled(ScrolledEventArgs args)
        {
            Scrolled?.Invoke(this, args);
        }

        #endregion
    }
}