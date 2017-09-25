using System;
using System.Collections;
using System.Text;
using System.Drawing;
using WorldWind;
using WorldWind.PluginEngine;
using WorldWind.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OgrVectorImporter
{
        /// <summary>
        /// Renders a message to the lower right corner (stolen from kmlimporter)
        /// </summary>
        class WaitMessage : RenderableObject
        {
            #region Private members
            private string _Text = "";
            private string leadString = "Vector Importer:\n";
            private int color = Color.White.ToArgb();
            private int distanceFromCorner = 25;
            #endregion

            /// <summary>
            /// Creates a new WaitMessage
            /// </summary>
            internal WaitMessage()
                : base("Status message", Vector3.Empty, Quaternion.Identity)
            {
                // We want to be drawn on top of everything else
                this.RenderPriority = RenderPriority.Icons;

                // true to make this layer active on startup, this is equal to the checked state in layer manager
                this.IsOn = true;
            }

            /// <summary>
            /// Gets/sets the message to be displayed
            /// </summary>
            public string Text
            {
                get { return _Text; }
                set { _Text = value; }
            }

            #region RenderableObject methods
            /// <summary>
            /// This is where we do our rendering 
            /// Called from UI thread = UI code safe in this function
            /// </summary>
            public override void Render(DrawArgs drawArgs)
            {
                // Draw the current text using default font in lower right corner
                Rectangle bounds = drawArgs.defaultDrawingFont.MeasureString(null, leadString + _Text, DrawTextFormat.None, 0);
                drawArgs.defaultDrawingFont.DrawText(null, leadString + _Text,
                    drawArgs.screenWidth - bounds.Width - distanceFromCorner, drawArgs.screenHeight - bounds.Height - distanceFromCorner,
                    color);
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
            /// OBS: Worker thread (don't update UI directly from this thread)
            /// </summary>
            public override void Update(DrawArgs drawArgs)
            {
            }

            /// <summary>
            /// RenderableObject abstract member (needed)
            /// OBS: Worker thread (don't update UI directly from this thread)
            /// </summary>
            public override void Dispose()
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
            #endregion
        }
}
