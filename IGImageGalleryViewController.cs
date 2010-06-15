using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.UrlImageStore;

namespace MonoTouch.ImageGallery
{
	public class IGImageGalleryViewController<TKey> : UIViewController
	{
		UIScrollView scrollView;
		//IGView<TKey> scrollView;
		IGView<TKey> igView;
		
		public IGImageGalleryViewController (UrlImageStore<TKey> imageStore, UrlImageStore<TKey> thumbnailImageStore) : base()
		{
			this.images = new List<IGImage<TKey>>();
			this.ImageStore = imageStore;		
			this.ThumbnailImageStore = thumbnailImageStore;
			this.Images = new List<IGImage<TKey>>();
			//this.NavigationController.NavigationBar.BarStyle = UIBarStyle.Black;
			//this.NavigationController.NavigationBar.Translucent = true;
			
			scrollView = new UIScrollView(this.View.Bounds);
			scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			scrollView.ScrollEnabled = true;
			//scrollView.DelaysContentTouches = true;
			
			this.igView = new IGView<TKey>(scrollView.Bounds, this.Images, this.ImageStore, this.ThumbnailImageStore);
			this.igView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
			this.igView.ImageTapped += delegate(int imageIndex) {
				Console.WriteLine("Image Tap[ped: " + imageIndex);
				singleViewController = new IGSingleImageViewController<TKey>(this.Images, imageIndex, this.ImageStore);
				singleViewController.SingleRightBarButton = this.SingleRightBarButton;
				this.BeginInvokeOnMainThread(delegate { 
					
					this.NavigationController.PushViewController(singleViewController, true);
				});
			};
			
			scrollView.AddSubview(this.igView);
			
			this.View.AddSubview(scrollView);
		}
			
		IGSingleImageViewController<TKey> singleViewController;
//		public string Title
//		{
//			get { return this.NavigationItem.Title; }
//			set { this.NavigationItem.Title = value; }
//		}
		
		public UIBarButtonItem GalleryRightBarButton
		{
			get; // { return this.NavigationItem == null ? null : this.NavigationItem.RightBarButtonItem; }
			set; // { if (this.NavigationItem != null) this.NavigationItem.RightBarButtonItem = value; }
		}
		
		public UIBarButtonItem SingleRightBarButton
		{
			get; // { return (this.singleViewController != null && this.singleViewController.NavigationItem != null) ? this.singleViewController.NavigationItem.RightBarButtonItem : null; }
			set; // { if (this.singleViewController != null && this.singleViewController.NavigationItem != null) this.singleViewController.NavigationItem.RightBarButtonItem = value; }
		}
		
		public UrlImageStore<TKey> ImageStore
		{
			get;set;	
		}
		
		public UrlImageStore<TKey> ThumbnailImageStore
		{
			get;set;	
		}
		
		List<IGImage<TKey>> images;
		
		public List<IGImage<TKey>> Images
		{
			get { return images; } 
			set { images = value; } //Update(); }	
		}
			
		
		public void Update()
		{
			//if (!appeared)
			//	return;
			
			int across = 4;
			if (scrollView.Frame.Width > 320f)
				across = 6;
			
			int rows = (int)Math.Ceiling((double)this.Images.Count / (double)across);
			
			float height = (rows * 75f) + ((rows + 1) * 4f);
						
			this.igView.Frame = new RectangleF(0, 0, scrollView.Frame.Width, height);
			
			this.scrollView.ContentSize = igView.Frame.Size;
			//this.scrollView.SetNeedsDisplay();
			
			igView.SetNeedsDisplay(); 	
		}
		
		bool appeared = false;
		
		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
			appeared = true;
			
			if (this.GalleryRightBarButton != null)
				this.NavigationItem.RightBarButtonItem = this.GalleryRightBarButton;
		}
		public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
		{
			Update();
			return true;
		}
	}
}

