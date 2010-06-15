using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.UrlImageStore;

namespace MonoTouch.ImageGallery
{
	public class IGSingleImageViewController<TKey> : UIViewController, IUrlImageUpdated<TKey>
	{
		static Selector SwipeLeftSelector = new Selector("HandleSwipeLeft");
		static Selector SwipeRightSelector = new Selector("HandleSwipeRight");
		static Selector TapSelector = new Selector("HandleTap");
		
		UIScrollView scrollView;
		UIImageView imageView;
		UISwipeGestureRecognizer swipeLeft;
		UISwipeGestureRecognizer swipeRight;
		UITapGestureRecognizer tapGesture;
		
		UITextView descriptionView;
		UIActivityIndicatorView activityView;
		
		bool showDetails = true;
		
		public IGSingleImageViewController (List<IGImage<TKey>> images, int imageIndex, UrlImageStore<TKey> imageStore) : base()
		{
			this.Images = images;
			this.ImageIndex = imageIndex;
			this.ImageStore = imageStore;
			
			this.scrollView = new UIScrollView(this.View.Bounds);
			
			this.scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			this.scrollView.BackgroundColor = UIColor.Black;
			this.scrollView.ScrollEnabled = true;
			this.scrollView.MinimumZoomScale = 0.5f;
			this.scrollView.MaximumZoomScale = 3.0f;
			
			var img = this.Images[this.ImageIndex];
			
			this.Title = img.Title;
			
			bool showActivity = !this.ImageStore.Exists(img.Id);
			
			UIImage imgUI = this.ImageStore.RequestImage(img.Id, img.Url, this);
			this.imageView = new UIImageView(imgUI);
			this.imageView.Frame = scrollView.Bounds;
			this.imageView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			
			this.scrollView.Delegate = new ScrollViewDelegate(this.imageView);
			this.scrollView.AddSubview(this.imageView);
			this.scrollView.ContentSize = this.imageView.Image.Size;
			
			this.View.AddSubview(this.scrollView);
			
			float descHeight = this.View.StringSize(this.Images[imageIndex].Caption, UIFont.SystemFontOfSize(13.0f), new SizeF(this.View.Bounds.Width - 10, this.View.Bounds.Height - 15), UILineBreakMode.WordWrap).Height;
			
			this.descriptionView = new UITextView(new RectangleF(0, this.View.Frame.Height - descHeight - 15 - 44, this.View.Frame.Width, descHeight + 15));
			this.descriptionView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
			this.descriptionView.Text = this.Images[imageIndex].Caption;
			this.descriptionView.BackgroundColor = UIColor.Black.ColorWithAlpha(0.5f);
			this.descriptionView.Font = UIFont.SystemFontOfSize(13.0f);
			this.descriptionView.TextColor = UIColor.White;
			this.descriptionView.Editable = false;
			this.View.AddSubview(this.descriptionView);
			
			this.activityView = new UIActivityIndicatorView(new RectangleF((this.View.Bounds.Width - 30) / 2, (this.View.Bounds.Height - 30) / 2, 30, 30));
			this.activityView.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleBottomMargin;
			this.activityView.Hidden = !showActivity;
			this.activityView.ActivityIndicatorViewStyle = UIActivityIndicatorViewStyle.WhiteLarge;
			this.View.AddSubview(this.activityView);
			
			
			if (showActivity)
				this.activityView.StartAnimating();
				
			//this.activityView.StartAnimating();
			
			
			//this.Title = this.Images[imageIndex].Title;
			
			tapGesture = new UITapGestureRecognizer();
			tapGesture.NumberOfTapsRequired = 1;
			tapGesture.DelaysTouchesEnded = true;
			tapGesture.DelaysTouchesBegain = true;
			
			tapGesture.AddTarget(this, TapSelector);
			this.View.AddGestureRecognizer(tapGesture);
			
			swipeLeft = new UISwipeGestureRecognizer();
			swipeLeft.Direction = UISwipeGestureRecognizerDirection.Left;
			swipeLeft.Delegate = new SwipeRecognizerDelegate();
			swipeLeft.AddTarget(this, SwipeLeftSelector);
			this.View.AddGestureRecognizer(swipeLeft);
			
			swipeRight = new UISwipeGestureRecognizer();
			swipeRight.Direction = UISwipeGestureRecognizerDirection.Right;
			swipeRight.Delegate = new SwipeRecognizerDelegate();
			swipeRight.AddTarget(this, SwipeRightSelector);
			this.View.AddGestureRecognizer(swipeRight);
			
			
			
			this.scrollView.DelaysContentTouches = true;
			
		}
		
//		RectangleF touchingRect = RectangleF.Empty;
//		
//		public override void TouchesBegan (NSSet touches, UIEvent evt)
//		{
//			base.TouchesBegan (touches, evt);
//			
//			var touchLocation =	((UITouch)evt.TouchesForView(this.View).AnyObject).LocationInView(this.View);
//			touchingRect = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);
//			
//			Console.WriteLine("TouchesBegan");
//		}
//		public override void TouchesCancelled (NSSet touches, UIEvent evt)
//		{
//			base.TouchesCancelled (touches, evt);
//			touchingRect = RectangleF.Empty;
//		}
//		public override void TouchesEnded (NSSet touches, UIEvent evt)
//		{
//			base.TouchesEnded (touches, evt);
//			
//			Console.WriteLine("TouchesEnded");
//			
//			if (touchingRect != RectangleF.Empty)
//			{
//				var touchLocation =	((UITouch)evt.TouchesForView(this.View).AnyObject).LocationInView(this.View);
//				var touchedRect = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);	
//				
//				var allowance = new RectangleF(touchingRect.X - 10, touchingRect.Y - 10, 20, 20);	
//				
//				if (allowance.IntersectsWith(touchedRect))
//				{
//					Console.WriteLine("Tapped");
//					
//					this.descriptionView.Hidden = !this.descriptionView.Hidden;
//					this.NavigationController.SetNavigationBarHidden(!this.NavigationController.NavigationBarHidden, true);
//				}
//				
//				touchingRect = RectangleF.Empty;
//			}			
//		}
	
