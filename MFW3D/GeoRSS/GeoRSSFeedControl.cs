using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace MFW3D.GeoRSS
{
    public partial class GeoRSSFeedControl : UserControl
    {
        internal GeoRssFeeds m_feeds;

        internal bool m_updateNeeded;

        public GeoRSSFeedControl()
        {
            InitializeComponent();

            openFileDialog.InitialDirectory = Application.StartupPath + @"\Plugins\GeoRSS\";
            openFileDialog.FileName = "geoRSS-small.png";

            iconTextBox.Text = Application.StartupPath + @"\Plugins\GeoRSS\georss-small.png";
        }

        public GeoRSSFeedControl(GeoRssFeeds feeds)
        {
            InitializeComponent();

            m_feeds = feeds;

            UpdateDataGridView();
        }

        internal void UpdateDataGridView()
        {
            feedDataGridView.Rows.Clear();

            foreach (GeoRssFeed feed in m_feeds.Feeds)
            {
                DataGridViewRow row = new DataGridViewRow();

                DataGridViewImageCell imageCell = new DataGridViewImageCell();
                row.Cells.Add(imageCell);

                DataGridViewTextBoxCell nameCell = new DataGridViewTextBoxCell();
                nameCell.Value = feed.Name;
                row.Cells.Add(nameCell);

                DataGridViewTextBoxCell urlCell = new DataGridViewTextBoxCell();
                urlCell.Value = feed.Url;
                row.Cells.Add(urlCell);

                DataGridViewTextBoxCell refreshCell = new DataGridViewTextBoxCell();
                refreshCell.Value = feed.UpdateInterval.ToString();
                row.Cells.Add(refreshCell);

                DataGridViewTextBoxCell lastUpdateCell = new DataGridViewTextBoxCell();
                lastUpdateCell.Value = feed.LastUpdate.ToString();
                row.Cells.Add(lastUpdateCell);

                DataGridViewButtonCell buttonCell = new DataGridViewButtonCell();
                buttonCell.UseColumnTextForButtonValue = true;
                row.Cells.Add(buttonCell);

                feedDataGridView.Rows.Add(row);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            m_feeds.Add(nameTextBox.Text, urlTextBox.Text);
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            openFileDialog.ShowDialog();

        }
    }
}
