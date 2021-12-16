using System;
using System.Collections.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna.Views
{
    public class Country : BaseViewModel
    {
        public Language LanguageValue { get; set; }
        public string Language { get; set;}
        public string Flag { get; set; }

        private bool selected;
        public bool Selected { get=>selected; set=>SetProperty(ref selected, value); }
    }

    public class OnboardingPageViewModel : BaseViewModel
    {
        public ObservableCollection<Country> Countries { get; set; }

        private bool isSelected;
        public bool IsSelected { get => isSelected; set => SetProperty(ref isSelected, value); }

        public OnboardingPageViewModel()
        {
            Countries = new ObservableCollection<Country>
            {
                new Country
                {
                    LanguageValue = Language.Korean,
                    Language = "한국어",
                    Flag = "🇰🇷"
                },
                new Country
                {
                    LanguageValue = Language.English,
                    Language = "English",
                    Flag = "🇺🇸"
                },
                new Country
                {
                    LanguageValue = Language.Spanish,
                    Language = "Español",
                    Flag = "🇪🇸"
                },
                new Country
                {
                    LanguageValue = Language.Chinese,
                    Language = "漢語",
                    Flag = "🇨🇳"
                },
                new Country
                {
                    LanguageValue = Language.Japanese,
                    Language = "日本語",
                    Flag = "🇯🇵"
                },
                new Country
                {
                    LanguageValue = Language.German,
                    Language = "Deutsch",
                    Flag = "🇩🇪"
                },
                new Country
                {
                    LanguageValue = Language.French,
                    Language = "français",
                    Flag = "🇫🇷"
                },
                new Country
                {
                    LanguageValue = Language.Hindi,
                    Language = "हिन्दी",
                    Flag = "🇮🇳"
                }
            };
        }
    }
    public partial class OnboardingPage : ContentPage
    {
        public EventHandler<Language> LanguageChanged;
        Language selectedLanguage = Language.Korean;

        public OnboardingPage()
        {
            InitializeComponent();
            BindingContext = new OnboardingPageViewModel();
        }

        public void SetSelectedLanguage()
        {
            var currentLanguage = AppManager.GetCurrentLanguageEnumValue();
            if (!(BindingContext is OnboardingPageViewModel viewModel)) return;
            if (viewModel.Countries == null) return;
            foreach (var node in viewModel.Countries)
            {
                if (node.LanguageValue.Equals(currentLanguage))
                {
                    node.Selected = true;
                }
            }
        }

        void TapGestureRecognizer_Tapped(System.Object sender, System.EventArgs e)
        {
            if (!(BindingContext is OnboardingPageViewModel viewModel)) return;
            if (viewModel.Countries == null) return;

            DependencyService.Get<IVibration>().VibrateLight();

            viewModel.IsSelected = true;
            button.IsEnabled = true;

            foreach (var node in viewModel.Countries)
            {
                node.Selected = false;
            }

            var country = ((TappedEventArgs)e).Parameter as Country;
            country.Selected = true;

            selectedLanguage = country.LanguageValue;
        }

        void Button_Clicked(object sender, EventArgs e)
        {
            DependencyService.Get<IVibration>().VibrateHeavy();

            Preferences.Set("CurrentLanguage", selectedLanguage.ToString());

            LanguageChanged?.Invoke(this, selectedLanguage);
            Navigation.PopModalAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            return false;
        }
    }
}
