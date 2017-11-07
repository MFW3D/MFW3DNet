using System;

namespace Rss
{
	/// <summary>Describes an items source</summary>
	[Serializable()]
	public class RssSource : RssElement
	{
		private string name = RssDefault.String;
		private Uri uri = RssDefault.Uri;
		/// <summary>Initialize a new instance of the RssSource class</summary>
		internal RssSource() {}

        /// <summary>
        /// Creates a new <see cref="RssSource"/> instance.
        /// </summary>
        /// <param name="url">URL.</param>
	    public RssSource(Uri url)
	    {
            if (url == null) throw new ArgumentNullException("url");
	        this.uri = url;
	    }

	    /// <summary>Name of the RSS channel that the item came from</summary>
		public string Name
		{
			get { return name; }
			set { name = RssDefault.Check(value); }
		}
		/// <summary>URL of the original RSS feed from which the item was republished</summary>
		public Uri Url
		{
			get { return uri; }
			set { uri = RssDefault.Check(value); }
		}
	}
}
