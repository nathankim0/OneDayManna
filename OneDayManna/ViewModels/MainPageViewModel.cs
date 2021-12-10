using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public class MainPageViewModel : BaseViewModel
    {
        private string range;
        public string Range { get => range; set => SetProperty(ref range, value); }

        private ObservableRangeCollection<MannaContent> mannaContents = new ObservableRangeCollection<MannaContent>();
        public ObservableRangeCollection<MannaContent> MannaContents { get => mannaContents; set => SetProperty(ref mannaContents, value); }

        private Color customTextColor = Color.FromHex(Preferences.Get("CustomTextColor", Constants.DEFAULT_TEXT_COLOR));
        public Color CustomTextColor { get => customTextColor; set => SetProperty(ref customTextColor, value); }

        private double customFontSize = Preferences.Get("TextSize", 17);
        public double CustomFontSize { get => customFontSize; set => SetProperty(ref customFontSize, value); }

        public bool IsAndroid { get => !Constants.IsDeviceIOS; }

        private bool isAllSelected = false;
        public bool IsAllSelected { get => isAllSelected; set => SetProperty(ref isAllSelected, value); }
    }
}
