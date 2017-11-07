using System;

namespace WorldWind.NewWidgets
{
	/// <summary>
	/// Summary description for JHU_Enums.
	/// </summary>
	public class WidgetEnums
	{
		/// <summary>
		/// Widget Anchor Styles.  Same values as Forms AnchorStyles
		/// </summary>
		[Flags]
		public enum AnchorStyles
		{
			None = 0x0000,
			Top = 0x0001,
			Bottom = 0x0002,
			Left = 0x0004,
			Right = 0x0008,
		}

		/// <summary>
		/// Default constructor - does nada
		/// </summary>
		public WidgetEnums()
		{
		}
	}
}
