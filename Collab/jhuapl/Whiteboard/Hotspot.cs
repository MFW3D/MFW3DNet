//========================= (UNCLASSIFIED) ==============================
// Copyright © 2007 The Johns Hopkins University /
// Applied Physics Laboratory.  All rights reserved.
//
// WorldWind Source Code - Copyright 2005 NASA World Wind 
// Modified under the NOSA License
//
//========================= (UNCLASSIFIED) ==============================
//
// LICENSE AND DISCLAIMER 
//
// Copyright (c) 2007 The Johns Hopkins University. 
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
using System.IO;
using System.Drawing;

using Collab.jhuapl.Util;
using WorldWind.Renderable;
using WorldWind;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX;
using System.Collections.Generic;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// A hotspot is a point of interest placed on the whiteboard
	/// </summary>
	public class Hotspot : WorldWind.Renderable.Icon
	{
		Color m_color;

		public Hotspot(
			string id,
			double lat,
			double lon,
			double alt,
			Color color,
			string url,
			string description) : base(id, lat, lon, alt)
		{
			m_id = id;
			m_color = color;
            ClickableActionURL = url;
			this.Description = description;
            isSelectable = true;
            AutoScaleIcon = true;

			Width = 24;
			Height = 24;

            Image = CreateBitmap(color);
		}

        /// <summary>
        /// Helper function to render icon label.  Broken out so that child classes can override this behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="sprite"></param>
        /// <param name="projectedPoint"></param>
        /// <param name="color"></param>
        /// <param name="isMouseOver">Whether we should render as a mouseover icon</param>
        protected override void RenderLabel(DrawArgs drawArgs, Sprite sprite, Vector3 projectedPoint, int color, List<Rectangle> labelRectangles, bool isMouseOver)
        {
            if ((this.Name != null) && ((IconTexture == null) || isMouseOver || NameAlwaysVisible))
            {
                String labelText = this.Name + "\n\n" + this.Description;

                if (this.IconTexture == null)
                {
                    // Original Icon Label Render code

                    // KML Label Render Code with Declutter

                    // Center over target as we have no bitmap
                    Rectangle realrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, labelText, DrawTextFormat.WordBreak, color);
                    realrect.X = (int)projectedPoint.X - (realrect.Width >> 1);
                    realrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDraw = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (Declutter && !NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (realrect.IntersectsWith(drawnrect))
                            {
                                bDraw = false;
                                break;
                            }
                        }
                    }

                    if (bDraw)
                    {
                        labelRectangles.Add(realrect);

                        drawArgs.defaultDrawingFont.DrawText(m_sprite, labelText, realrect, DrawTextFormat.WordBreak, color);
                    }
                }
                else
                {
                    // KML Label Render Code with Declutter

                    // Adjust text to make room for icon
                    int spacing = (int)(Width * 0.3f);
                    if (spacing > 5)
                        spacing = 5;
                    int offsetForIcon = (Width >> 1) + spacing;

                    // Text to the right
                    Rectangle rightrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, labelText, DrawTextFormat.WordBreak, color);
                    rightrect.X = (int)projectedPoint.X + offsetForIcon;
                    rightrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    // Text to the left
                    Rectangle leftrect = drawArgs.defaultDrawingFont.MeasureString(m_sprite, labelText, DrawTextFormat.WordBreak, color);
                    leftrect.X = (int)projectedPoint.X - offsetForIcon - rightrect.Width;
                    leftrect.Y = (int)(projectedPoint.Y - (drawArgs.defaultDrawingFont.Description.Height >> 1));

                    bool bDrawRight = true;
                    bool bDrawLeft = true;

                    // Only not show if declutter is turned on and we aren't always supposed to be seen
                    if (Declutter && !NameAlwaysVisible)
                    {
                        foreach (Rectangle drawnrect in labelRectangles)
                        {
                            if (rightrect.IntersectsWith(drawnrect))
                            {
                                bDrawRight = false;
                            }
                            if (leftrect.IntersectsWith(drawnrect))
                            {
                                bDrawLeft = false;
                            }
                            if (!bDrawRight && !bDrawLeft)
                            {
                                break;
                            }
                        }
                    }

                    // draw either right or left if we have space.  If we don't too bad.
                    if (bDrawRight)
                    {
                        labelRectangles.Add(rightrect);
                        drawArgs.defaultDrawingFont.DrawText(m_sprite, labelText, rightrect, DrawTextFormat.WordBreak, color);
                    }
                    else if (bDrawLeft)
                    {
                        labelRectangles.Add(leftrect);
                        drawArgs.defaultDrawingFont.DrawText(m_sprite, labelText, leftrect, DrawTextFormat.WordBreak, color);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a small 32x32 bitmap with circle of the specified color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        private Bitmap CreateBitmap(Color color)
        {
            int bitmapSize = 24;

            SolidBrush backBrush = new SolidBrush(color);
            Bitmap bitmap = new Bitmap(bitmapSize, bitmapSize);

            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphics.FillEllipse(backBrush, 0, 0, bitmapSize, bitmapSize);

            graphics.Flush(System.Drawing.Drawing2D.FlushIntention.Flush);

            return bitmap;
        }

		public override string ToString()
		{
			// build long description from values
			string retString = "Hotspot:" +
				"\nName: " + m_id + 
				"\nDescription: " + m_description +
                "\nLat: " + Latitude +
                "\nLon: " + Longitude +
                "\nAlt: " + Altitude +
				"\n\n";

			return retString;
		}
	}
}