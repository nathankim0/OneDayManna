using System;
using System.Diagnostics;
using System.Linq;
using Rg.Plugins.Popup.Services;
using Xamarin.Forms;

namespace OneDayManna
{
    public static class AppManager
    {
        private static readonly string EXCEPTION_COMMON_TEXT = "Exception occured at";

        public static async void InitManna()
        {
            var isGetMannaCompleted = await MannaDataManager.GetManna(DateTime.Now);
            if (!isGetMannaCompleted)
            {
                await Application.Current.MainPage.DisplayAlert("만나 불러오기 실패", "새로고침 해주세요", "확인");
            }
        }

        public static void PrintException(string location, string message)
        {
            Debug.WriteLine($"{EXCEPTION_COMMON_TEXT} {location}!\n{message}");
        }

        public static void PrintStartText(string location)
        {
            Debug.WriteLine($"** {location} start! **");
        }

        public static void PrintCompleteText(string location)
        {
            Debug.WriteLine($"** {location} complete! **");
        }

        public static bool IsPopupNavigationNullOrExist()
        {
            return PopupNavigation.Instance.PopupStack?.Any() ?? true;
        }
    }
}
