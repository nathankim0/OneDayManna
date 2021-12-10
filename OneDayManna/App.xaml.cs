using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using OneDayManna.Views;

namespace OneDayManna
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
