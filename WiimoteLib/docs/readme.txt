v1.2.0.0 Modified
by Nigel Tzeng

I have added Johnny Lee's modifications to work with the 1.1 library and
his VR and headtracking code.

Managed Library for Nintendo's Wiimote
v1.2.0.0
by Brian Peek (http://www.brianpeek.com/)

For more information, please visit the associated article for this project at:

http://msdn.microsoft.com/coding4fun/hardware/article.aspx?articleid=1879033

There you will find documentation on how all of this works.

If all else fails, please contact me at the address above.  Enjoy!

Changes
=======

v1.2.0.0
--------
	o Moved to CodePlex! (http://www.codeplex.com/WiimoteLib)
	o New license!  Please read the included license.txt/copyright.txt for more
	  info.  This likely doesn't change anything for anyone, but at least now
	  it's official.
	o AltWriteMethod deprecated.  Connect will now determine which write method
	  to use at runtime.  It remains in case someone needs to override the
	  write method for some reason. (gl.tter)
	o WiimoteState.LEDState is now filled with proper values.
	  (identified by gl.tter/Leif902)
	o Extensions that are attached at startup are now recognized properly.
	  (identified by Will Pressly)
	o "Partially inserted" extensions now handled properly (Michael Dorman)
	o SetRumble method now does this via the SetLEDs method instead of using the
	  status report to avoid a needless response from the Wiimote. (Michael Dorman)
	o IRState now contains RawMidX/Y and MidX/Y containing the value of the
	  midpoint between the IR points.
	o Async reads now begin after the data parsing and event has been raised.
	  This should lead to non-overlapping events.
	o Updated the test application with the above changes and cleaned up the UI
	  updates by using delegates a bit more effeciently.

	Breaking Changes (may not be a complete list)
	----------------------------------------------
	o LEDs renamed to LEDState
	o GetBatteryLevel renamed to GetStatus
	o OnWiimoteChanged renamed to WiimoteChanged
	o OnWiimoteExtensionChanged renamed to WiimoteExtensionChanged
	o CalibrationInfo renamed to AccelCalibrationInfo
	o Event handlers renamed to WiimoteChangedEventHandler and WiimoteExtensionChangedEventHandler

v1.1.0.0
--------
	o Support for XP and Vista x64 (Paul Miller)
	o VB fix in ParseExtension (Evan Merz)
	o New "AltWriteMethod" property which will try a secondary approach to writing
	  to the Wiimote.  If you get an error when connecting, set this property and
	  try again to see if it fixes the issue.
	o Microsoft Robotics Studio project
	  Open the WiimoteMSRS directory and start the Wiimote.sln solution to take a
	  look! (David Lee)

v1.0.1.0
--------
	o Calibration copy/paste error (James Darpinian)