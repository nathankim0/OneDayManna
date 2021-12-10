using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OneDayManna.Popups
{
    public partial class SelectFeaturePopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        private static SelectFeaturePopup _instance;
        public static SelectFeaturePopup Instance => _instance ?? (_instance = new SelectFeaturePopup());

        public EventHandler ShareClicked;
        public EventHandler CopyClicked;
        public EventHandler DownloadClicked;

        public SelectFeaturePopup()
        {
            InitializeComponent();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return false;
        }

        void OnShareClicked(object sender, EventArgs e)
        {
            ShareClicked?.Invoke(this, EventArgs.Empty);
        }

        void OnCopyClicked(object sender, EventArgs e)
        {
            CopyClicked?.Invoke(this, EventArgs.Empty);
        }

        void OnDownloadClicked(object sender, EventArgs e)
        {
            DownloadClicked?.Invoke(this, EventArgs.Empty);
        }
    }
}
