using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Rg.Plugins.Popup.Extensions;
using Xamarin.Forms;

namespace OneDayManna
{
    public partial class ColorPickerPopup : Rg.Plugins.Popup.Pages.PopupPage
    {
        private static ColorPickerPopup _instance;
        public static ColorPickerPopup Instance => _instance ?? (_instance = new ColorPickerPopup());

        private Color selectedColor;
        public EventHandler<Color> ColorChanged;

        public ColorPickerPopup()
        {
            InitializeComponent();
            colorPicker.Padding = new Thickness(15, 0, 15, Constants.StatusBarHeight + 15);
        }

        void ColorCircle_SelectedColorChanged(object sender, ColorPicker.BaseClasses.ColorPickerEventArgs.ColorChangedEventArgs e)
        {
            ColorChanged?.Invoke(this, selectedColor);
            selectedColor = e.NewColor;
        }

        //async void Button_Clicked(System.Object sender, System.EventArgs e)
        //{
        //    ColorChanged?.Invoke(this, selectedColor);
        //    await Navigation.RemovePopupPageAsync(this);
        //}
        async void OnXButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopPopupAsync();
        }
        public void SetColor(Color color)
        {
            colorPicker.SelectedColor = color;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }

        // ### Methods for supporting animations in your popup page ###

        // Invoked before an animation appearing
        protected override void OnAppearingAnimationBegin()
        {
            base.OnAppearingAnimationBegin();
        }

        // Invoked after an animation appearing
        protected override void OnAppearingAnimationEnd()
        {
            base.OnAppearingAnimationEnd();
        }

        // Invoked before an animation disappearing
        protected override void OnDisappearingAnimationBegin()
        {
            base.OnDisappearingAnimationBegin();
        }

        // Invoked after an animation disappearing
        protected override void OnDisappearingAnimationEnd()
        {
            base.OnDisappearingAnimationEnd();
        }

        protected override Task OnAppearingAnimationBeginAsync()
        {
            return base.OnAppearingAnimationBeginAsync();
        }

        protected override Task OnAppearingAnimationEndAsync()
        {
            return base.OnAppearingAnimationEndAsync();
        }

        protected override Task OnDisappearingAnimationBeginAsync()
        {
            return base.OnDisappearingAnimationBeginAsync();
        }

        protected override Task OnDisappearingAnimationEndAsync()
        {
            return base.OnDisappearingAnimationEndAsync();
        }

        // ### Overrided methods which can prevent closing a popup page ###

        // Invoked when a hardware back button is pressed
        protected override bool OnBackButtonPressed()
        {
            // Return true if you don't want to close this popup page when a back button is pressed
            return base.OnBackButtonPressed();
        }

        // Invoked when background is clicked
        protected override bool OnBackgroundClicked()
        {
            // Return false if you don't want to close this popup page when a background of the popup page is clicked
            return base.OnBackgroundClicked();
        }
    }
}
