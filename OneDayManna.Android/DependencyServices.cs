using Android.App;
using Xamarin.Forms;
using OneDayManna.Droid;
using System.Threading.Tasks;
using OneDayManna.Interfaces;
using Java.IO;
using Environment = Android.OS.Environment;

[assembly: Dependency(typeof(StatusBar))]
[assembly: Dependency(typeof(PhotoLibrary))]

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

    public class PhotoLibrary : IPhotoLibrary
    {
    public async Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            try
            {
                File picturesDirectory = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryPictures);
                File folderDirectory = picturesDirectory;

                if (!string.IsNullOrEmpty(folder))
                {
                    folderDirectory = new File(picturesDirectory, folder);
                    folderDirectory.Mkdirs();
                }

                using (File bitmapFile = new File(folderDirectory, filename))
                {
                    bitmapFile.CreateNewFile();

                    using (FileOutputStream outputStream = new FileOutputStream(bitmapFile))
                    {
                        await outputStream.WriteAsync(data);
                    }

                    //// Make sure it shows up in the Photos gallery promptly.
                    //MediaScannerConnection.ScanFile(MainActivity.Instance,
                    //                                new string[] { bitmapFile.Path },
                    //                                new string[] { "image/png", "image/jpeg" }, null);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
