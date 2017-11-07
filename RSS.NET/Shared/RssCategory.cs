using System;

namespace Rss
{
	/// <summary>Provide information regarding the location of the subject matter of the channel in a taxonomy</summary>
	[Serializable()]
	public class RssCategory : RssElement
	{
		private string name = RssDefault.String;
		private string domain = RssDefault.String;

		/// <summary>Initialize a new instance of the RssCategory class</summary>
		public RssCategory() {}

		/// <summary>Actual categorization given for this item, within the chosen taxonomy</summary>
		public string Name
		{
			get { return name; }
			set { name = RssDefault.Check(value); }
		}
		/// <summary>URL of external taxonomy</summary>
		public string Domain
		{
			get { return domain; }
			set { domain = RssDefault.Check(value); }
		}
	}

}
