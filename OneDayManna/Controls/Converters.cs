using System;
using System.Globalization;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace OneDayManna
{
    public class ProfileIconConverter : IValueConverter
    {
        private readonly string IMAGE_RESOURCE_PATH = "OneDayManna.Resources.Images.";
        public string Source { get; set; }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Source = (string)value;

            if (string.IsNullOrWhiteSpace(Source))
            {
                Source = "image1";
            }

            var imageSource = ImageSource.FromResource(IMAGE_RESOURCE_PATH + Source + ".jpg", typeof(ImageResourceExtension).GetTypeInfo().Assembly);
            return imageSource;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        public string Source { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null)
            {
                return null;
            }

            // Do your translation lookup here, using whatever method you require
            var imageSource = ImageSource.FromResource(Source, typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            return imageSource;
        }
    }
}
