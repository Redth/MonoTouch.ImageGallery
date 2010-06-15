using System;
using System.Collections.Generic;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.UrlImageStore;

namespace MonoTouch.ImageGallery
{
	public class IGView<TKey> : UIView, IUrlImageUpdated<TKey>
	{
		public delegate void ImageTappedDelegate(int imageIndex);
		public event ImageTappedDelegate ImageTapped;
		
		public const float ImageWidth = 75f;
		public const float ImageHeight = 75f;
		public const float Padding = 4f;	
		
		public IGView (List<IGImage<TKey>> images, UrlImageStore<TKey> imageStore, UrlImageStore<TKey> thumbnailImageStore) : base()
		{
			this.Images = images;
			this.ImageStore = imageStore;
			this.ThumbnailImageStore = thumbnailImageStore;
		}
		
		public IGView(RectangleF frame, List<IGImage<TKey>> images, UrlImageStore<TKey> imageStore, UrlImageStore<TKey> thumbnailImageStore) : base(frame)
		{
			this.Images = images;
			this.ImageStore = imageStore;
			this.ThumbnailImageStore = thumbnailImageStore;
		}
	
	
		public List<IGImage<TKey>> Images
		{
			get;set;	
		}
		
		public UrlImageStore<TKey> ImageStore
		{
			get;set;	
		}
		
		public UrlImageStore<TKey> ThumbnailImageStore
		{
			get;set;	
		}
		
		public void UrlImageUpdated (TKey id)
		{
			SetNeedsDisplay();
		}
		
		public override void Draw (RectangleF rect)
		{
			DateTime start = DateTime.Now;
			base.Draw (rect);
			
			var context = UIGraphics.GetCurrentContext();
			
			UIColor.White.SetFill();
			context.FillRect(rect);
			
			int across = 4;
			
			if (rect.Width > 320f)
				across = 6;
			
			int row = 0;
			int col = 0;
			//int rows = (int)Math.Ceiling((double)this.Images.Count / (double)across);
			
			
			foreach (var img in Images)
			{
				float x = col * ImageWidth + col * Padding + Padding;
				float y = row * ImageHeight + row * Padding + Padding;
				//draw image
				var imgRect =new RectangleF(x, y, ImageWidth, ImageHeight);
				
				this.ThumbnailImageStore.RequestImage(img.Id, img.Url, this).Draw(imgRect);
				
				if (touchingImage != null && img.Id.Equals( touchingImage.Id))
				{
					UIColor.LightGray.SetStroke();
					context.StrokeRectWithWidth(imgRect, 3.0f);
				}
				
				col++;
				
				if (col >= across)
				{
					row++;
					col = 0;
				}
			}
			
			TimeSpan len = DateTime.Now - start;
			
			Console.WriteLine("Draw Time: " + len.TotalMilliseconds);
		}
		
		IGImage<TKey> touchingImage = null;
		RectangleF touchingImageRect = RectangleF.Empty;
		int touchingImageIndex = -1;
		
		public override void TouchesBegan (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesBegan (touches, evt);
			
			var touchLocation =	((UITouch)evt.TouchesForView(this).AnyObject).LocationInView(this);
			var touchFrame = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);
			
			int across = this.Bounds.Size.Width > 320 ? 6 : 4;
			int row = 0;
			int col = 0;
			
			int imgOn = 0;
			foreach (var img in Images)
			{
				float x = col * ImageWidth + col * Padding + Padding;
				float y = row * ImageHeight + row * Padding + Padding;
				var imgRect = new RectangleF(x, y, ImageWidth, ImageHeight);
				
				if (touchFrame.IntersectsWith(imgRect))
				{
					touchingImage = img;
					touchingImageRect = imgRect;
					touchingImageIndex = imgOn;
					SetNeedsDisplay();
					break;
				}
				
				col++;
				if (col >= across)
				{
					row++;
					col = 0;
				}
				imgOn++;
			}
		}
		
		public override void TouchesCancelled (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesCancelled (touches, evt);
			
			touchingImage = null;
			touchingImageRect = RectangleF.Empty;	
			
			SetNeedsDisplay();
		}
		
		
		public override void TouchesMoved (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesMoved (touches, evt);
		} 
		public override void TouchesEnded (Foundation.NSSet touches, UIEvent evt)
		{
			base.TouchesEnded (touches, evt);
						
			if (touchingImage == null || touchingImageRect == RectangleF.Empty)
			{
				touchingImage = null;
				touchingImageRect = RectangleF.Empty;
				touchingImageIndex = -1;
				//base.TouchesEnded(touches, evt);
				return;
			}
			
			var touchLocation =	((UITouch)evt.TouchesForView(this).AnyObject).LocationInView(this);
			var touchFrame = new RectangleF(touchLocation.X, touchLocation.Y, 1, 1);
			
			if (touchFrame.IntersectsWith(touchingImageRect))
			{
				int touchedImageIndex = touchingImageIndex;
				
				touchingImage = null;
				touchingImageRect = RectangleF.Empty;
				touchingImageIndex = -1;
				
				//Fire off delegate
				if (this.ImageTapped != null)
					this.ImageTapped(touchedImageIndex);
			}
			
			SetNeedsDisplay();
			
		}
		
	
	}
}

