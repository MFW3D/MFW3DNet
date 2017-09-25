using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WorldWind.GeoRSS
{
    public partial class GeoRssForm : Form
    {
        public GeoRssForm(GeoRssFeeds feed)
        {
            InitializeComponent();

            geoRSSFeedControl1.m_feeds = feed;
            geoRSSFeedControl1.UpdateDataGridView();
        }

        internal void UpdateDataGridView()
        {
            geoRSSFeedControl1.UpdateDataGridView();
        }

        private void GeoRssForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();
            } 
        }
    }
}