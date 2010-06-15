using System;
using System.Drawing;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;

namespace MonoTouch.ImageGallery
{	
	public class ScrollViewDelegate : UIScrollViewDelegate
	{
		UIView zoomView;
		
		public ScrollViewDelegate(UIView zoomView)
		{
			this.zoomView = zoomView;	
		}
		
		public override UIView ViewForZoomingInScrollView (UIScrollView scrollView)
		{
			return this.zoomView;
		}
	}	
}

