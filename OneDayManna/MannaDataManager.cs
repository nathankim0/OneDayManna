using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Diagnostics;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Collections.Generic;

namespace OneDayManna
{
    public static class MannaDataManager
    {
        public static event EventHandler MannaContentsCollectionChanged;

        private static readonly RestService _restService = new RestService();

        private static readonly string _bibleWebUrlEndPoint = "https://www.bible.com/ko/bible/1/";
        private static readonly string _bibleAppUrlEndPoint = "youversion://bible?reference=";
        private static readonly string MannaEndpoint = "http://3.138.184.130:9179/api/v1/today-manna/";

        public static ObservableRangeCollection<MannaContent> MannaContents = new ObservableRangeCollection<MannaContent>();
        public static JsonMannaModel JsonMannaData = new JsonMannaModel();

        public static string Today { get; set; } = DateTime.Now.ToString("yyyy년 MM월 dd일 (ddd)");
        public static string DisplayDateRange { get; set; } = DateTime.Now.ToString("MM/dd");
        public static string AllMannaTexts { get; set; } = "";
        public static string MannaShareRange { get; set; } = "";

        public static string BibleWebUrl = "";
        public static string BibleAppUrl = "";

        private static readonly string EXCEPTION_COMMON_TEXT = "Exception occured at";

        public static async Task<bool> GetManna(DateTime dateTime)
        {
            Today = dateTime.ToString("yyyy년 MM월 dd일 (ddd)");
            DisplayDateRange = dateTime.ToString("MM/dd");

            var endPoint = MannaEndpoint + dateTime.ToString("yyyy-MM-dd");

            try
            {
                JsonMannaData = await _restService.GetMannaDataAsync(endPoint);

                MannaShareRange = $"만나: {JsonMannaData.Verse}";

                var bookAndJang = ExtractBookAndJang();

                SetBibleWebAndAppUrl(bookAndJang);
                SetMannaCollection(bookAndJang);

                MannaContentsCollectionChanged?.Invoke(null, EventArgs.Empty);

                return true;
            }
            catch (Exception e)
            {
                ExceptionCommonText("GetManna()", e.Message);
                return false;
            }

        }

        private static void SetMannaCollection(string bookAndJang)
        {
            var mannaContents = new List<MannaContent>();
            var allMannaTexts = string.Empty;

            foreach (var node in JsonMannaData.Contents)
            {
                var onlyNum = 0;
                var onlyString = "";

                try { onlyString = Regex.Replace(node, @"\d", "").Substring(1); }
                catch (Exception e) { ExceptionCommonText("SetMannaCollection onlystring", e.Message); }

                try { onlyNum = int.Parse(Regex.Replace(node, @"\D", "")); }
                catch (Exception e) { ExceptionCommonText("SetMannaCollection onlyNum", e.Message); }

                var verse = bookAndJang + onlyNum;

                mannaContents.Add(new MannaContent
                {
                    Number = onlyNum,
                    Verse = verse,
                    MannaString = onlyString,
                });

                allMannaTexts += node + "\n\n";
            }

            MannaContents = new ObservableRangeCollection<MannaContent>(mannaContents);
            AllMannaTexts = allMannaTexts;
        }

        private static void SetBibleWebAndAppUrl(string bookAndJang)
        {
            var bookKor = ExtractBookKor(bookAndJang);
            var jang = ExtractJang(bookAndJang);
            var redirectUrl = $"{bookKor.BibleBookKorToEng()}.{jang}.{GetJeolRange()}.NKJV";

            BibleWebUrl = $"{_bibleWebUrlEndPoint}{redirectUrl}";
            BibleAppUrl = $"{_bibleAppUrlEndPoint}{redirectUrl}";
        }

        private static int ExtractJang(string guonAndJang)
        {
            var _jang = 1;
            try
            {
                _jang = int.Parse(Regex.Replace(guonAndJang, @"\D", ""));
            }
            catch (Exception e)
            {
                ExceptionCommonText("ExtractJang()", e.Message);
            }
            return _jang;
        }

        private static string ExtractBookKor(string guonAndJang)
        {
            var _bookKor = "창";
            try
            {
                _bookKor = Regex.Replace(guonAndJang, @"\d", "");
            }
            catch (Exception e)
            {
                ExceptionCommonText("ExtractBookKor()", e.Message);
            }
            return _bookKor;
        }

        private static string GetJeolRange()
        {
            var tmpVerseNumRange = "1-10";
            try
            {
                tmpVerseNumRange = Regex.Replace(JsonMannaData.Verse.Substring(JsonMannaData.Verse.IndexOf(":") + 1), "~", "-");
            }
            catch (Exception e)
            {
                ExceptionCommonText("GetJeolRange()", e.Message);
            }

            return tmpVerseNumRange;
        }

        private static string ExtractBookAndJang()
        {
            var tmpBibleAt = "창1";
            try
            {
                tmpBibleAt = JsonMannaData.Verse.Substring(0, JsonMannaData.Verse.IndexOf(":"));
            }
            catch (Exception e)
            {
                ExceptionCommonText("ExtractBookAndJang()", e.Message);
            }

            return tmpBibleAt;
        }

        private static string ExceptionCommonText(string location, string message)
        {
            return $"{EXCEPTION_COMMON_TEXT} {location}!\n{message}";
        }
    }
}
