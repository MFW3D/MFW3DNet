//----------------------------------------------------------------------------
// NAME: SpacePilot
// VERSION: 0.9
// DESCRIPTION: SpacePilot Controller. Based on Bjorn Reppen's (Mashi) joystick plugin
// DEVELOPER: Nigel Tzeng
// WEBSITE: http://www.jhuapl.edu
// REFERENCES: System.Data, Microsoft.DirectX.DirectInput
//----------------------------------------------------------------------------
//========================= (UNCLASSIFIED) ==============================
// Copyright © 2005-2006 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
// Joystick Plugin v1.0 was released to the public domain.
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
using Microsoft.DirectX;
using Microsoft.DirectX.DirectInput;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using WorldWind.Renderable;
using WorldWind.Camera;
using WorldWind;
using System.IO;
using System;
using System.Threading;
using System.Reflection;
using System.Drawing;

namespace jhuapl.util
{
	/// <summary>
	/// The plugin (main class)
	/// </summary>
	public class SpacePilot : WorldWind.PluginEngine.Plugin 
	{
		public enum ButtonType
		{
			BUTTON0 = 0,
			BUTTON1,
			BUTTON2,			
			BUTTON3,
			BUTTON4,
			BUTTON5,
			BUTTON_T,
			BUTTON_L,			
			BUTTON_R,
			BUTTON_F,
			BUTTON10,
			BUTTON11,
			BUTTON12,
			BUTTON13,
			BUTTON14,			
			BUTTON15,
			INCREASE_SENS,
			DECREASE_SENS,
			DOMINANT,
			LOCK3D, 
			BUTTON20,
			NONE
		}

		protected DrawArgs drawArgs;

		protected Microsoft.DirectX.DirectInput.Device spacePilot;
		protected Thread joyThread;

		// constants
		protected const double ROTATIONFACTOR = 1e-3f;
		protected const double ROTATIONSCALEMIN = .01;
		protected const double ROTATIONSCALEMAX = .1;
		protected const double PANFACTOR = 1e-5f;
		protected const double PANSCALEMIN = .00001;
		protected const double PANSCALEMAX = 1;
		protected const double ZOOMFACTOR = 1.0;
		protected const int AXISRANGE = 100;
		protected const int THRESHOLD = 128;
		protected const int BUTTONDELAY = 10;
		protected const int MAXSENS = 10;
		protected const int MINSENS = 1;

		protected int m_increaseSensCount = 0;
		protected int m_decreaseSensCount = 0;
		protected int m_dominantCount = 0;
		protected int m_lock3DCount = 0;

		protected int m_increaseSensTick = 0;
		protected int m_decreaseSensTick = 0;

		protected int m_incTiltCount = 0;
		protected int m_incTiltTick = 0;

		protected int m_decTiltCount = 0;
		protected int m_decTiltTick = 0;

		protected bool m_spacePilot = false;
		protected bool m_lock3D = true;

		protected int m_sensitivity = 5;

		/// <summary>
		/// Plugin entry point 
		/// </summary>
		public override void Load() 
		{
			drawArgs = ParentApplication.WorldWindow.DrawArgs;

			// Find the first space pilot or traveler device
			DeviceList dl = Microsoft.DirectX.DirectInput.Manager.GetDevices(DeviceClass.GameControl, EnumDevicesFlags.AttachedOnly);
			dl.MoveNext();
			while(dl.Current != null)
			{
				if ( (((DeviceInstance)dl.Current).ProductName != "SpacePilot") &&
					(((DeviceInstance)dl.Current).ProductName != "SpaceTraveler USB") )
					dl.MoveNext();
				else
					break;
			}
			if(dl.Current==null)
			{
				throw new ApplicationException("No SpacePilot detected.  Please check your connections and verify your device appears in Control panel -> Game Controllers.");
			}
			DeviceInstance di = (DeviceInstance) dl.Current;
			spacePilot = new Microsoft.DirectX.DirectInput.Device( di.InstanceGuid );
			spacePilot.SetDataFormat(DeviceDataFormat.Joystick);
			spacePilot.SetCooperativeLevel(ParentApplication,  
				CooperativeLevelFlags.NonExclusive | CooperativeLevelFlags.Background);


			if (di.ProductName == "SpacePilot")
				m_spacePilot = true;

			foreach(DeviceObjectInstance d in spacePilot.Objects) 
			{
				// For axes that are returned, set the DIPROP_RANGE property for the
				// enumerated axis in order to scale min/max values.
				if((d.ObjectId & (int)DeviceObjectTypeFlags.Axis)!=0) 
				{
					// Try to set the AXISRANGE for the axis but this seems ignored by space pilot
					spacePilot.Properties.SetRange(ParameterHow.ById, d.ObjectId, new InputRange(-AXISRANGE, AXISRANGE));
					spacePilot.Properties.SetDeadZone(ParameterHow.ById, d.ObjectId, 1000); // 10%

					// change the axis mode to absolute to work more like how its expected
					spacePilot.Properties.AxisModeAbsolute = false;
				}
			}

			spacePilot.Acquire();

			// Start a new thread to poll the SpacePilot
			// TODO: The Device supports events, use them
			joyThread = new Thread( new ThreadStart(SpacePilotLoop) );
			joyThread.IsBackground = true;
			joyThread.Start();
		}

