using System;
using System.Drawing;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

using WorldWind;
using WorldWind.Renderable;

namespace Collab.jhuapl.Util
{
	/// <summary>
	/// This is the classification banner for CollabSpace.
	/// </summary>
	public class ClassificationBanner : RenderableObject
	{
		public enum ClassificationLevel
		{
			UNCLASS,
			CONFIDENTIAL,
			SECRET,
			TOPSECRET
		}

		public string[] ClassificationString =
		{
			"UNCLASSIFIED",
			"CONFIDENTIAL",
			"SECRET",
			"TOP SECRET"
		};

		public int[] ClassificationColor = 
		{
			Color.PaleGreen.ToArgb(),
			Color.Turquoise.ToArgb(),
			Color.Red.ToArgb(),
			Color.Orange.ToArgb()
		};

		#region private members
		
		private const int distanceFromRight = 65;
		private const int distanceFromBottom = 5;

		#endregion

		#region public members

		public ClassificationLevel Classification;

		#endregion

		/// <summary>
		/// Default Constructor
		/// </summary>
        public ClassificationBanner()
            : base("Banner", Vector3.Empty, Quaternion.Identity)
		{
			// draw with icons (on top)
			this.RenderPriority = RenderPriority.Icons;

			// enable this layer
			this.IsOn = true;

			// start as unclass
			Classification = ClassificationLevel.UNCLASS;
		}

		#region RenderableObject methods

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Dispose()
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed) 
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Initialize(DrawArgs drawArgs)
		{
		}

		/// <summary>
		/// RenderableObject abstract member (needed)
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override bool PerformSelectionAction(DrawArgs drawArgs)
		{
			return false;
		}

		/// <summary>
		/// This is where we do our rendering 
		/// Called from UI thread = UI code safe in this function
		/// </summary>
		public override void Render(DrawArgs drawArgs)
		{
			// Draw the current time using default font in lower right corner
			string text = ClassificationString[(int) Classification] + " - " + DateTime.Now.ToString();
			Rectangle bounds = drawArgs.defaultDrawingFont.MeasureString(null, text, DrawTextFormat.None, 0);
			drawArgs.defaultDrawingFont.DrawText(null, text, 
				(drawArgs.screenWidth-bounds.Width)/2, drawArgs.screenHeight-bounds.Height-distanceFromBottom,
				ClassificationColor[(int) Classification] );
		}
		
		/// <summary>
		/// RenderableObject abstract member (needed)
		/// OBS: Worker thread (don't update UI directly from this thread)
		/// </summary>
		public override void Update(DrawArgs drawArgs)
		{
		}

		#endregion
	}
}
