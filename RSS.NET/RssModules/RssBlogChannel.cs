using System;

namespace Rss
{
	/// <summary>A RSS module that adds elements at the channel level that are common to weblogs.</summary>
	public sealed class RssBlogChannel : RssModule
	{
        private static Uri blogNamespaceUri = new Uri("http://backend.userland.com/blogChannelModule");

        /// <summary>URL for the given module namespace</summary>
        public override Uri NamespaceURL
        {
            get
            {
                return blogNamespaceUri;
            }
        }

        /// <summary>Prefix for the given module namespace</summary>
        public override string NamespacePrefix
        {
            get
            {
                return "blogChannel";
            }
        }


	    /// <summary>Initialize a new instance of the </summary>
		/// <param name="blogRoll">The URL of an OPML file containing the blogroll for the site.</param>
		/// <param name="mySubscriptions">The URL of an OPML file containing the author's RSS subscriptions.</param>
		/// <param name="blink">
		///		The URL of a weblog that the author of the weblog is promoting per Mark Pilgrim's description.
		///		<remarks>"http://diveintomark.org/archives/2002/09/17.html#blink_and_youll_miss_it"</remarks>
		///	</param>
		/// <param name="changes">
		///		The URL of a changes.xml file. When the feed that contains this element updates, it pings a server that updates this file. The presence of this element says to aggregators that they only have to read the changes file to see if this feed has updated. If several feeds point to the same changes file, the aggregator has to do less polling, resulting in better use of server bandwidth, and the Internet as a whole; and resulting in faster scans. Everyone wins. For more technical information, see the howto on the XML-RPC site.
		///		<remarks>"http://www.xmlrpc.com/weblogsComForRss"</remarks>
		/// </param>
		public RssBlogChannel(Uri blogRoll, Uri mySubscriptions, Uri blink, Uri changes) 
		{
			base.ChannelExtensions.Add(new RssModuleItem("blogRoll", true, RssDefault.Check(blogRoll.ToString())));
			base.ChannelExtensions.Add(new RssModuleItem("mySubscriptions", true, RssDefault.Check(mySubscriptions.ToString())));
			base.ChannelExtensions.Add(new RssModuleItem("blink", true, RssDefault.Check(blink.ToString())));
			base.ChannelExtensions.Add(new RssModuleItem("changes", true, RssDefault.Check(changes.ToString())));
		}
	}
}