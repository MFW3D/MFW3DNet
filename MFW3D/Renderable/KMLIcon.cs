//
// Copyright ?2005 NASA.  Available under the NOSA License
//
// Portions copied from Icon - Copyright ?2005-2006 The Johns Hopkins University 
// Applied Physics Laboratory.  Available under the JHU/APL Open Source Agreement.
//
using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Net;

using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Text;
using MFW3D.NewWidgets;

namespace MFW3D.Renderable
{
	/// <summary>
	/// One icon in an icon layer
	/// </summary>
	public class KMLIcon : Icon
    {
        public new string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                isSelectable = value != null;
                m_description = value;
            }
        }

        public string NormalIcon;

        public bool HasBeenUpdated = true;

        public bool IsDescriptionVisible = false;

        public KMLDialog DescriptionBubble;

        /// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.Icon"/> class  
        /// </summary>
		/// <param name="name">Name of the icon</param>
		/// <param name="latitude">Latitude in decimal degrees.</param>
		/// <param name="longitude">Longitude in decimal degrees.</param>
        /// <param name="normalicon"></param>
        /// <param name="heightAboveSurface">Altitude</param>
        public KMLIcon(string name, double latitude, double longitude, string normalicon, double heightAboveSurface)
            : base(name, latitude, longitude, heightAboveSurface)
		{
			NormalIcon = normalicon;
            AutoScaleIcon = true;
            Declutter = true;
            IsAGL = false;
		}

		#region Overrriden methods

        /// <summary>
        /// Disposes the icon (when disabled)
        /// </summary>
        public override void Dispose()
        {
            // Nothing to dispose
            // Ashish Datta - make sure the description bubble is destroyed and not visisble.
            try
            {
                if (this.DescriptionBubble != null)
                    this.DescriptionBubble.Dispose();
            }
            finally
            {
                base.Dispose();
            }
        }

        protected override bool PerformLMBAction(DrawArgs drawArgs)
        {
            if (DescriptionBubble != null)
            {
                IsDescriptionVisible = false;
                DescriptionBubble.isVisible = false;
                DescriptionBubble.Dispose();
            }

            DescriptionBubble = new KMLDialog();
            DescriptionBubble.Owner = (Form)drawArgs.parentControl.Parent.Parent.Parent;

            if (IsDescriptionVisible == false)
            {
                IsDescriptionVisible = true;
            }
            else
            {
                DescriptionBubble.Dispose();
                IsDescriptionVisible = false;
            }

            return true;
        }

        protected override void RenderDescription(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color)
        {
            // Ashish Datta - Show/Hide the description bubble.
            if (IsDescriptionVisible)
            {
                if (DescriptionBubble.isVisible == true)
                {
                    DescriptionBubble.Location = new Point((int)projectedPoint.X + (Width / 4), (int)projectedPoint.Y);
                    DescriptionBubble.Show();

                    if (DescriptionBubble.HTMLIsSet == false)
                        DescriptionBubble.SetHTML(Description + "<br>URL: <A HREF=" + ClickableActionURL + ">" + ClickableActionURL + "</A>");

                    DescriptionBubble.BringToFront();
                }
            }
        }

        public override void NoRender(DrawArgs drawArgs)
        {
            base.NoRender(drawArgs);

            if (IsDescriptionVisible)
            {
                DescriptionBubble.Hide();
                IsDescriptionVisible = false;
            }
        }

		#endregion
	}
}