		UIColor originalBarColour = UIColor.Black;
		float originalBarAlpha = 1.0f;
		
		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			this.NavigationController.NavigationBar.TintColor = originalBarColour;
			this.NavigationController.NavigationBar.Alpha = originalBarAlpha;
			
		}
		
		public override void ViewDidAppear (bool animated)
		{
			if (this.SingleRightBarButton != null)
				this.NavigationItem.RightBarButtonItem = this.SingleRightBarButton;
			
			this.View.SetNeedsLayout();
			this.View.LayoutSubviews();
			
			
			LayoutDescription();
			
			this.NavigationController.SetNavigationBarHidden(true, false);
			this.NavigationController.SetNavigationBarHidden(false, false);
		//	this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
		//	this.NavigationController.NavigationBar.TintColor = UIColor.Black;
//			this.NavigationController.NavigationBar.Opaque = false;
			
			this.originalBarAlpha = this.NavigationController.NavigationBar.Alpha;
			this.originalBarColour	 = this.NavigationController.NavigationBar.TintColor;
			
			this.NavigationController.NavigationBar.TintColor = UIColor.Black;
			this.NavigationController.NavigationBar.Translucent = true;
			this.NavigationController.NavigationBar.Alpha = 0.6f;
//			
//			this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
//			this.NavigationController.NavigationBar.Translucent = true;
			
		}
		
		public void ChangeImage(bool next)
		{
			this.BeginInvokeOnMainThread(delegate { changeImage(next); });	
		}
		
