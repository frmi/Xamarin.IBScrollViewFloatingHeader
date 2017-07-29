using System;
using CoreGraphics;
using Foundation;
using IBFloatingHeader;
using UIKit;

namespace Sample
{
    public partial class ViewController : UIViewController
    {
        protected ViewController(IntPtr handle) : base(handle)
        {
            // Note: this .ctor should not contain any initialization logic.
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			// Perform any additional setup after loading the view, typically from a nib.

			var header = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, 40))
			{
				BackgroundColor = UIColor.Red
			};

			var tableView = new IBTableViewFloatingHeader()
			{
				Frame = View.Bounds,
				DataSource = new DataSource()
			};

			tableView.FloatingHeaderView = header;

			View.AddSubview(tableView);
		}

		public override void DidReceiveMemoryWarning()
		{
			base.DidReceiveMemoryWarning();
			// Release any cached data, images, etc that aren't in use.
		}

		class DataSource : UITableViewDataSource
		{
			string[] array = new string[] { @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", @"Red", @"Green", @"Blue", @"Yellow", null };

			public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
			{
				var cell = tableView.DequeueReusableCell("Cell");
				if (cell == null)
				{
					cell = new UITableViewCell(UITableViewCellStyle.Default, "Cell");
				}

				cell.TextLabel.Text = array[indexPath.Row];

				return cell;
			}

			public override nint RowsInSection(UITableView tableView, nint section)
			{
				return array.Length;
			}
		}
    }
}
