using System;
using UIKit;
using Xamarin.Forms;
using Hands.iOS;
using Hands.Models;

[assembly: ExportRenderer(typeof(Page), typeof(ModalPageRenderer))]
namespace Hands.iOS
{
    public class ModalPageRenderer : Xamarin.Forms.Platform.iOS.PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (Element is ICancelableModalPage modalPage)
            {
                NavigationController.TopViewController.NavigationItem.LeftBarButtonItem =
                    new UIBarButtonItem(title: "Cancel",
                        style: UIBarButtonItemStyle.Plain,
                        handler: (sender, args) => { modalPage.OnCancel(); });
            }
        }
    }
}
