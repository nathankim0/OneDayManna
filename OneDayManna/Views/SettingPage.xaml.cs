using System;
using System.Collections.Generic;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna.Views
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();

            outerScrollView.Margin = new Thickness(0, Constants.StatusBarHeight, 0, 0);
            fontSizeSlider.Value = double.TryParse(Preferences.Get("TextSize", "17"), out var font) ? font : 17;

            sampleLabel.TextColor = Color.FromHex(Preferences.Get("CustomTextColor", Constants.DEFAULT_TEXT_COLOR));
            backgroundDimBoxView.BackgroundColor = Color.FromHex(Preferences.Get("CustomBackgroundDimColor", Constants.DEFAULT_BACKGROUND_DIM_COLOR));
            sampleLabel.FontSize = fontSizeSlider.Value;
        }

        async void OnTextColorButtonClicked(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup();
            colorPickerPopup.SetColor(Color.FromHex(Preferences.Get("CustomTextColor", Constants.DEFAULT_TEXT_COLOR)));
            colorPickerPopup.ColorChanged += (s, color) =>
            {
                var hex = color.ToHex();
                Preferences.Set("CustomTextColor", hex);

                sampleLabel.TextColor = color;
            };
            await Navigation.PushPopupAsync(colorPickerPopup);
        }

        async void OnBackgroundDimColorButtonClicked(object sender, EventArgs e)
        {
            var colorPickerPopup = new ColorPickerPopup();
            colorPickerPopup.SetColor(Color.FromHex(Preferences.Get("CustomBackgroundDimColor", Constants.DEFAULT_BACKGROUND_DIM_COLOR)));
            colorPickerPopup.ColorChanged += (s, color) =>
            {
                var hex = color.ToHex();
                Preferences.Set("CustomBackgroundDimColor", hex);

                backgroundDimBoxView.BackgroundColor = color;
            };
            await Navigation.PushPopupAsync(colorPickerPopup);
        }

        void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            Preferences.Set("TextSize", e.NewValue);
            sampleLabel.FontSize = e.NewValue;
        }

        async void OnBackButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
