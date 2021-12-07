using System;
using Xamarin.Forms;

namespace OneDayManna
{
    public static class AppManager
    {
        public static async void Init()
        {
            var isGetMannaCompleted = await MannaDataManager.GetManna(DateTime.Now);
            if (!isGetMannaCompleted)
            {
                await Application.Current.MainPage.DisplayAlert("만나 불러오기 실패", "새로고침 해주세요", "확인");
            }
        }
    }
}
