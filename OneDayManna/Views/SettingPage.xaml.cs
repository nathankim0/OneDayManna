using System;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna.Views
{
    public partial class SettingPage : ContentPage
    {
        public EventHandler<Language> LanguageChanged;

        public SettingPage()
        {
            InitializeComponent();

            outerStackLayout.Margin = new Thickness(0, Constants.StatusBarHeight, 0, 0);

            fontSizeSlider.Value = AppManager.GetCurrentTextSize();
            sampleLabel.TextColor = AppManager.GetCurrentTextColor();
            backgroundDimBoxView.BackgroundColor = AppManager.GetCurrentBackgroundDimColor();
            sampleLabel.FontSize = fontSizeSlider.Value;
            languagePicker.SelectedIndex = GetLanguageSelectedIndex();

            SetTextByLanguage(AppManager.GetCurrentLanguage());
        }

        private void SetTextByLanguage(string currentLanguage)
        {
            if (currentLanguage == Language.Korean.ToString())
            {
                fontTitleLabel.Text = "폰트 크기";
                languageTitleLabel.Text = "Language";
            }
            else if(currentLanguage == Language.English.ToString())
            {
                fontTitleLabel.Text = "Font Size";
                languageTitleLabel.Text = "언어";
            }
        }

        private async void OnTextColorButtonClicked(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup();
            colorPickerPopup.SetColor(AppManager.GetCurrentTextColor());
            colorPickerPopup.ColorChanged += (s, color) =>
            {
                var hex = color.ToHex();
                Preferences.Set("CustomTextColor", hex);

                sampleLabel.TextColor = color;
            };
            await Navigation.PushPopupAsync(colorPickerPopup);
        }

        private async void OnBackgroundDimColorButtonClicked(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup();
            colorPickerPopup.SetColor(AppManager.GetCurrentBackgroundDimColor());
            colorPickerPopup.ColorChanged += (s, color) =>
            {
                var hex = color.ToHex();
                Preferences.Set("CustomBackgroundDimColor", hex);

                backgroundDimBoxView.BackgroundColor = color;
            };
            await Navigation.PushPopupAsync(colorPickerPopup);
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Preferences.Set("TextSize", e.NewValue);
            sampleLabel.FontSize = e.NewValue;
        }

        private async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private int GetLanguageSelectedIndex()
        {
            return AppManager.GetCurrentLanguage() switch
            {
                "Korean" => 0,
                "English" => 1,
                _ => 0,
            };
            ;
        }

        private void OnLanguagePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            var selectedLanguage = picker.SelectedIndex switch
            {
                0 => Language.Korean,
                1 => Language.English,
                _ => Language.Korean,
            };
            Preferences.Set("CurrentLanguage", selectedLanguage.ToString());

            SetTextByLanguage(AppManager.GetCurrentLanguage());

            LanguageChanged?.Invoke(this, selectedLanguage);
        }

        void dimTitleButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            button.Opacity = 0.5;
        }

        void dimTitleButton_Released(System.Object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            button.Opacity = 1;
        }

        void textColorTitleButton_Pressed(System.Object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            button.Opacity = 0.5;
        }

        void textColorTitleButton_Released(System.Object sender, System.EventArgs e)
        {
            var button = (ImageButton)sender;
            button.Opacity = 1;
        }
    }
}
