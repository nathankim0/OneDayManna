using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace OneDayManna.Views
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();

            colorPicker.SelectedColor = Color.FromHex(Preferences.Get("CustomTextColor", Constants.DEFAULT_TEXT_COLOR));
            fontSizeSlider.Value = Preferences.Get("TextSize", 17);

            sampleLabel.TextColor = colorPicker.SelectedColor;
            sampleLabel.FontSize = fontSizeSlider.Value;
        }

        void ColorCircle_SelectedColorChanged(object sender, ColorPicker.BaseClasses.ColorPickerEventArgs.ColorChangedEventArgs e)
        {
            var hex = e.NewColor.ToHex();
            Preferences.Set("CustomTextColor", hex);

            sampleLabel.TextColor = e.NewColor;
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
