using System.Xml;

namespace Rss
{
	/// <summary>
	/// Summary description for Rss20Writer.
	/// </summary>
	public class Rss20Writer : RssWriter
	{
        /// <summary>
        /// Creates a new <see cref="Rss20Writer"/> instance.
        /// </summary>
		public Rss20Writer(XmlTextWriter writer) : base(writer)
		{
		}

        /// <summary>
        /// Gets the version.
        /// </summary>
        /// <value>The <see cref="RssVersion"/> of the output.</value>
        public override RssVersion Version
        {
            get
            {
                return RssVersion.RSS20;
            }
        }


        /// <summary>
        /// Opens the root RSS element.
        /// </summary>
        protected override void OpenRootElement()
        {
            Writer.WriteStartElement("rss");
            Writer.WriteAttributeString("version", Version.ToString());
            
            foreach(RssModule rssModule in Modules)
            {
                WriteAttribute("xmlns:" + rssModule.NamespacePrefix, rssModule.NamespaceURL.ToString(), true);
            }
        }
	}
}
