/* Rss20Writer.cs
 * ==========
 * 
 * RSS.NET (http://rss-net.sf.net/)
 * Copyright © 2002 - 2005 George Tsiokos. All Rights Reserved.
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
