using System;

namespace Rss
{
	/// <summary>Globally unique identifier</summary>
	[Serializable()]
	public class RssGuid : RssElement
	{
		private DBBool permaLink = DBBool.Null;
		private string name = RssDefault.String;
		/// <summary>Initialize a new instance of the RssGuid class.</summary>
		public RssGuid() {}
		/// <summary>If true, a url that can be opened in a web browser that points to the item</summary>
		public DBBool PermaLink
		{
			get { return permaLink; }
			set { permaLink = value; }
		}
		/// <summary>Globally unique identifier value</summary>
		public string Name
		{
			get { return name; }
			set { name = RssDefault.Check(value); }
		}
	}
}
