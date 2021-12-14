using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using OneDayManna.Popups;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna.Views
{
    public partial class MainPage : ContentPage
    {
        readonly MainPageViewModel viewModel;
        private List<MannaContent> selectedContentList = new List<MannaContent>();

        public MainPage()
        {
            InitializeComponent();

            innerStackLayout.Margin = new Thickness(0, Constants.StatusBarHeight + 20, 0, 0);
            optionsStackLayout.Margin = new Thickness(0, Constants.StatusBarHeight + 20, 30, 0);

            viewModel = new MainPageViewModel();
            BindingContext = viewModel;

            SelectFeaturePopup.Instance.CopyClicked += async (s, e) =>
            {
                var selectedVersesText = GetSelectedMannaShareVersesText();
                var selectedContentsText = GetSelectedMannaShareContentsText();

                await ResetSelection();
                
                await Clipboard.SetTextAsync(selectedContentsText);

                string title = "";
                string ok = "";
                if (AppManager.GetCurrentLanguage() == Language.English.ToString())
                {
                    title = "Copied to clipboard";
                    ok = "Ok";
                }
                else
                {
                    title = "클립보드에 복사됨";
                    ok = "확인";
                }

                await DisplayAlert(title, selectedVersesText, ok);
            };

            SelectFeaturePopup.Instance.DownloadClicked += (s, e) =>
            {

            };

            SelectFeaturePopup.Instance.ShareClicked += async (s, e) =>
            {
                var selectedContentsText = GetSelectedMannaShareContentsText();
                await ResetSelection();

                string title = "";
                if (AppManager.GetCurrentLanguage() == Language.English.ToString())
                {
                    title = "Share";
                }
                else
                {
                    title = "공유";
                }

                await Share.RequestAsync(new ShareTextRequest
                {
                    Text = DateTime.Today.ToString("yy-MM-dd") + "\n\n" + selectedContentsText,
                    Title = title
                });
            };

            viewModel.IsRefreshing = true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.CustomBackgroundDimColor = AppManager.GetCurrentBackgroundDimColor();
            mainPageViewModel.CustomTextColor = AppManager.GetCurrentTextColor();
            mainPageViewModel.CustomFontSize = AppManager.GetCurrentTextSize();
        }
        private string GetSelectedMannaShareVersesText()
        {
            try
            {
                var selectedVersesText = string.Join(", ", selectedContentList.Select(x => $"{x.BookAndJang}:{x.Jeol}").ToArray());
                Debug.WriteLine(selectedVersesText);
                return selectedVersesText;
            }
            catch(Exception ex)
            {
                AppManager.PrintException("GetSelectedMannaShare Verses Text()", ex.Message);
                return string.Empty;
            }
        }

        private string GetSelectedMannaShareContentsText()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return string.Empty;

            try
            {
                var selectedContentsText = string.Join("\n", selectedContentList.Select(x => $"{x.BookAndJang}:{x.Jeol} {x.MannaString}").ToArray());

                if (viewModel?.IsAllSelected ?? false)
                {
                    selectedContentsText = $"{viewModel.Range}\n{selectedContentsText}";
                }

                Debug.WriteLine(selectedContentsText);
                return selectedContentsText;
            }
            catch (Exception ex)
            {
                AppManager.PrintException("GetSelectedMannaShare Contents Text()", ex.Message);
                return string.Empty;
            }
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            optionsStackLayout.IsVisible = false;
            await ResetSelection();

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
                await Application.Current.MainPage.DisplayAlert("불러오기 실패", "다시 시도 해주세요", "확인");
            }
            else
            {
                if (AppManager.GetCurrentLanguage() == Language.English.ToString())
                {
                    SetEnglishMannaContents();
                }
                else
                {
                    SetKoreanMannaContents();
                }
            }
            optionsStackLayout.IsVisible = true;
        }

        private void SetEnglishMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.EnglishMannaContents;
            mainPageViewModel.Range = MannaDataManager.JsonOtherLanguageManna.Reference;
        }

        private void SetKoreanMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.KoreanMannaContents;
            mainPageViewModel.Range = MannaDataManager.JsonMannaData.Verse;
        }
        
        async void OnSettingClicked(object sender, EventArgs e)
        {
            await ResetSelection();
            var settingPage = new SettingPage();
            settingPage.LanguageChanged += (object sender, Language language) =>
            {
                if(language == Language.English)
                {
                    SetEnglishMannaContents();
                }
                else
                {
                    SetKoreanMannaContents();
                }
            };
            await Navigation.PushAsync(settingPage);
        }

        async void OnSelectAllButtonClicked(object sender, EventArgs e)
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            if (mainPageViewModel.IsAllSelected) //다 선택되어 있을 때
            {
                mainPageViewModel.IsAllSelected = false;
                await ResetSelection();
            }
            else
            {
                mainPageViewModel.IsAllSelected = true;

                mainPageViewModel.MannaContents?.All(manna => manna.Selected = true);
                selectedContentList = mainPageViewModel.MannaContents.ToList();

                if (AppManager.IsPopupNavigationNullOrExist()) return;
                await Navigation.PushPopupAsync(SelectFeaturePopup.Instance);
            }

        }

        async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var mannaContent = ((TappedEventArgs)e).Parameter as MannaContent;
            var selected = !mannaContent.Selected;
            mannaContent.Selected = selected;

            try
            {
                if (selected)
                {
                    selectedContentList?.Add(mannaContent);
                }
                else
                {
                    selectedContentList?.Remove(mannaContent);
                }
            }
            catch (Exception ex)
            {
                AppManager.PrintException("selectedContentList add/remove", ex.Message);
            }

            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;
            if (mainPageViewModel.MannaContents?.All(x => x.Selected) ?? false)
            {
                mainPageViewModel.IsAllSelected = true;
            }
            else
            {
                mainPageViewModel.IsAllSelected = false;
            }

            try
            {
                if (selected && (selectedContentList?.Count ?? 0) == 1)
                {
                    if (AppManager.IsPopupNavigationNullOrExist()) return;
                    await Navigation.PushPopupAsync(SelectFeaturePopup.Instance);
                }
                else if (!selected && (selectedContentList?.Count ?? 0) == 0)
                {
                    await Navigation.RemovePopupPageAsync(SelectFeaturePopup.Instance);
                }
            }
            catch (Exception ex)
            {
                AppManager.PrintException("popup push/remove", ex.Message);
            }
        }

        async Task ResetSelection()
        {
            try
            {
                selectedContentList = new List<MannaContent>();

                var mannaContents = viewModel.MannaContents ?? (viewModel.MannaContents = new Xamarin.CommunityToolkit.ObjectModel.ObservableRangeCollection<MannaContent>());
                mannaContents.ToList().ForEach(manna => manna.Selected = false);

                if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;
                mainPageViewModel.IsAllSelected = false;

                try
                {
                    await Navigation.PopAllPopupAsync();
                }
                catch { }

            }
            catch (Exception ex)
            {
                AppManager.PrintException("ResetSelection", ex.Message);
            }
        }
    }
}
