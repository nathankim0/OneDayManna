using OneDayManna.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentPage), typeof(SwipeNavigationRenderer))]
namespace OneDayManna.iOS
{
    public class SwipeNavigationRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewDidAppear(animated);
            if (ViewController.NavigationController is null) return;
            ViewController.NavigationController.InteractivePopGestureRecognizer.Enabled = true;
            ViewController.NavigationController.InteractivePopGestureRecognizer.Delegate = new UIGestureRecognizerDelegate();
        }
    }
}