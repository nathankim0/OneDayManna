using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public class MainPageViewModel : BaseViewModel
    {
        private const string DEFAULT_TEXT_COLOR = "#FFFFFFFF";

        private string range;
        public string Range { get => range; set => SetProperty(ref range, value); }

        private ObservableRangeCollection<MannaContent> mannaContents = new ObservableRangeCollection<MannaContent>();
        public ObservableRangeCollection<MannaContent> MannaContents { get => mannaContents; set => SetProperty(ref mannaContents, value); }

        private Color customTextColor = Color.FromHex(Preferences.Get("CustomTextColor", DEFAULT_TEXT_COLOR));
        public Color CustomTextColor { get => customTextColor; set => SetProperty(ref customTextColor, value); }

        public bool IsAndroid { get => !Constants.IsDeviceIOS; }
    }
}
