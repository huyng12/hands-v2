//using System;
//using Hands.iOS;
//using UIKit;
//using Xamarin.Forms;
//using Xamarin.Forms.Platform.iOS;

//[assembly: ExportRenderer(typeof(Hands.AppShell), typeof(AppShellTabbarRenderer))]
//namespace Hands.iOS
//{
//    public class AppShellTabbarRenderer : ShellRenderer
//    {
//        protected override IShellSectionRenderer CreateShellSectionRenderer(
//            ShellSection shellSection)
//            => base.CreateShellSectionRenderer(shellSection);

//        protected override IShellTabBarAppearanceTracker CreateTabBarAppearanceTracker()
//            => new CustomTabbarAppearance();
//    }

//    public class CustomTabbarAppearance : IShellTabBarAppearanceTracker
//    {
//        public void Dispose() { }

//        public void ResetAppearance(UITabBarController controller) { }

//        public void UpdateLayout(UITabBarController controller) { }

//        public void SetAppearance(UITabBarController controller, ShellAppearance appearance)
//        {
//            if (controller.TabBar == null) return;
//            controller.TabBar.BackgroundColor = UIColor.White;
//            controller.TabBar.BarTintColor = UIColor.Black;
//            controller.TabBar.UnselectedItemTintColor =
//                Color.FromHex("#A8A8B2").ToUIColor();

//            if (controller.TabBar.Items == null) return;
//            foreach (UITabBarItem item in controller.TabBar.Items)
//                //item.tex
//                item.ImageInsets = new UIEdgeInsets(6, 0, -6, 0);
//        }
//    }
//}
