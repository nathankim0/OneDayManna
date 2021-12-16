using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarcTron.Plugin;
using OneDayManna.Controls;
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

            optionsStackLayout.Margin = new Thickness(0, Constants.StatusBarHeight + 20, 30, 0);

            viewModel = new MainPageViewModel();
            BindingContext = viewModel;

            SelectFeaturePopup.Instance.CopyClicked += OnCopyCliked;
            SelectFeaturePopup.Instance.ShareClicked += OnShareCliked;

            if (Constants.IsDeviceIOS)
            {
                CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-8018732227937375/8481038983");
            }
            else
            {
                CrossMTAdmob.Current.LoadInterstitial("ca-app-pub-8018732227937375/5527572586");
            }

            viewModel.IsRefreshing = true;


            if (VersionTracking.IsFirstLaunchEver)
            {
                var onboardingPage = new OnboardingPage();
                onboardingPage.LanguageChanged += GetMannaByLanguage;
                Navigation.PushModalAsync(onboardingPage, false);
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.CustomBackgroundDimColor = AppManager.GetCurrentBackgroundDimColor();
            mainPageViewModel.CustomTextColor = AppManager.GetCurrentTextColor();
            mainPageViewModel.CustomFontSize = AppManager.GetCurrentTextSize();
        }

        private void optionsStackLayout_SizeChanged(object sender, EventArgs e)
        {
            innerStackLayout.Margin = new Thickness(0, innerStackLayout.Y + 70, 0, 0);
        }

        private async void RefreshView_Refreshing(object sender, EventArgs e)
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.IsLoadingServer = true;

            optionsStackLayout.IsVisible = false;
            await ResetSelection();

            var getImageTask = ImageManager.GetImage();
            var getMannaTask = MannaDataManager.GetManna(DateTime.Now);

            await Task.WhenAll(getImageTask, getMannaTask);

            var result = getImageTask.Result;

            if (result)
            {
                mainPageViewModel.CurrentImageSource = ImageSource.FromStream(() => ImageManager.DownloadedImageSource);
            }
            else
            {
                mainPageViewModel.CurrentImageSource = ImageSource.FromFile("image1.jpg");
            }

            viewModel.IsRefreshing = false;

            if (!getMannaTask.Result)
            {
                await Application.Current.MainPage.DisplayAlert("불러오기 실패", "다시 시도 해주세요", "확인");
            }
            else
            {
                SetContentsByLanguage(AppManager.GetCurrentLanguageString());
            }
            optionsStackLayout.IsVisible = true;

            mainPageViewModel.IsLoadingServer = false;
        }

        private bool isDownloading;
        private async void OnDownloadButtonClicked(object sender, EventArgs e)
        {
            if (isDownloading) return;

            DependencyService.Get<IVibration>().VibrateLight();

            isDownloading = true;

            LoadingPopup.Instance.SetText("Saving...");
            await Navigation.PushPopupAsync(LoadingPopup.Instance);

            var result = await ImageManager.SaveImage();

            await Navigation.RemovePopupPageAsync(LoadingPopup.Instance);

            var isKorean = AppManager.GetCurrentLanguageString() == Language.Korean.ToString();

            if (!result)
            {
                await Application.Current.MainPage.DisplayAlert(isKorean ? "이미지 저장 실패" : "Failed to save image", isKorean ? "이미지 저장에 실패했습니다. 다시 시도해보세요!" : "Image could not be saved. Try again!", isKorean ? "확인" : "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(isKorean ? "이미지 저장 완료" : "Image saved", isKorean ? "앨범을 확인해보세요!" : "Check out the photo library", isKorean ? "확인" : "OK");
            }

            isDownloading = false;

            CrossMTAdmob.Current.ShowInterstitial();
        }

        private bool isCapturing;
        private async void OnCaptureButtonClicked(object sender, EventArgs e)
        {
            if (isCapturing) return;

            DependencyService.Get<IVibration>().VibrateLight();

            isCapturing = true;

            optionsStackLayout.IsVisible = false;
            admobBanner.IsVisible = false;

            var screenshot = await Screenshot.CaptureAsync();

            optionsStackLayout.IsVisible = true;
            admobBanner.IsVisible = true;

            LoadingPopup.Instance.SetText("Saving...");
            await Navigation.PushPopupAsync(LoadingPopup.Instance);

            var stream = await screenshot.OpenReadAsync();
            var bitmap = await ImageManager.ConvertStreamToSKBitmap(stream);
            var result = await ImageManager.Save(bitmap);

            await Navigation.RemovePopupPageAsync(LoadingPopup.Instance);

            var isKorean = AppManager.GetCurrentLanguageString() == Language.Korean.ToString();

            if (!result)
            {
                await Application.Current.MainPage.DisplayAlert(isKorean ? "스크린샷 저장 실패" : "Failed to save screenshot", isKorean ? "이미지 저장에 실패했습니다. 다시 시도해보세요!" : "Image could not be saved. Try again!", isKorean ? "확인" : "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert(isKorean ? "스크린샷 저장 완료" : "Screenshot saved", isKorean ? "앨범을 확인해보세요!" : "Check out the photo library", isKorean ? "확인" : "OK");
            }

            isCapturing = false;

            CrossMTAdmob.Current.ShowInterstitial();
        }

        private async void OnSelectAllButtonClicked(object sender, EventArgs e)
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            DependencyService.Get<IVibration>().VibrateLight();

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

        private async void OnSettingClicked(object sender, EventArgs e)
        {
            await ResetSelection();
            var settingPage = new SettingPage();
            settingPage.LanguageChanged += GetMannaByLanguage;
            await Navigation.PushAsync(settingPage);
        }

        private async void GetMannaByLanguage(object sender, Language language)
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.IsLoadingServer = true;

            await MannaDataManager.GetManna(language);
            SetContentsByLanguage(language.ToString());

            mainPageViewModel.IsLoadingServer = false;
        }

        private void SetContentsByLanguage(string language)
        {
            if (language == Language.Korean.ToString())
            {
                SetKoreanMannaContents();
            }
            else if (language == Language.Spanish.ToString())
            {
                SetSpanishMannaContents();
            }
            else if (language == Language.Chinese.ToString())
            {
                SetChineseMannaContents();
            }
            else if (language == Language.Japanese.ToString())
            {
                SetJapaneseMannaContents();
            }
            else if (language == Language.German.ToString())
            {
                SetGermanMannaContents();
            }
            else if (language == Language.French.ToString())
            {
                SetFrenchMannaContents();
            }
            else if (language == Language.Hindi.ToString())
            {
                SetHindiMannaContents();
            }
            else
            {
                SetEnglishMannaContents();
            }
        }

        private async void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            var selected = false;

            try
            {
                var mannaContent = ((TappedEventArgs)e).Parameter as MannaContent;
                selected = !mannaContent.Selected;
                mannaContent.Selected = selected;

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

        private async void OnCopyCliked(object sender, EventArgs e)
        {
            var selectedVersesText = GetSelectedMannaShareVersesText();
            var selectedContentsText = GetSelectedMannaShareContentsText();

            await ResetSelection();

            await Clipboard.SetTextAsync(selectedContentsText);

            string title = "";
            string ok = "";
            if (AppManager.GetCurrentLanguageString() == Language.Korean.ToString())
            {
                title = "클립보드에 복사됨";
                ok = "확인";
            }
            else
            {
                title = "Copied to clipboard";
                ok = "Ok";
            }

            await DisplayAlert(title, selectedVersesText, ok);
        }

        private async void OnShareCliked(object sender, EventArgs e)
        {
            var selectedContentsText = GetSelectedMannaShareContentsText();
            await ResetSelection();

            string title = "";
            if (AppManager.GetCurrentLanguageString() == Language.Korean.ToString())
            {
                title = "공유";
            }
            else
            {
                title = "Share";
            }

            await Share.RequestAsync(new ShareTextRequest
            {
                Text = DateTime.Today.ToString("yy-MM-dd") + "\n\n" + selectedContentsText,
                Title = title
            });
        }

        private string GetSelectedMannaShareVersesText()
        {
            try
            {
                var selectedVersesText = string.Join(", ", selectedContentList.Select(x => $"{x.BookAndJang}:{x.Jeol}").ToArray());
                Debug.WriteLine(selectedVersesText);
                return selectedVersesText;
            }
            catch (Exception ex)
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
                    var contentsTextWithOnlyJeol = string.Join("\n", selectedContentList.Select(x => $"{x.Jeol} {x.MannaString}").ToArray());
                    selectedContentsText = $"{viewModel.Range}\n{contentsTextWithOnlyJeol}";
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

        private void SetEnglishMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.EnglishMannaContents;
            mainPageViewModel.Range = MannaDataManager.EnglishMannaData.Reference;
        }

        private void SetKoreanMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.KoreanMannaContents;
            mainPageViewModel.Range = MannaDataManager.KoreanMannaData.Verse;
        }

        private void SetSpanishMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.SpanishMannaContents;
            mainPageViewModel.Range = MannaDataManager.SpanishRange;
        }

        private void SetChineseMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.ChineseMannaContents;
            mainPageViewModel.Range = MannaDataManager.ChineseRange;
        }

        private void SetJapaneseMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.JapaneseMannaContents;
            mainPageViewModel.Range = MannaDataManager.JapaneseRange;
        }

        private void SetGermanMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.GermanMannaContents;
            mainPageViewModel.Range = MannaDataManager.GermanRange;
        }

        private void SetFrenchMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.FrenchMannaContents;
            mainPageViewModel.Range = MannaDataManager.FrenchRange;
        }

        private void SetHindiMannaContents()
        {
            if (!(BindingContext is MainPageViewModel mainPageViewModel)) return;

            mainPageViewModel.MannaContents = MannaDataManager.HindiMannaContents;
            mainPageViewModel.Range = MannaDataManager.HindiRange;
        }

        private async Task ResetSelection()
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
