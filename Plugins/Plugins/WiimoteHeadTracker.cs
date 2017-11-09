using System;
using System.Windows.Forms;

using MFW3D;
using WiimoteLib;

namespace jhuapl.util
{
	/// <summary>
	/// The plugin (main class)
	/// </summary>
    public class WiimoteHeadtracker : MFW3D.PluginEngine.Plugin 
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

                m_keyHandler = new KeyEventHandler(keyUp);
                Global.worldWindow.KeyUp += m_keyHandler;

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
            Global.worldWindow.KeyUp -= m_keyHandler;
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
            }
            else
            {
                World.Settings.CameraHeadTracking = true;
                World.Settings.AllowNegativeTilt = true;
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
