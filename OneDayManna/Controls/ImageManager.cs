using System;
using System.IO;
using System.Threading.Tasks;
using OneDayManna.Interfaces;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Xamarin.Forms;

namespace OneDayManna.Controls
{
    public static class ImageManager
    {
        public static Stream DownloadedImageSource;
        static SKBitmap imageBitmap;

        public static async Task<bool> GetImage()
        {
            try
            {
                DownloadedImageSource = await RestService.Instance.GetRandomImageStream();

                if (DownloadedImageSource != null && DownloadedImageSource != Stream.Null)
                {
                    imageBitmap = await ConvertStreamToSKBitmap(DownloadedImageSource);
                    return true;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }

        public async static Task<SKBitmap> ConvertStreamToSKBitmap(Stream stream)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                await stream.CopyToAsync(memStream);
                memStream.Seek(0, SeekOrigin.Begin);

                return SKBitmap.Decode(memStream);
            };
        }

        public static async Task<bool>SaveImage()
        {
            //await draw(text);
            return await Save(imageBitmap);
        }

        public static async Task<bool> Save(SKBitmap bitmap)
        {
            using (SKImage image = SKImage.FromBitmap(bitmap))
            {
                SKData data = image.Encode();
                DateTime dt = DateTime.Now;
                string filename = string.Format("DayManna-{0:D4}{1:D2}{2:D2}-{3:D2}{4:D2}{5:D2}{6:D3}.png",
                                                dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, dt.Millisecond);

                IPhotoLibrary photoLibrary = DependencyService.Get<IPhotoLibrary>();
                bool result = await photoLibrary.SavePhotoAsync(data.ToArray(), "DayManna", filename);

                return result;
            }
        }

        private static Task draw(string text)
        {
            // Create bitmap and draw on it
            using (SKPaint textPaint = new SKPaint
            {
                TextSize = 100,
                Color = AppManager.GetCurrentTextColor().ToSKColor()
            })
            {
                SKRect bounds = new SKRect();
                textPaint.MeasureText(text, ref bounds);

                using (SKCanvas bitmapCanvas = new SKCanvas(imageBitmap))
                {
                    bitmapCanvas.DrawText(text, 0, -bounds.Top, textPaint);
                }
            }
            // Create SKCanvasView to view result
            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += (sender, args) =>
            {
                SKImageInfo info = args.Info;
                SKSurface surface = args.Surface;
                SKCanvas canvas = surface.Canvas;

                canvas.Clear(SKColors.Aqua);
                canvas.DrawBitmap(imageBitmap, imageBitmap.Width/2, imageBitmap.Height/2);
            };
            return Task.CompletedTask;
        }
    }
}
