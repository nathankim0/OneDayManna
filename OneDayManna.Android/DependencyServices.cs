using Android.App;
using Xamarin.Forms;
using Android.Content;
using Android.Views.InputMethods;
using Android.OS;
using Android.Support.V4.App;
using System.Collections.Generic;
using System;
using OneDayManna.Droid;

[assembly: Dependency(typeof(StatusBar))]

namespace OneDayManna.Droid
{
    public class StatusBar : IStatusBar
    {
        public static Activity Activity { get; set; }

        public int GetHeight()
        {

            float statusBarHeight = -1;
            int resourceId = Activity.Resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (resourceId > 0)
            {
                statusBarHeight = Activity.Resources.GetDimension(resourceId);
            }

            float density = Activity.Resources.DisplayMetrics.Density;

            return (int)(statusBarHeight / density);
        }
    }
}