		/// <summary>
		/// Unloads our plugin
		/// </summary>
		public override void Unload() 
		{
			if(joyThread != null && joyThread.IsAlive)
				joyThread.Abort();
			joyThread = null;
		}


		/// <summary>
		/// Background thread runs this function in a loop reading SpacePilot state.
		/// </summary>
		void SpacePilotLoop()
		{
			while( true )
			{
				Thread.Sleep(20);
				try 
				{
					// Poll the device for info.
					spacePilot.Poll();
					HandleSpacePilot();
				}
				catch(InputException inputex) 
				{
					if((inputex is NotAcquiredException) || (inputex is InputLostException)) 
					{
						// Check to see if either the app
						// needs to acquire the device, or
						// if the app lost the device to another
						// process.
						try 
						{
							// Acquire the device.
							spacePilot.Acquire();
						}
						catch(InputException) 
						{
							// Failed to acquire the device.
							// This could be because the app
							// doesn't have focus.
							Thread.Sleep(1000);
						}
					}
				}
			}
		}

		/// <summary>
		/// Time to update things again.
		/// </summary>
		void HandleSpacePilot()
		{
			// Get the state of the device.
			JoystickState jss = spacePilot.CurrentJoystickState;
			byte[] button = jss.GetButtons();

			double x = 0;
			double y = 0;
			double z = 0;
			double Rx = 0;
			double Ry = 0;
			double Rz = 0;

			// damp out THRESHOLD values

			if (jss.X > THRESHOLD)
				x = jss.X - THRESHOLD;
			else if (jss.X < -THRESHOLD)
				x = jss.X + THRESHOLD;

			if (jss.Y > THRESHOLD)
				y = -jss.Y + THRESHOLD;
			else if (jss.Y < -THRESHOLD)
				y = -jss.Y - THRESHOLD;

			if (jss.Z > THRESHOLD)
				z = jss.Z - THRESHOLD;
			else if (jss.Z < -THRESHOLD)
				z = jss.Z + THRESHOLD;

			if (jss.Rx > THRESHOLD)
				Rx = jss.Rx - THRESHOLD;
			else if (jss.Rx < -THRESHOLD)
				Rx = jss.Rx + THRESHOLD;

			if (jss.Ry > THRESHOLD)
				Ry = jss.Ry - THRESHOLD;
			else if (jss.Ry < -THRESHOLD)
				Ry = jss.Ry + THRESHOLD;

			if (jss.Rz > THRESHOLD)
				Rz = jss.Rz - THRESHOLD;
			else if (jss.Rz < -THRESHOLD)
				Rz = jss.Rz + THRESHOLD;

			//m_debugWidget.Text = x + ", " + y + ", " + z + ", " + Rx + ", " + Ry + ", " + Rz + ", " + m_sensitivity;

			// handle space pilot buttons
			if (m_spacePilot)
			{
				// 3D lock pressed - enables/disables bank/tilt rather than rotation
				if (button[(int) ButtonType.LOCK3D] != 0)
				{
					if (m_lock3DCount == 0)
						m_lock3D = !m_lock3D;

					m_lock3DCount++;
				}
				else
					m_lock3DCount = 0;

				// Dom Axis pressed
				if (button[(int) ButtonType.DOMINANT] != 0)
				{
				}

				// - sensitivity
				if (button[(int) ButtonType.DECREASE_SENS] != 0)
				{
					if ((m_decreaseSensCount == 0) || (m_decreaseSensCount > BUTTONDELAY - m_decreaseSensTick))
					{
						if (m_sensitivity > MINSENS)
							m_sensitivity--;

						m_decreaseSensCount = 0;
						m_decreaseSensTick++;
					}

					m_decreaseSensCount++;
				}
				else
				{
					m_decreaseSensCount = 0;
					m_decreaseSensTick = 0;
				}
			
				// + sensitivity
				if (button[(int) ButtonType.INCREASE_SENS] != 0)
				{
					if ((m_increaseSensCount == 0) || (m_increaseSensCount > BUTTONDELAY - m_increaseSensTick))
					{
						if (m_sensitivity < MAXSENS)
							m_sensitivity++;

						m_increaseSensCount = 0;
						m_increaseSensTick++;
					}

					m_increaseSensCount++;
				}
				else
				{
					m_increaseSensCount = 0;
					m_increaseSensTick = 0;
				}

				// + tilt
				if (button[(int) ButtonType.BUTTON_F] != 0)
				{
					if (m_incTiltTick < 1000)
						m_incTiltTick+=10;

					drawArgs.WorldCamera.Tilt += Angle.FromRadians( (double)(m_incTiltTick+10)*ROTATIONFACTOR*((double)m_sensitivity/10) );	
				}
				else
				{
					m_incTiltTick = 0;
				}

				// - tilt
				if (button[(int) ButtonType.BUTTON_T] != 0)
				{
					if (m_decTiltTick < 1000)
						m_decTiltTick+=20;

					drawArgs.WorldCamera.Tilt -= Angle.FromRadians( (double)(m_decTiltTick)*ROTATIONFACTOR*((double)m_sensitivity/10) );	

				}
				else
				{
					m_decTiltTick = 0;
				}
			}

			// if we are rotating or panning
			if ( (x!=0) || (y!=0) || (Rz !=0) )
			{
				// if rotating
				if (Rz != 0)
				{
					double rotationScaling = drawArgs.WorldCamera.ViewRange.Radians;
					if (rotationScaling > ROTATIONSCALEMAX) rotationScaling = ROTATIONSCALEMAX;
					else if (rotationScaling < ROTATIONSCALEMIN) rotationScaling = ROTATIONSCALEMIN;

					Rz = Rz * rotationScaling * m_sensitivity * ROTATIONFACTOR;
				}

				// if panning
				if ((x != 0) || (y != 0))
				{
					double panScaling = drawArgs.WorldCamera.ViewRange.Radians;

					if (panScaling > PANSCALEMAX) panScaling = PANSCALEMAX;
					else if (panScaling < PANSCALEMIN) panScaling = PANSCALEMIN;

					x = x * panScaling * m_sensitivity * PANFACTOR;
					y = y * panScaling * m_sensitivity * PANFACTOR;
				}

				// change the camera
				drawArgs.WorldCamera.RotationYawPitchRoll(
					Angle.FromRadians(x), // yaw 
					Angle.FromRadians(y), // pitch
					Angle.FromRadians(Rz) );  // roll
			}

			// if we are zooming
			if (z != 0)
			{
				double altitudeDelta = z/1000 * (ZOOMFACTOR + m_sensitivity/20) * drawArgs.WorldCamera.Altitude;
				drawArgs.WorldCamera.Altitude -= altitudeDelta;
			}

			// if we are in 3D mode try to bank/tilt
			if (!m_lock3D)
			{
				// Bank
				drawArgs.WorldCamera.Bank -= Angle.FromRadians( Ry*ROTATIONFACTOR*((double)m_sensitivity/10.0) );

				// Tilt
				drawArgs.WorldCamera.Tilt += Angle.FromRadians( Rx*ROTATIONFACTOR*((double)m_sensitivity/10.0) );	
			}
		}
	}
}
