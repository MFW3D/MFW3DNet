//----------------------------------------------------------------------------
// NAME: WiimoteHeadtracker
// VERSION: 0.1
// DESCRIPTION: Head Tracking via the Wiimote. Based on Johnny Lee's WiiDesktopVR
// DEVELOPER: Nigel Tzeng
// WEBSITE: http://www.jhuapl.edu
// REFERENCES: 
//----------------------------------------------------------------------------
//========================= (UNCLASSIFIED) ==============================
// Copyright © 2008 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
// WiiDesktopVR Code - Copyright 2007 Johnny Lee
//
// From WiiDesktopVR Readme:
//
// "This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
// FITNESS FOR A PARTICULAR PURPOSE. Use of this software is entirely at your 
// own risk. It is provided without any support beyond this document."  
//
// WiimoteLib Code - Copyright 2007 Brian Peek
// Modified under the MS-PL License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2005 The Johns Hopkins University. 
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
using System.Windows.Forms;

using WorldWind;
using WiimoteLib;

namespace jhuapl.util
{
	/// <summary>
	/// The plugin (main class)
	/// </summary>
    public class WiimoteHeadtracker : WorldWind.PluginEngine.Plugin 
	{
        Wiimote remote;
        float dotDistanceInMM = 8.5f * 25.4f;//width of the wii sensor bar
        float screenHeightinMM = 20 * 25.4f;
        float radiansPerPixel = (float)(Math.PI / 4) / 1024.0f; //45 degree field of view with a 1024x768 camera
        float m_xScaling = 0.5f;
        float m_yScaling = 0.75f;

        float cameraVerticaleAngle = 0; //begins assuming the camera is point straight forward
        float relativeVerticalAngle = 0; //current head position view angle
        bool cameraIsAboveScreen = false;//has no affect until zeroing and then is set automatically.

        Point2D[] wiimotePointsNormalized = new Point2D[4];

        //headposition
        float headX = 0;
        float headY = 0;
        float headDist = 2;

        KeyEventHandler m_keyHandler;
        public System.Windows.Forms.MenuItem m_enableMenuItem;

        bool m_calibrate = false;

		/// <summary>
		/// Plugin entry point 
		/// </summary>
		public override void Load() 
		{
            try
            {
                remote = new Wiimote();
                remote.Connect();
                remote.SetReportType(Wiimote.InputReport.IRAccel, true);
                remote.GetStatus();
                remote.SetLEDs(true, false, false, false);
                remote.WiimoteChanged += new WiimoteChangedEventHandler(OnWiimoteChanged);

                for (int i = 0; i < 4; i++)
                {
                    wiimotePointsNormalized[i] = new Point2D();
                }

                m_enableMenuItem = new MenuItem();
                m_enableMenuItem.Text = "Enable Headtracking\t*";
                m_enableMenuItem.Click += new System.EventHandler(enableMenuItem_Click);
                ParentApplication.ToolsMenu.MenuItems.Add(m_enableMenuItem);

                m_keyHandler = new KeyEventHandler(keyUp);
                ParentApplication.WorldWindow.KeyUp += m_keyHandler;

                World.Settings.AllowNegativeTilt = false;
                World.Settings.CameraHeadTracking = false;
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot find a wii remote: " + e.Message);
                remote = null;
            }
		}

		/// <summary>
		/// Unloads our plugin
		/// </summary>
		public override void Unload() 
		{
            if (remote != null)
                remote.Disconnect();
            ParentApplication.WorldWindow.KeyUp -= m_keyHandler;
		}


        void OnWiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            ParseWiimoteData();
        }

        public void CalibrateWiimote()
        {
            //zeros the head position and computes the camera tilt
            double angle = Math.Acos(.5 / headDist) - Math.PI / 2;//angle of head to screen
            if (!cameraIsAboveScreen)
                angle = -angle;
            cameraVerticaleAngle = (float)angle; // (float)((angle - relativeVerticalAngle));//absolute camera angle 
            m_calibrate = false; 
        }

