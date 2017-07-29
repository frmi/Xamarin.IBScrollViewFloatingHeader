using System;
using CoreGraphics;
using Foundation;
using UIKit;

namespace IBFloatingHeader
{
    public class IBScrollViewFloatingHeader : UIScrollView
    {
		private static NSString ContentOffsentKey = new NSString("contentOffset");
		private static IntPtr IBFloatingHeaderViewKey = (IntPtr)1;
		private static IntPtr IBFloatingHeaderViewContext = (IntPtr)2;

		private nfloat oldYOffset = 0;

		private UIView _floatingHeaderView;
		public UIView FloatingHeaderView
		{
			get
			{
				return _floatingHeaderView;
			}
			set
			{
				HandlePreviousHeaderView();

				_floatingHeaderView = value;

				HandleNewHeaderView();
			}
		}

		void HandlePreviousHeaderView()
		{
			UIView previousHeaderView = _floatingHeaderView;
			if (previousHeaderView != null)
			{
				UIEdgeInsets contentInset = ContentInset;
				UIEdgeInsets scrollInset = ScrollIndicatorInsets;
				contentInset.Top -= previousHeaderView.Frame.Size.Height;
				scrollInset.Top -= previousHeaderView.Frame.Size.Height;
				ContentInset = contentInset;
				ScrollIndicatorInsets = scrollInset;
				previousHeaderView.RemoveFromSuperview();
				try
				{
					RemoveObserver(this, ContentOffsentKey);
				}
				catch (Exception)
				{
				}

				previousHeaderView = null;
			}
		}

		void HandleNewHeaderView()
		{
			if (_floatingHeaderView != null)
			{
				AddSubview(_floatingHeaderView);
				AddObserver(this, ContentOffsentKey, NSKeyValueObservingOptions.New, IBFloatingHeaderViewContext);
				_floatingHeaderView.Frame = new CGRect(0, -_floatingHeaderView.Frame.Size.Height, this.Frame.Size.Width, _floatingHeaderView.Frame.Size.Height);
				UIEdgeInsets contentInset = ContentInset;
				UIEdgeInsets scrollInset = ScrollIndicatorInsets;
				contentInset.Top += _floatingHeaderView.Frame.Size.Height;
				scrollInset.Top += _floatingHeaderView.Frame.Size.Height;
				ContentInset = contentInset;
				ScrollIndicatorInsets = scrollInset;
				ContentOffset = new CGPoint(0, -_floatingHeaderView.Frame.Size.Height);
			}
		}

		#region Scroll Logic
		/*
         * Three scrollStates:
         * - Fast Swipe up + header not visible = start showing the header
         * - Header is showing at least part of itself: scroll it by the content offset difference (with a maximum of the top offset)
         * - Header is completely showing = header should not scroll any more
         */
		void ScrolledFromOffset(nfloat oldYOffset, nfloat newYOffset)
		{
			CGPoint scrollVelocity = PanGestureRecognizer.VelocityInView(this);
			bool fastSwipe = false;
			if (Math.Abs(scrollVelocity.Y) > 1000)
			{
				fastSwipe = true;
			}
			bool isHeaderShowing = _floatingHeaderView.Frame.Y + _floatingHeaderView.Frame.Size.Height >= newYOffset;
			bool isHeaderCompletelyShowing = _floatingHeaderView.Frame.Y >= oldYOffset;

			if (fastSwipe && !isHeaderShowing && oldYOffset > newYOffset)
			{
				nfloat difference = oldYOffset - newYOffset;
				CGRect floatingFrame = _floatingHeaderView.Frame;
				floatingFrame.Y = newYOffset - floatingFrame.Size.Height + difference;
				if (floatingFrame.Y > newYOffset)
				{
					floatingFrame.Y = newYOffset;
				}
				_floatingHeaderView.Frame = floatingFrame;
			}

			if (isHeaderShowing)
			{
				nfloat difference = oldYOffset - newYOffset;
				CGRect floatingFrame = _floatingHeaderView.Frame;
				//floatingFrame.origin.y = floatingFrame.origin.y  + difference;
				if (floatingFrame.Y > newYOffset)
				{
					floatingFrame.Y = newYOffset;
				}
				_floatingHeaderView.Frame = floatingFrame;
			}

			if (isHeaderCompletelyShowing && newYOffset < -_floatingHeaderView.Frame.Size.Height)
			{
				CGRect floatingFrame = _floatingHeaderView.Frame;
				floatingFrame.Y = newYOffset;
				_floatingHeaderView.Frame = floatingFrame;

			}
		}
		#endregion

		#region KVO

		public override void ObserveValue(NSString keyPath, NSObject ofObject, NSDictionary change, IntPtr context)
		{
			if (context == IBFloatingHeaderViewContext)
			{
				if (keyPath.Equals(ContentOffsentKey))
				{
					// "old" is somehow never added to the change dictionary
					//nfloat oldYOffset = ((NSValue)change["old"]).CGPointValue.Y;
					nfloat newYOffset = ((NSValue)change["new"]).CGPointValue.Y;

					ScrolledFromOffset(oldYOffset, newYOffset);

					// Work around use private field for keeping track of the old value
					oldYOffset = newYOffset;
				}
			}
			else
			{
				base.ObserveValue(keyPath, ofObject, change, context);
			}
		}

		void RemoveContentOffsetObserver()
		{
			RemoveObserver(this, ContentOffsentKey, IBFloatingHeaderViewContext);
		}

		#endregion
	}
}
