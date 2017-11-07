using System;

namespace Rss
{
	/// <summary>A RSS module that adds elements at the channel or item level that specifies which Creative Commons license applies.</summary>
	public sealed class RssCreativeCommons : RssModule
	{
        private static Uri namespaceUri = new Uri("http://backend.userland.com/creativeCommonsRssModule");

        /// <summary>URL for the given module namespace</summary>
        public override Uri NamespaceURL
        {
            get
            {
                return namespaceUri;
            }
        }

        /// <summary>Prefix for the given module namespace</summary>
        public override string NamespacePrefix
        {
            get
            {
                return "creativeCommons";
            }
        }
		/// <summary>Initialize a new instance of the </summary>
		/// <param name="license">
		///		If present as a sub-element of channel, indicates that the content of the RSS file is available under a license, indicated by a URL, which is the value of the license element. A list of some licenses that may be used in this context is on the Creative Commons website on this page, however the license element may point to licenses not authored by Creative Commons.
		///		You may also use the license element as a sub-element of item. When used this way it applies only to the content of that item. If an item has a license, and the channel does too, the license on the item applies, i.e. the inner license overrides the outer one.
		///		Multiple license elements are allowed, in either context, indicating that the content is available under multiple licenses.
		///		<remarks>"http://www.creativecommons.org/licenses/"</remarks>
		///	</param>
		/// <param name="isChannelSubElement">If present as a sub-element of channel then true, otherwise false</param>
		public RssCreativeCommons(Uri license, bool isChannelSubElement)
		{
			if(isChannelSubElement)
			{
				base.ChannelExtensions.Add(new RssModuleItem("license", true, RssDefault.Check(license.ToString())));
			}
			else
			{
				RssModuleItemCollection rssItems = new RssModuleItemCollection();

				rssItems.Add(new RssModuleItem("license", true, RssDefault.Check(license.ToString())));

				base.ItemExtensions.Add(rssItems);
			}
		}
	}
}