using System;

namespace Rss
{
	/// <summary>A reference to an attachment to the item</summary>
	[Serializable()]
	public class RssEnclosure : RssElement
	{
		private Uri uri = RssDefault.Uri;
		private int length = RssDefault.Int;
		private string type = RssDefault.String;
		/// <summary>Initialize a new instance of the RssEnclosure class.</summary>
		public RssEnclosure() {}

        /// <summary>
        /// Creates a new <see cref="RssEnclosure"/> instance.
        /// </summary>
        /// <param name="url">URL.</param>
        /// <param name="length">Length.</param>
        /// <param name="type">Type.</param>
	    public RssEnclosure(Uri url, int length, string type)
	    {
            if (url == null) throw new ArgumentNullException("url");
            if (type == null) throw new ArgumentNullException("type");
            if (type.Length == 0) throw new ArgumentException("A non zero-length string is required.", "type");
            if (length < 0) throw new ArgumentOutOfRangeException("length");
	        this.uri = url;
	        this.length = length;
	        this.type = type;
	    }

	    /// <summary>Where the enclosure is located</summary>
		public Uri Url
		{
			get { return uri; }
			set { uri= RssDefault.Check(value); }
		}
		/// <summary>The size of the enclosure, in bytes</summary>
		/// <remarks>-1 represents a null.</remarks>
		public int Length
		{
			get { return length; }
			set { length = RssDefault.Check(value); }
		}
		/// <summary>A standard Multipurpose Internet Mail Extensions (MIME) type</summary>
		public string Type
		{
			get { return type; }
			set { type = RssDefault.Check(value); }
		}
	}
}
