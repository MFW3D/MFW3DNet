using System;

namespace Rss
{
	/// <summary>A link and description for a graphic that represent a channel</summary>
	[Serializable()]
	public class RssImage : RssElement
	{
		private string title = RssDefault.String;
		private string description = RssDefault.String;
		private Uri uri = RssDefault.Uri;
		private Uri link = RssDefault.Uri;
		private int width = RssDefault.Int;
		private int height = RssDefault.Int;

		/// <summary>Initialize a new instance of the RssImage class.</summary>
		internal RssImage() {}

        /// <summary>
        /// Creates a new <see cref="RssImage"/> instance.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="uri">URI.</param>
        /// <param name="link">Link.</param>
		public RssImage(string title, Uri uri, Uri link)
		{
            if (title == null) throw new ArgumentNullException("title");
            if (title.Length == 0) throw new ArgumentException("A non zero-length string is required.", "title");
            if (uri == null) throw new ArgumentNullException("uri");
            if (link == null) throw new ArgumentNullException("link");
		    this.title = title;
            this.uri = uri;
            this.link = link;
		}

		/// <summary>The URL of a GIF, JPEG or PNG image that represents the channel.</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public Uri Url
		{
			get { return uri; }
			set { uri = RssDefault.Check(value); }
		}
		/// <summary>Describes the image, it's used in the ALT attribute of the HTML img tag when the channel is rendered in HTML.</summary>
		/// <remarks>Maximum length is 100 (For RSS 0.91).</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>The URL of the site, when the channel is rendered, the image is a link to the site.</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91).</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
		/// <summary>Contains text that is included in the TITLE attribute of the link formed around the image in the HTML rendering.</summary>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>Width of image in pixels</summary>
		/// <remarks>Maximum value for height is 400 (For RSS 0.91)</remarks>
		public int Width
		{
			get { return width; }
			set { width = RssDefault.Check(value); }
		}
		/// <summary>Height of image in pixels</summary>
		/// <remarks>Maximum value for width is 144 (For RSS 0.91)</remarks>
		public int Height
		{
			get { return height; }
			set { height = RssDefault.Check(value); }
		}
	}
}
