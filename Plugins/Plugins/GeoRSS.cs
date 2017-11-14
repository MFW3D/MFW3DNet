using System;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Windows;
using System.Windows.Forms;

using MFW3D;
using MFW3D.Menu;
using MFW3D.Renderable;

using Collab.jhuapl.Whiteboard;
using MFW3D.NewWidgets;
using System.Text.RegularExpressions;
using MFW3D.GeoRSS;

namespace jhuapl.sample
{
	/// <summary>
	/// 
	/// </summary>
	public class GeoRSSLoader : MFW3D.PluginEngine.Plugin
	{
		protected GeoRSSMenuButton m_menuButton;

		/// <summary>
		/// This is the layer holding all geoRSS feeds
		/// </summary>
		public RenderableObjectList FeedLayer
		{
			get { return m_feedLayer; }
		}
		protected RenderableObjectList m_feedLayer;

        /// <summary>
		/// This is the test layer holding the WorldWindCentral feed
		/// </summary>
		public Icons WWCLayer
		{
			get { return m_wwcLayer; }
		}
		protected Icons m_wwcLayer;

		/// <summary>
		/// The whiteboard menu item
		/// </summary>
		public System.Windows.Forms.MenuItem GeoRSSMenu
		{
			get { return m_geoRSSMenuItem; }
		}
		protected System.Windows.Forms.MenuItem m_geoRSSMenuItem;


        public GeoRssFeeds Feeds
        {
            get { return m_feeds; }
            set { m_feeds = value; }
        }
        GeoRssFeeds m_feeds;



		public GeoRSSLoader()
		{
		}

        public override void Load()
        {
            // Create our whiteboard layer
            m_feedLayer = new RenderableObjectList("GeoRSS Feeds");
            Global.worldWindow.CurrentWorld.RenderableObjects.Add(m_feedLayer);

            m_feeds = new GeoRssFeeds(m_feedLayer);

            // Add our menu button
            m_menuButton = new GeoRSSMenuButton(this.PluginDirectory + @"\Plugins\GeoRSS\georss-large.png", this);

            // Add our navigation menu item
            m_geoRSSMenuItem = new System.Windows.Forms.MenuItem();
            m_geoRSSMenuItem.Text = "Load GeoRSS Feed";
            m_geoRSSMenuItem.Click += new System.EventHandler(WbMenuItem_Click);

            m_feeds.Add("WWC Hotspot Feed",
                        "http://www.worldwindcentral.com/hotspots/rss.php",
                        new TimeSpan(1, 0, 0),
                        this.PluginDirectory + @"\Plugins\GeoRSS\wwc.png");
            m_feeds.Add("CNN Latest News Feed",
                        "http://ws.geonames.org/rssToGeoRSS?feedUrl=http://rss.cnn.com/rss/cnn_latest.rss",
                        new TimeSpan(0, 15, 0),
                        this.PluginDirectory + @"\Plugins\GeoRSS\cnn.png");
            m_feeds.Add("CNN World News Feed",
                        "http://ws.geonames.org/rssToGeoRSS?feedUrl=http://rss.cnn.com/rss/cnn_world.rss",
                        new TimeSpan(0, 15, 0),
                        this.PluginDirectory + @"\Plugins\GeoRSS\cnn.png");
            m_feeds.Add("CNN US News Feed",
                        "http://ws.geonames.org/rssToGeoRSS?feedUrl=http://rss.cnn.com/rss/cnn_us.rss",
                        new TimeSpan(0, 15, 0),
                        this.PluginDirectory + @"\Plugins\GeoRSS\cnn.png");
            m_feeds.Add("Disaster Feed",
                        "http://www.rsoe.hu/hisz/rss/disrss-eng.php",
                        new TimeSpan(0, 15, 0),
                        this.PluginDirectory + @"\Plugins\GeoRSS\rsoe.png");


            m_feeds.Start();

            base.Load();
        }
	
		public override void Unload()
		{
			base.Unload ();
		}

		protected void WbMenuItem_Click(object sender, EventArgs s)
		{
		}

		protected void keyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyData==Keys.W)
			{
				WbMenuItem_Click(sender, e);
			} 
		}
	}

	/// <summary>
	/// 
	/// </summary>
	public class GeoRSSMenuButton : MenuButton
	{
		#region к╫спЁит╠

		// The plugin associated with this button object
        internal static GeoRSSLoader m_plugin;

		protected bool m_setFlag = true;

		#endregion

        public GeoRSSMenuButton(string buttonIconPath, GeoRSSLoader plugin)
            : base(buttonIconPath)
		{
			m_plugin = plugin;
			this.Description = "Refresh GeoRSS Feeds";
			this.SetPushed(true);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}

		public override void Update(DrawArgs drawArgs)
		{
		}

		public override bool IsPushed()
		{
			return m_plugin.FeedLayer.IsOn;
		}

		public override void SetPushed(bool isPushed)
		{
            m_plugin.FeedLayer.IsOn = isPushed;

            if (isPushed)
                m_plugin.Feeds.ControlForm.Show();
            else
                m_plugin.Feeds.ControlForm.Hide();

		}

        public string StripTags(string text)
        {
            string stripped = Regex.Replace(text, Regex.Escape("<br>"), "\n", RegexOptions.IgnoreCase);
            return Regex.Replace(stripped, "<.*?>", string.Empty, RegexOptions.Compiled);
        }

        public override void Render(DrawArgs drawArgs)
        {
        }

        public override bool OnMouseUp(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseDown(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }

        public override void OnKeyUp(KeyEventArgs keyEvent)
        {
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
        }
    }
}