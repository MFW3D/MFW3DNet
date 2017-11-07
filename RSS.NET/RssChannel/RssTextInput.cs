using System;

namespace Rss
{
	/// <summary>Multi-purpose channel element for the purpose of allowing users to submit queries back to the publisher's site</summary>
	/// <remarks>Typically for a search or subscription</remarks>
	[Serializable()]
	public class RssTextInput : RssElement
	{
		private string title = RssDefault.String;
		private string description = RssDefault.String;
		private string name = RssDefault.String;
		private Uri link = RssDefault.Uri;
		
		/// <summary>Initialize a new instance of the RssTextInput class</summary>
		internal RssTextInput() {}

        /// <summary>
        /// Creates a new <see cref="RssTextInput"/> instance.
        /// </summary>
        /// <param name="title">Title.</param>
        /// <param name="name">Name.</param>
        /// <param name="description">Description.</param>
        /// <param name="link">Link.</param>
	    public RssTextInput(string title, string name, string description, Uri link)
	    {
            if (title == null) throw new ArgumentNullException("title");
            if (name == null) throw new ArgumentNullException("name");
            if (description == null) throw new ArgumentNullException("description");
            if (title.Length == 0) throw new ArgumentException("A non zero-length string is required.", "title");
            if (name.Length == 0) throw new ArgumentException("A non zero-length string is required.", "name");
            if (description.Length == 0) throw new ArgumentException("A non zero-length string is required.", "description");
            if (link == null) throw new ArgumentNullException("link");
	        this.title = title;
	        this.description = description;
	        this.name = name;
	        this.link = link;
	    }

	    /// <summary>The label of the submit button in the text input area</summary>
		///	<remarks>Maximum length is 100 (For RSS 0.91)</remarks>
		public string Title
		{
			get { return title; }
			set { title = RssDefault.Check(value); }
		}
		/// <summary>Explains the text input area</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public string Description
		{
			get { return description; }
			set { description = RssDefault.Check(value); }
		}
		/// <summary>The name of the text object in the text input area</summary>
		/// <remarks>Maximum length is 20 (For RSS 0.91).</remarks>
		public string Name
		{
			get { return name; }
			set { name = RssDefault.Check(value); }
		}
		/// <summary>The URL of the script that processes text input requests</summary>
		/// <remarks>Maximum length is 500 (For RSS 0.91)</remarks>
		public Uri Link
		{
			get { return link; }
			set { link = RssDefault.Check(value); }
		}
	}
}
