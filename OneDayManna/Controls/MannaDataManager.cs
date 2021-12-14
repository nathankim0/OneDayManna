using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.ObjectModel;
using System.Collections.Generic;

namespace OneDayManna
{
    public static class MannaDataManager
    {
        public static ObservableRangeCollection<MannaContent> KoreanMannaContents = new ObservableRangeCollection<MannaContent>();
        public static ObservableRangeCollection<MannaContent> EnglishMannaContents = new ObservableRangeCollection<MannaContent>();
        public static JsonMannaModel JsonMannaData = new JsonMannaModel();
        public static JsonMannaOtherLanguageModel JsonOtherLanguageManna = new JsonMannaOtherLanguageModel();

        public static string Today { get; set; } = DateTime.Now.ToString("yyyy년 MM월 dd일 (ddd)");
        public static string DisplayDateRange { get; set; } = DateTime.Now.ToString("MM/dd");
        public static string AllMannaTexts { get; set; } = "";
        public static string MannaShareRange { get; set; } = "";

        public static string BibleWebUrl = "";
        public static string BibleAppUrl = "";

        public static string BookKor;
        public static int Jang;
        public static string JeolRange;

        public static string EnglishMannaRange;

        public static async Task<bool> GetManna(DateTime dateTime)
        {
            AppManager.PrintStartText("GetManna()");

            Today = dateTime.ToString("yyyy년 MM월 dd일 (ddd)");
            DisplayDateRange = dateTime.ToString("MM/dd");

            var endPoint = Constants.MANNA_ENDPOINT + dateTime.ToString("yyyy-MM-dd");

            try
            {
                JsonMannaData = await RestService.Instance.GetMannaDataAsync(endPoint);

                MannaShareRange = $"만나: {JsonMannaData.Verse}";

                var bookAndJang = ExtractBookAndJang();

                SetBibleWebAndAppUrl(bookAndJang);
                SetMannaCollection(JsonMannaData, bookAndJang);

                JsonOtherLanguageManna = await RestService.Instance.GetMultilanguageManna(BookKor, Jang, JeolRange);
                SetMannaCollection(JsonOtherLanguageManna);

                AppManager.PrintCompleteText("GetManna()");

                return true;
            }
            catch (Exception e)
            {
                AppManager.PrintException("GetManna()", e.Message);
                return false;
            }
        }

        private static void SetMannaCollection(JsonMannaModel JsonMannaData, string bookAndJang)
        {
            var mannaContents = new List<MannaContent>();
            var allMannaTexts = string.Empty;

            foreach (var node in JsonMannaData.Contents)
            {
                var jeol = 0;
                var onlyString = "";

                try { onlyString = Regex.Replace(node, @"\d", "").Substring(1); }
                catch (Exception e) { AppManager.PrintException("SetMannaCollection onlystring", e.Message); }

                try { jeol = int.Parse(Regex.Replace(node, @"\D", "")); }
                catch (Exception e) { AppManager.PrintException("SetMannaCollection onlyNum", e.Message); }

                mannaContents.Add(new MannaContent
                {
                    BookAndJang = bookAndJang,
                    Jeol = jeol,
                    MannaString = onlyString,
                });

                allMannaTexts += node + "\n\n";
            }

            KoreanMannaContents = new ObservableRangeCollection<MannaContent>(mannaContents);
            AllMannaTexts = allMannaTexts;
        }

        private static void SetMannaCollection(JsonMannaOtherLanguageModel JsonMannaData)
        {
            var mannaContents = new List<MannaContent>();

            foreach (var node in JsonMannaData.Verses)
            {
                mannaContents.Add(new MannaContent
                {
                    BookAndJang = $"{node.BookName}{node.Chapter}",
                    Jeol = node.Verse,
                    MannaString = node.Text,
                });
            }

            EnglishMannaContents = new ObservableRangeCollection<MannaContent>(mannaContents);
        }

        private static void SetBibleWebAndAppUrl(string bookAndJang)
        {
            BookKor = ExtractBookKor(bookAndJang);
            Jang = ExtractJang(bookAndJang);
            JeolRange = GetJeolRange();

            var redirectUrl = $"{BookKor.BibleBookKorToEng()}.{Jang}.{JeolRange}.NKJV";

            BibleWebUrl = $"{Constants.BIBLE_WEB_ENDPOINT}{redirectUrl}";
            BibleAppUrl = $"{Constants.BIBLE_APP_ENDPOINT}{redirectUrl}";
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
                AppManager.PrintException("ExtractJang()", e.Message);
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
                AppManager.PrintException("ExtractBookKor()", e.Message);
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
                AppManager.PrintException("GetJeolRange()", e.Message);
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
                AppManager.PrintException("ExtractBookAndJang()", e.Message);
            }

            return tmpBibleAt;
        }
    }
}
