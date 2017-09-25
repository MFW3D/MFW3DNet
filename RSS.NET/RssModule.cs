/* RssModule.cs
 * ============
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright � 2002 - 2005 George Tsiokos. All Rights Reserved.
 * 
 * RSS 2.0 (http://blogs.law.harvard.edu/tech/rss)
 * RSS 2.0 is offered by the Berkman Center for Internet & Society at 
 * Harvard Law School under the terms of the Attribution/Share Alike 
 * Creative Commons license.
 * 
 * Permission is hereby granted, free of charge, to any person obtaining 
 * a copy of this software and associated documentation files (the "Software"), 
 * to deal in the Software without restriction, including without limitation 
 * the rights to use, copy, modify, merge, publish, distribute, sublicense, 
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL 
 * THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
 * THE SOFTWARE.
*/
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
