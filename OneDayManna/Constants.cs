using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna
{
    public static class Constants
    {
        public static readonly bool IsDeviceIOS = Device.RuntimePlatform == Device.iOS;
        public static readonly double StatusBarHeight = DependencyService.Get<IStatusBar>().GetHeight();
        public static readonly double height = DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
        public static readonly double width = DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        public static readonly string BIBLE_WEB_ENDPOINT = "https://www.bible.com/ko/bible/1/";
        public static readonly string BIBLE_APP_ENDPOINT = "youversion://bible?reference=";
        public static readonly string MANNA_ENDPOINT = "http://3.138.184.130:9179/api/v1/today-manna/";
        public static readonly string IMAGE_API_ENDPOINT = "https://source.unsplash.com/random/1080x1920/?nature/";
    }
}
