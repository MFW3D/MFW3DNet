//----------------------------------------------------------------------------------------------------------------------------
// NAME: GPSTracker
// DEVELOPER: Javier Santoro
// WEBSITE: http://www.worldwindcentral.com/wiki/Add-on:GPS_Tracker_(plugin)
// VERSION: V04R06 (May 16, 2007)
// COPYRIGHT NOTICE: Copyright 2007, Javier Santoro
//                   Permission is granted to use GpsTracker for non-commercial purposes. 
//                   Permission is granted to use GpsTracker source code for educational and non-commercial purposes.
//----------------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace GpsTracker
{
    public partial class PlaybackControl : Form
    {
        private int m_iSliderPosition = 0;
        private int m_iCurrentPosition = 0;
        private int m_iTotalLines = 0;
        private bool bUpdate;
        private GPSIcon m_gpsIcon;

        public PlaybackControl(GPSIcon gpsIcon, string sFileSource, int iTotalLines, int iCurrentLine)
        {
            InitializeComponent();

            m_gpsIcon = gpsIcon;
            this.Text = "File Playback Control :: " + sFileSource;
            m_iTotalLines = iTotalLines;
            m_iCurrentPosition = iCurrentLine;
            trackBarPlayback.SetRange(0, m_iTotalLines - 1);
            trackBarPlayback.Value = m_iCurrentPosition;
            bUpdate = true;
            labelPercentage.Text = "0%";
        }

        public void UpdatePosition(int iCurrentLine)
        {
            if (bUpdate)
            {
                if (iCurrentLine >= m_iTotalLines)
                    iCurrentLine = m_iTotalLines - 1;

                m_iCurrentPosition = iCurrentLine;
                trackBarPlayback.Value = m_iCurrentPosition;
            }
        }

        void trackBarPlayback_ValueChanged(object sender, System.EventArgs e)
        {
            labelPercentage.Text = Convert.ToString((trackBarPlayback.Value * 100) / m_iTotalLines) + "%";
        }

        private void buttonPlayPause_Click(object sender, EventArgs e)
        {
            m_gpsIcon.PlaybackPlayPause(true);
            this.Text = "File Playback Control :: " + m_gpsIcon.m_RenderInfo.sDescription + " (Paused)";
        }

        private void PlaybackControl_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_gpsIcon.PlaybackControlClosed();

        }

        private void trackBarPlayback_MouseDown(object sender, MouseEventArgs e)
        {
            bUpdate = false;
            m_iSliderPosition = trackBarPlayback.Value;

        }

        private void trackBarPlayback_MouseUp(object sender, MouseEventArgs e)
        {
            bUpdate = true;
            if (trackBarPlayback.Value != m_iSliderPosition)
                m_gpsIcon.PlaybackUpdatePosition(trackBarPlayback.Value);
        }

        private void buttonPlay_Click(object sender, EventArgs e)
        {
            m_gpsIcon.PlaybackPlayPause(false);
            this.Text = "File Playback Control :: " + m_gpsIcon.m_RenderInfo.sDescription;
        }

        private void buttonRestart_Click(object sender, EventArgs e)
        {
            m_gpsIcon.PlaybackUpdatePosition(0);
        }
    }
}