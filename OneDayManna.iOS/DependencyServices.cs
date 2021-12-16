using System;
using OneDayManna.iOS;
using UIKit;
using Xamarin.Forms;
using Foundation;
using OneDayManna.Interfaces;
using System.Threading.Tasks;

[assembly: Dependency(typeof(StatusBar))]
[assembly: Dependency(typeof(PhotoLibrary))]
[assembly: Dependency(typeof(Vibration))]
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

    public class Vibration : IVibration, IDisposable
    {
        public enum HapticFeedbackType
        {
            ImpactHeavy, // Heavy impact
            ImpactMedium, // Medium impact
            ImpactLight, // Light impact
            Selection, // To tick while scrolling through a scrollview or carousel
            NotificationError, // When an in-app error notification occurs
            NotificationWarning, // When an in-app warning notification occurs
            NotificationSuccess // When an in-app success notification occurs
        }

        private UISelectionFeedbackGenerator _selection;

        private UIImpactFeedbackGenerator _impactHeavyFeedbackGenerator;
        private UIImpactFeedbackGenerator _impactMediumFeedbackGenerator;
        private UIImpactFeedbackGenerator _impactLightFeedbackGenerator;
        private UISelectionFeedbackGenerator _selectionFeedbackGenerator;
        private UINotificationFeedbackGenerator _notificationFeedbackGenerator;

        public Vibration()
        {
            _impactHeavyFeedbackGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Heavy);
            _impactMediumFeedbackGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Medium);
            _impactLightFeedbackGenerator = new UIImpactFeedbackGenerator(UIImpactFeedbackStyle.Light);
            _selectionFeedbackGenerator = new UISelectionFeedbackGenerator();
            _notificationFeedbackGenerator = new UINotificationFeedbackGenerator();
        }

        public void VibrateLight()
        {
            if (_selection == null)
            {
                _selection = new UISelectionFeedbackGenerator();
                _selection.Prepare();
            }
            _selection.SelectionChanged();
        }

        public void VibrateHeavy()
        {
            PrepareHapticFeedback(HapticFeedbackType.NotificationSuccess);
            ExecuteHapticFeedback(HapticFeedbackType.NotificationSuccess);
        }

        public void PrepareHapticFeedback(HapticFeedbackType type)
        {
            switch (type)
            {
                case HapticFeedbackType.ImpactHeavy:
                    _impactHeavyFeedbackGenerator?.Prepare();
                    break;
                case HapticFeedbackType.ImpactMedium:
                    _impactMediumFeedbackGenerator?.Prepare();
                    break;
                case HapticFeedbackType.ImpactLight:
                    _impactLightFeedbackGenerator?.Prepare();
                    break;
                case HapticFeedbackType.Selection:
                    _selectionFeedbackGenerator?.Prepare();
                    break;
                case HapticFeedbackType.NotificationError:
                case HapticFeedbackType.NotificationWarning:
                case HapticFeedbackType.NotificationSuccess:
                    _notificationFeedbackGenerator?.Prepare();
                    break;
            }
        }

        public void ExecuteHapticFeedback(HapticFeedbackType type)
        {
            switch (type)
            {
                case HapticFeedbackType.ImpactHeavy:
                    _impactHeavyFeedbackGenerator?.ImpactOccurred();
                    break;
                case HapticFeedbackType.ImpactMedium:
                    _impactMediumFeedbackGenerator?.ImpactOccurred();
                    break;
                case HapticFeedbackType.ImpactLight:
                    _impactLightFeedbackGenerator?.ImpactOccurred();
                    break;
                case HapticFeedbackType.Selection:
                    _selectionFeedbackGenerator?.SelectionChanged();
                    break;
                case HapticFeedbackType.NotificationError:
                    _notificationFeedbackGenerator?.NotificationOccurred(UINotificationFeedbackType.Error);
                    break;
                case HapticFeedbackType.NotificationWarning:
                    _notificationFeedbackGenerator?.NotificationOccurred(UINotificationFeedbackType.Warning);
                    break;
                case HapticFeedbackType.NotificationSuccess:
                    _notificationFeedbackGenerator?.NotificationOccurred(UINotificationFeedbackType.Success);
                    break;
            }
        }

        #region IDisposable
        public void Dispose()
        {
            _impactHeavyFeedbackGenerator = null;
            _impactMediumFeedbackGenerator = null;
            _impactLightFeedbackGenerator = null;
            _selectionFeedbackGenerator = null;
            _notificationFeedbackGenerator = null;
        }
        #endregion
    }
}
