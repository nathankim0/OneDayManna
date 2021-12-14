using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace OneDayManna.Popups
{
    public partial class LoadingPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        private static LoadingPopup _instance;
        public static LoadingPopup Instance => _instance ?? (_instance = new LoadingPopup());

        public LoadingPopup()
        {
            InitializeComponent();
        }
    }
}
