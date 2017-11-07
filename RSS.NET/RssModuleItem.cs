using System;

namespace Rss
{
	/// <summary>A module may contain any number of items (either channel-based or item-based).</summary>
	[Serializable()]
	public class RssModuleItem : RssElement
	{
		private bool _bRequired = false;
		private string _sElementName = RssDefault.String;
		private string _sElementText = RssDefault.String;
		private RssModuleItemCollection _rssSubElements = new RssModuleItemCollection();

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		public RssModuleItem()
		{
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		public RssModuleItem(string name)
		{
			this._sElementName = RssDefault.Check(name);
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		/// <param name="required">Is text required for this RssModuleItem?</param>
		public RssModuleItem(string name, bool required) : this(name)
		{
			this._bRequired = required;
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		/// <param name="text">The text contained within this RssModuleItem.</param>
		public RssModuleItem(string name, string text) : this(name)
		{
			this._sElementText = RssDefault.Check(text);
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		/// <param name="required">Is text required for this RssModuleItem?</param>
		/// <param name="text">The text contained within this RssModuleItem.</param>
		public RssModuleItem(string name, bool required, string text) : this(name, required)
		{
			this._sElementText = RssDefault.Check(text);
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		/// <param name="text">The text contained within this RssModuleItem.</param>
		/// <param name="subElements">The sub-elements of this RssModuleItem (if any exist).</param>
		public RssModuleItem(string name, string text, RssModuleItemCollection subElements) : this(name, text)
		{
			this._rssSubElements = subElements;
		}

		/// <summary>Initialize a new instance of the RssModuleItem class</summary>
		/// <param name="name">The name of this RssModuleItem.</param>
		/// <param name="required">Is text required for this RssModuleItem?</param>
		/// <param name="text">The text contained within this RssModuleItem.</param>
		/// <param name="subElements">The sub-elements of this RssModuleItem (if any exist).</param>
		public RssModuleItem(string name, bool required, string text, RssModuleItemCollection subElements) : this(name, required, text)
		{
			this._rssSubElements = subElements;
		}

		/// <summary>Returns a string representation of the current Object.</summary>
		/// <returns>The item's title, description, or "RssModuleItem" if the title and description are blank.</returns>
		public override string ToString()
		{
			if (Name != RssDefault.String)
				return Name;
			else if (Text != RssDefault.String)
				return Text;
			else
				return "RssModuleItem";
		}

		/// <summary>
		/// The name of this RssModuleItem.
		/// </summary>
		public string Name
		{
			get { return this._sElementName; }
			set { this._sElementName = RssDefault.Check(value); }
		}

		/// <summary>
		/// The text contained within this RssModuleItem.
		/// </summary>
		public string Text
		{
			get { return this._sElementText; }
			set { this._sElementText = RssDefault.Check(value); }
		}

		/// <summary>
		/// The sub-elements of this RssModuleItem (if any exist).
		/// </summary>
		public RssModuleItemCollection SubElements
		{
			get { return this._rssSubElements; }
			set { this._rssSubElements = value;}
		}

		/// <summary>
		/// Is text for this element required?
		/// </summary>
		public bool IsRequired
		{
			get { return this._bRequired; }
		}
	}
}
