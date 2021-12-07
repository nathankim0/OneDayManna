using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public partial class MainPage : ContentPage
    {
        MainPageViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            BindingContext = viewModel;

            MannaDataManager.MannaContentsCollectionChanged += Instance_MannaContentsCollectionChanged;

            for(int i = 1; i <= 6; i++)
            {
                viewModel.Images.Add($"image{i}");
            }
        }

        private void Instance_MannaContentsCollectionChanged(object sender, EventArgs e)
        {
            viewModel.MannaContents = MannaDataManager.MannaContents;
            viewModel.Range = MannaDataManager.JsonMannaData.Verse;
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            var isGetMannaCompleted = await MannaDataManager.GetManna(DateTime.Now);

            viewModel.IsRefreshing = false;

            if (!isGetMannaCompleted)
            {
                await Application.Current.MainPage.DisplayAlert("만나 불러오기 실패", "다시 시도 해주세요", "확인");
            }
            else
            {
                viewModel.MannaContents = MannaDataManager.MannaContents;
                viewModel.Range = MannaDataManager.JsonMannaData.Verse;
                var random = new Random();
                viewModel.BackgroundImage = viewModel.Images[random.Next(viewModel.Images?.Count ?? 0)];
            }
        }
    }

    public class MainPageViewModel : BaseViewModel
    {
        private string range;
        public string Range { get => range; set => SetProperty(ref range, value); }

        private ObservableRangeCollection<MannaContent> _mannaContents = new ObservableRangeCollection<MannaContent>();
        public ObservableRangeCollection<MannaContent> MannaContents { get => _mannaContents; set => SetProperty(ref _mannaContents, value); }

        private string backgroundImage = "image1";
        public string BackgroundImage { get => backgroundImage; set => SetProperty(ref backgroundImage, value); }

        private ObservableRangeCollection<string> images = new ObservableRangeCollection<string>();
        public ObservableRangeCollection<string> Images { get => images; set => SetProperty(ref images, value); }
    }

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
