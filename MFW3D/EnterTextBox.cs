using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace MFW3D
{
	// class to make textboxes that can activate buttons with the enter key
	public class EnterTextBox : TextBox
	{
		protected override bool IsInputKey(Keys key)
		{
			if (key == Keys.Enter)
				return true;
			return base.IsInputKey(key);
		}
	}
}