		public void changeImage(bool next)
		{
			int newIndex = next ? this.ImageIndex + 1 : this.ImageIndex - 1;
			
			if (newIndex > (this.Images.Count - 1) || newIndex < 0)
				return;
			
			//Fade image out
			UIView.BeginAnimations(null);
			UIView.SetAnimationDuration(0.3f);
		//	UIView.SetAnimationCurve(UIViewAnimationCurve.EaseOut);
			this.imageView.Alpha = 0.0f;
			UIView.CommitAnimations();
			
			this.ImageIndex = newIndex;
			
			var img = this.Images[this.ImageIndex];
			bool showActivity = !this.ImageStore.Exists(img.Id);
			UIImage imgUI = this.ImageStore.RequestImage(img.Id, img.Url, this);
			
			UIView.BeginAnimations(null);
			UIView.SetAnimationDuration(0.3f);
			this.imageView.Alpha = 1.0f;
			
			this.imageView.Image = imgUI;
			UIView.CommitAnimations();
			
			this.scrollView.ContentSize = imgUI.Size;
			
			this.scrollView.ZoomScale = 1.0f;
			
			this.activityView.Hidden = !showActivity;
			
			this.Title = img.Title;
			
			if (showActivity)
				this.activityView.StartAnimating();
			LayoutDescription();
			
		}
		
		void LayoutDescription()
		{
			var img = this.Images[this.ImageIndex];
			float descHeight = this.View.StringSize(img.Caption, UIFont.SystemFontOfSize(13.0f), new SizeF(this.View.Bounds.Width - 10, this.View.Bounds.Height - 10), UILineBreakMode.WordWrap).Height;
			
			this.descriptionView.Frame = new RectangleF(0, this.View.Bounds.Height - descHeight - 15, this.View.Bounds.Width, descHeight + 15);
			this.descriptionView.Text = img.Caption;
		}
	
		public UIBarButtonItem SingleRightBarButton
		{
			get;set;
		}
		
		public UrlImageStore<TKey> ImageStore
		{
			get;set;	
		}
		
		public List<IGImage<TKey>> Images
		{
			get;set;	
		}
		
		public int ImageIndex
		{
			get;set;	
		}
		
		[ExportAttribute("HandleSwipeLeft")]
		public void HandleSwipeLeft(UISwipeGestureRecognizer recognizer)
		{
			Console.WriteLine("Swipe Left");
			ChangeImage(true);
		}
		
		[ExportAttribute("HandleSwipeRight")]
		public void HandleSwipeRight(UISwipeGestureRecognizer recognizer)
		{
			Console.WriteLine("Swipe Right");
			ChangeImage(false);
		}
		
		[Export("HandleTap")]
		public void HandleTap(UITapGestureRecognizer recognizer)
		{
			
			if (this.descriptionView.Hidden)
			{
				this.descriptionView.Hidden = false;
				
				UIView.BeginAnimations("showDesc");
				UIView.SetAnimationDuration(0.5);
				
				this.NavigationController.SetNavigationBarHidden(false, false);
			
				LayoutDescription();
				
				UIView.CommitAnimations();
			}
			else
			{
				this.BeginInvokeOnMainThread(delegate {
					this.NavigationController.SetNavigationBarHidden(true, true);
				
				});
				UIView.BeginAnimations("hideDesc");
				UIView.SetAnimationDuration(1.5);
				UIView.SetAnimationTransition(UIViewAnimationTransition.CurlDown, this.descriptionView, true);
				//UIView.SetAnimationTransition(UIViewAnimationTransition.CurlUp, this.NavigationController.NavigationBar, true);
				
				
				this.descriptionView.Frame = new RectangleF(0, this.descriptionView.Frame.Y + this.descriptionView.Frame.Height + 10, this.descriptionView.Frame.Width, this.descriptionView.Frame.Height);
				
				UIView.CommitAnimations();
				
				this.descriptionView.Hidden = true;
				
			
		
			}
		}
		
		public void UrlImageUpdated (TKey id)
		{
			if (id.Equals(this.Images[this.ImageIndex].Id))
			{
				this.BeginInvokeOnMainThread(delegate {
					this.imageView.Image = this.ImageStore.GetImage(this.Images[this.ImageIndex].Id);
					this.scrollView.ContentSize = this.imageView.Image.Size;
					
					this.activityView.StopAnimating();
					this.activityView.Hidden = true;
				});
			}
		}
		
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			return true;
		}
	}
	
	class SwipeRecognizerDelegate : UIGestureRecognizerDelegate
	{
		public override bool ShouldBegin (UIGestureRecognizer recognizer)
		{
			return true;
		}
		public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
		{
			return true;
		}
	}
}

