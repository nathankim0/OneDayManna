using System;
using System.Linq;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Services;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public partial class MainPage : ContentPage
    {
        readonly MainPageViewModel viewModel;

        public MainPage()
        {
            InitializeComponent();

            refreshView.Margin = new Thickness(0, Constants.StatusBarHeight, 0, 0);

            viewModel = new MainPageViewModel();
            BindingContext = viewModel;

            ColorPickerPopup.Instance.ColorChanged += (sender, changedColor) =>
            {
                var hex = changedColor.ToHex();
                Preferences.Set("CustomTextColor", hex);
                viewModel.CustomTextColor = changedColor;
            };

            viewModel.IsRefreshing = true;
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            var getImageTask = RestService.Instance.GetRandomImageStream();
            var getMannaTask = MannaDataManager.GetManna(DateTime.Now);

            await Task.WhenAll(getImageTask, getMannaTask);

            var imageStream = getImageTask.Result;
            if (imageStream == null || imageStream == System.IO.Stream.Null)
            {
                bacgroundCachedImage.Source = "image1.jpg";
            }
            else
            {
                bacgroundCachedImage.Source = ImageSource.FromStream(() => imageStream);
            }
            viewModel.IsRefreshing = false;

            if (!getMannaTask.Result)
            {
                await Application.Current.MainPage.DisplayAlert("만나 불러오기 실패", "다시 시도 해주세요", "확인");
            }
            else
            {
                viewModel.MannaContents = MannaDataManager.MannaContents;
                viewModel.Range = MannaDataManager.JsonMannaData.Verse;
            }
        }

        async void Button_Clicked(object sender, EventArgs e)
        {
            if (PopupNavigation.Instance.PopupStack?.Any() ?? true) return;
            if (Navigation.NavigationStack.Contains(ColorPickerPopup.Instance)) return;
            await Navigation.PushPopupAsync(ColorPickerPopup.Instance);
        }
    }
}
