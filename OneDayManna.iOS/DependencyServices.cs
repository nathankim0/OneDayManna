using System;
using OneDayManna.iOS;
using UIKit;
using Xamarin.Forms;
using System.Collections.Generic;
using Foundation;
using UserNotifications;
using System.IO;

[assembly: Dependency(typeof(StatusBar))]
namespace OneDayManna.iOS
{
    public class StatusBar : IStatusBar
    {
        public int GetHeight()
        {
            return (int)UIApplication.SharedApplication.StatusBarFrame.Height;
        }
    }
}