        public void ParseWiimoteData()
        {
            if (remote == null || remote.WiimoteState == null)
                return;

            Point2D firstPoint = new Point2D();
            Point2D secondPoint = new Point2D();
            int numvisible = 0;

            if (remote.WiimoteState.IRState.Found1)
            {
                wiimotePointsNormalized[0].x = 1.0f - remote.WiimoteState.IRState.RawX1 / 768.0f;
                wiimotePointsNormalized[0].y = remote.WiimoteState.IRState.RawY1 / 768.0f;
                firstPoint.x = remote.WiimoteState.IRState.RawX1;
                firstPoint.y = remote.WiimoteState.IRState.RawY1;
                numvisible = 1;
            }
            else
            {
                //not visible
            }

            if (remote.WiimoteState.IRState.Found2)
            {
                wiimotePointsNormalized[1].x = 1.0f - remote.WiimoteState.IRState.RawX2 / 768.0f;
                wiimotePointsNormalized[1].y = remote.WiimoteState.IRState.RawY2 / 768.0f;
                if (numvisible == 0)
                {
                    firstPoint.x = remote.WiimoteState.IRState.RawX2;
                    firstPoint.y = remote.WiimoteState.IRState.RawY2;
                    numvisible = 1;
                }
                else
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX2;
                    secondPoint.y = remote.WiimoteState.IRState.RawY2;
                    numvisible = 2;
                }
            }
            else
            {
                //not visible
            }
            if (remote.WiimoteState.IRState.Found3)
            {
                wiimotePointsNormalized[2].x = 1.0f - remote.WiimoteState.IRState.RawX3 / 768.0f;
                wiimotePointsNormalized[2].y = remote.WiimoteState.IRState.RawY3 / 768.0f;
                if (numvisible == 0)
                {
                    firstPoint.x = remote.WiimoteState.IRState.RawX3;
                    firstPoint.y = remote.WiimoteState.IRState.RawY3;
                    numvisible = 1;
                }
                else if (numvisible == 1)
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX3;
                    secondPoint.y = remote.WiimoteState.IRState.RawY3;
                    numvisible = 2;
                }
            }
            else
            {
                //not visible
            }
            if (remote.WiimoteState.IRState.Found4)
            {
                wiimotePointsNormalized[3].x = 1.0f - remote.WiimoteState.IRState.RawX4 / 768.0f;
                wiimotePointsNormalized[3].y = remote.WiimoteState.IRState.RawY4 / 768.0f;
                if (numvisible == 1)
                {
                    secondPoint.x = remote.WiimoteState.IRState.RawX4;
                    secondPoint.y = remote.WiimoteState.IRState.RawY4;
                    numvisible = 2;
                }
            }
            else
            {
                //not visible
            }

            if (numvisible == 2)
            {
                float dx = firstPoint.x - secondPoint.x;
                float dy = firstPoint.y - secondPoint.y;
                float pointDist = (float)Math.Sqrt(dx * dx + dy * dy);

                float angle = radiansPerPixel * pointDist / 2;
                //in units of screen hieght since the box is a unit cube and box hieght is 1
                headDist =  (float)((dotDistanceInMM / 2) / Math.Tan(angle)) / screenHeightinMM;

                float avgX = (firstPoint.x + secondPoint.x) / 2.0f;
                float avgY = (firstPoint.y + secondPoint.y) / 2.0f;

                //should  calaculate based on distance

                headX = (float)(m_xScaling * Math.Sin(radiansPerPixel * (avgX - 512)) * headDist);

                relativeVerticalAngle = (avgY - 384) * radiansPerPixel;//relative angle to camera axis

                if (m_calibrate)
                    CalibrateWiimote();

                if (cameraIsAboveScreen)
                    headY = .5f + (float)(m_yScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
                else
                    headY = .5f - (float)(m_yScaling * Math.Sin(relativeVerticalAngle + cameraVerticaleAngle) * headDist);
            }

            // compute tilt, swivel and zoom
            DrawArgs.Camera.HeadTilt = Angle.FromRadians(headY);
            DrawArgs.Camera.HeadSwivel = Angle.FromRadians(-headX);
            DrawArgs.Camera.HeadZoom = headDist * 1000;
        }

        protected void enableMenuItem_Click(object sender, EventArgs s)
        {
            if (World.Settings.CameraHeadTracking)
            {
                World.Settings.CameraHeadTracking = false;
                World.Settings.AllowNegativeTilt = false;
                m_enableMenuItem.Text = "Enable Headtracking\t*";
            }
            else
            {
                World.Settings.CameraHeadTracking = true;
                World.Settings.AllowNegativeTilt = true;
                m_enableMenuItem.Text = "Disable Headtracking\t*";

                m_calibrate = true;
            }
        }

        protected void keyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Multiply)
            {
                enableMenuItem_Click(sender, e);
            }
        }
	}

    class Point2D
    {
        public float x = 0.0f;
        public float y = 0.0f;
        public void set(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
