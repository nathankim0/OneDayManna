using System;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public class BaseViewModel : ObservableObject
    {
        private bool isRefreshing;
        public bool IsRefreshing { get => isRefreshing; set => SetProperty(ref isRefreshing, value); }

        public INavigation Navigation { get; set; }

        private bool _isNotConnected;
        public bool IsNotConnected { get => _isNotConnected; set => SetProperty(ref _isNotConnected, value); }

        public BaseViewModel()
        {
            IsNotConnected = Connectivity.NetworkAccess != NetworkAccess.Internet;
            Connectivity.ConnectivityChanged += (s, e) => { IsNotConnected = e.NetworkAccess != NetworkAccess.Internet; };
        }
    }
}
