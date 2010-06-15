using System;
using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;

namespace MonoTouch.ImageGallery
{
	public class IGImage<TKey>
	{
		public IGImage (TKey id, string url, string title, string caption)
		{
			this.Id = id;
			this.Url = url;
			this.Title = title;
			this.Caption = caption;
		}
		
		public TKey Id
		{
			get;set;
		}
				
		public string Title
		{
			get;set;	
		}
		
		public string Caption
		{
			get;set;
		}	
		
		public string Url
		{
			get;set;	
		}
	}
}

