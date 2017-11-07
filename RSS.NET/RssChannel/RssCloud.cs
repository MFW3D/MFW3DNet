using System;

namespace Rss
{
	/// <summary>Allow processes to register with a cloud to be notified of updates to the channel.</summary>
	[Serializable()]
	public class RssCloud : RssElement
	{
		private RssCloudProtocol protocol = RssCloudProtocol.Empty;
		private string domain = RssDefault.String;
		private string path = RssDefault.String;
		private string registerProcedure = RssDefault.String;
		private int port = RssDefault.Int;
		/// <summary>Initialize a new instance of the RssCloud class.</summary>
		public RssCloud() {}
		/// <summary>Domain name or IP address of the cloud</summary>
		public string Domain
		{
			get { return domain; }
			set { domain = RssDefault.Check(value); }
		}
		/// <summary>TCP port that the cloud is running on</summary>
		public int Port
		{
			get { return port; }
			set { port = RssDefault.Check(value); }
		}
		/// <summary>Location of its responder</summary>
		public string Path
		{
			get { return path; }
			set { path = RssDefault.Check(value); }
		}

		/// <summary>Name of the procedure to call to request notification</summary>
		public string RegisterProcedure
		{
			get { return registerProcedure; }
			set { registerProcedure = RssDefault.Check(value); }
		}
		/// <summary>Protocol used</summary>
		public RssCloudProtocol Protocol
		{
			get { return protocol; }
			set { protocol = value; }
		}
	}
}
