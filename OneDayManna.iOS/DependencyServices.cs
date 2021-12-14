using System;
using OneDayManna.iOS;
using UIKit;
using Xamarin.Forms;
using System.Collections.Generic;
using Foundation;
using UserNotifications;
using System.IO;
using OneDayManna.Interfaces;
using System.Threading.Tasks;

[assembly: Dependency(typeof(StatusBar))]
[assembly: Dependency(typeof(PhotoLibrary))]
namespace OneDayManna.iOS
{
    public class StatusBar : IStatusBar
    {
        public int GetHeight()
        {
            return (int)UIApplication.SharedApplication.StatusBarFrame.Height;
        }
    }

    public class PhotoLibrary : IPhotoLibrary
    {
        public Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            NSData nsData = NSData.FromArray(data);
            UIImage image = new UIImage(nsData);
            TaskCompletionSource<bool> taskCompletionSource = new TaskCompletionSource<bool>();

            image.SaveToPhotosAlbum((UIImage img, NSError error) =>
            {
                taskCompletionSource.SetResult(error == null);
            });

            return taskCompletionSource.Task;
        }
    }
}
