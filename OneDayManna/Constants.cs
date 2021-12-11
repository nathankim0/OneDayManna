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

        public static readonly string DEFAULT_TEXT_COLOR = "#FFFFFFFF";
        public static readonly string DEFAULT_BACKGROUND_DIM_COLOR = "#40000000";
    }
}

/*
 * 
 100% — FF
95% — F2
90% — E6
85% — D9
80% — CC
75% — BF
70% — B3
65% — A6
60% — 99
55% — 8C
50% — 80
45% — 73
40% — 66
35% — 59
30% — 4D
25% — 40
20% — 33
15% — 26
10% — 1A
5% — 0D
0% — 00

출처: https://dvlv.tistory.com/127 [dev love]
 */