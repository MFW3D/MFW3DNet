//----------------------------------------------------------------------------
// NAME: GeoRSS
// VERSION: 0.9
// DESCRIPTION: GeoRSS example code
// DEVELOPER: Nigel Tzeng
// WEBSITE: http://www.jhuapl.edu
// REFERENCES:
//----------------------------------------------------------------------------
//========================= (UNCLASSIFIED) ==============================
// Copyright © 2005-2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
//
// This software was developed at The Johns Hopkins University/Applied 
// Physics Laboratory (“JHU/APL”) that is the author thereof under the 
// “work made for hire” provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software”), to use the 
// Software without restriction, including without limitation the rights 
// to copy, modify, merge, publish, distribute, sublicense, and/or sell 
// copies of the Software, and to permit others to do so, subject to the 
// following conditions: 
//
// 1.  This LICENSE AND DISCLAIMER, including the copyright notice, shall 
//     be included in all copies of the Software, including copies of 
//     substantial portions of the Software; 
//
// 2.  JHU/APL assumes no obligation to provide support of any kind with 
//     regard to the Software.  This includes no obligation to provide 
//     assistance in using the Software nor to provide updated versions of 
//     the Software; and 
//
// 3.  THE SOFTWARE AND ITS DOCUMENTATION ARE PROVIDED AS IS AND WITHOUT 
//     ANY EXPRESS OR IMPLIED WARRANTIES WHATSOEVER.  ALL WARRANTIES 
//     INCLUDING, BUT NOT LIMITED TO, PERFORMANCE, MERCHANTABILITY, FITNESS
//     FOR A PARTICULAR PURPOSE, AND NONINFRINGEMENT ARE HEREBY DISCLAIMED.  
//     USERS ASSUME THE ENTIRE RISK AND LIABILITY OF USING THE SOFTWARE.  
//     USERS ARE ADVISED TO TEST THE SOFTWARE THOROUGHLY BEFORE RELYING ON 
//     IT.  IN NO EVENT SHALL THE JOHNS HOPKINS UNIVERSITY BE LIABLE FOR 
//     ANY DAMAGES WHATSOEVER, INCLUDING, WITHOUT LIMITATION, ANY LOST 
//     PROFITS, LOST SAVINGS OR OTHER INCIDENTAL OR CONSEQUENTIAL DAMAGES, 
//     ARISING OUT OF THE USE OR INABILITY TO USE THE SOFTWARE. 
//
using System;
using System.Threading;
using System.Timers;
using System.Collections;
using System.Windows;
using System.Windows.Forms;

using WorldWind;
using WorldWind.Menu;
using WorldWind.Renderable;

using Collab.jhuapl.Whiteboard;
using WorldWind.NewWidgets;
using System.Text.RegularExpressions;
using WorldWind.GeoRSS;

namespace jhuapl.sample
{
	/// <summary>
	/// 
	/// </summary>
	public class GeoRSSLoader : WorldWind.PluginEngine.Plugin
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
            ParentApplication.WorldWindow.CurrentWorld.RenderableObjects.Add(m_feedLayer);

            m_feeds = new GeoRssFeeds(m_feedLayer);

            // Add our menu button
            m_menuButton = new GeoRSSMenuButton(this.PluginDirectory + @"\Plugins\GeoRSS\georss-large.png", this);
            ParentApplication.WorldWindow.MenuBar.AddToolsMenuButton(m_menuButton);

            // Add our navigation menu item
            m_geoRSSMenuItem = new System.Windows.Forms.MenuItem();
            m_geoRSSMenuItem.Text = "Load GeoRSS Feed";
            m_geoRSSMenuItem.Click += new System.EventHandler(WbMenuItem_Click);
            ParentApplication.ToolsMenu.MenuItems.Add(m_geoRSSMenuItem);

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
		#region Private Members

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