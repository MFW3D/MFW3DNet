using System;
using System.Collections;

namespace Rss
{
	/// <summary>Base class for all RSS modules</summary>
	[Serializable()]
	public abstract class RssModule
	{
		private ArrayList _alBindTo = new ArrayList();
		private RssModuleItemCollection _rssChannelExtensions = new RssModuleItemCollection();
		private RssModuleItemCollectionCollection _rssItemExtensions = new RssModuleItemCollectionCollection();

		/// <summary>Initialize a new instance of the RssModule class</summary>
		protected RssModule() {}

		/// <summary>Collection of RSSModuleItem that are to be placed in the channel</summary>
		internal RssModuleItemCollection ChannelExtensions
		{
			get { return this._rssChannelExtensions; }
			set { this._rssChannelExtensions = value; }
		}

		/// <summary>Collection of RSSModuleItemCollection that are to be placed in the channel item</summary>
		internal RssModuleItemCollectionCollection ItemExtensions
		{
			get { return this._rssItemExtensions; }
			set { this._rssItemExtensions = value; }
		}

		/// <summary>Prefix for the given module namespace</summary>
		public abstract string NamespacePrefix{get;}

		/// <summary>URL for the given module namespace</summary>
		public abstract Uri NamespaceURL {get;}

		/// <summary>Bind a particular channel to this module</summary>
		/// <param name="channelHashCode">Hash code of the channel</param>
		public void BindTo(int channelHashCode)
		{
			this._alBindTo.Add(channelHashCode);
		}

		/// <summary>Check if a particular channel is bound to this module</summary>
		/// <param name="channelHashCode">Hash code of the channel</param>
		/// <returns>true if this channel is bound to this module, otherwise false</returns>
		public bool IsBoundTo(int channelHashCode)
		{
			return (this._alBindTo.BinarySearch(0, this._alBindTo.Count, channelHashCode, null) >= 0);
		}
	}
}
