using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Hands.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(TextCell), typeof(StandardTextCellRenderer))]
namespace Hands.iOS
{
    public class StandardTextCellRenderer : TextCellRenderer
    {
        public override UIKit.UITableViewCell GetCell(Cell item, UIKit.UITableViewCell reusableCell, UIKit.UITableView tv)
        {
            // Use UITableViewCellStyle.Value1 to acchieve TextDetail right-aligned
            // https://stackoverflow.com/a/43782481
            //var textCell = (TextCell)item;

            //var fullName = item.GetType().FullName;
            //var cell = tv.DequeueReusableCell(fullName) as CellTableViewCell;
            //if (cell == null) cell = new CellTableViewCell(UITableViewCellStyle.Value1, fullName);
            //else cell.Cell.PropertyChanged -= cell.HandlePropertyChanged;

            //cell.Cell = textCell;
            //textCell.PropertyChanged += (sender, args) => tv.ReloadData();
            //cell.PropertyChanged = this.HandlePropertyChanged;

            //cell.TextLabel.Text = textCell.Text;
            //cell.DetailTextLabel.Text = textCell.Detail;

            //UpdateBackground(cell, item);

            // Support UITableViewCellAccessory
            var cell = base.GetCell(item, reusableCell, tv);
            switch (item.StyleId)
            {
                case "button-like":
                    cell.TextLabel.TextColor = UIColor.SystemBlue;
                    break;
                case "disclosure":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DisclosureIndicator;
                    break;
                case "checkmark":
                    cell.Accessory = UIKit.UITableViewCellAccessory.Checkmark;
                    break;
                case "detail-button":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DetailButton;
                    break;
                case "detail-disclosure-button":
                    cell.Accessory = UIKit.UITableViewCellAccessory.DetailDisclosureButton;
                    break;
                case "none":
                default:
                    cell.Accessory = UIKit.UITableViewCellAccessory.None;
                    break;
            }

            // Fix incorrect background color of ViewCells in TableView
            // https://github.com/xamarin/Xamarin.Forms/issues/8431#issuecomment-764699237
            cell.ContentView.BackgroundColor = null;
            cell.ContentView.Opaque = false;
            cell.BackgroundView = new UIKit.UIView
            {
                BackgroundColor = UIKit.UIColor.White
            };

            return cell;
        }
    }
}
