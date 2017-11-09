//========================= (UNCLASSIFIED) ==============================
// Copyright ?2005-2007 The Johns Hopkins University /
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
// Physics Laboratory (“JHU/APL? that is the author thereof under the 
// “work made for hire?provisions of the copyright law.  Permission is 
// hereby granted, free of charge, to any person obtaining a copy of this 
// software and associated documentation (the “Software?, to use the 
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

using MFW3D;
using MFW3D.Renderable;

using MFW3D.NewWidgets;
using Collab.jhuapl.Util;

namespace Collab.jhuapl.Whiteboard
{
	/// <summary>
	/// This form widget contains all the tools needed for whiteboard 
	/// operations.
	/// </summary>
	public class WhiteboardWidget : FormWidget
	{
        /// <summary>
        /// The basepath for all icons/images
        /// </summary>
        public string BasePath
        {
            get { return m_basePath; }
            set { m_basePath = value; }
        }
        protected string m_basePath;

        /// <summary>
        /// The whiteboard drawing layer
        /// </summary>
		public DrawLayer WhiteboardLayer
		{
			get { return m_whiteboardLayer; }
			set { m_whiteboardLayer = value; }
		}
        protected DrawLayer m_whiteboardLayer;

		/// <summary>
		/// Top level treenode for the whiteboard widget
		/// </summary>
		public TreeNodeWidget RootWidget
		{
			get { return m_rootWidget; }
			set { m_rootWidget = value; }
		}
		protected TreeNodeWidget m_rootWidget;

		/// <summary>
		/// The treenode that holds all the drawing tools
		/// </summary>
		public TreeNodeWidget DrawWidget
		{
			get { return m_drawWidget; }
			set { m_drawWidget = value; }
		}
		protected TreeNodeWidget m_drawWidget;

		/// <summary>
		/// This widget enables draw lock.  Once draw lock is enabled every
		/// mouse event triggers whatever drawing mode was last used.
		/// </summary>
		public TreeNodeWidget LockWidget
		{
			get { return m_lockWidget; }
			set { m_lockWidget = value; }
		}
		protected TreeNodeWidget m_lockWidget;

		public TreeNodeWidget ShapeWidget
		{
			get { return m_shapeWidget; }
			set { m_shapeWidget = value; }
		}
		protected TreeNodeWidget m_shapeWidget;

		public PanelWidget ShapePalette
		{
			get { return m_shapePalette; }
			set { m_shapePalette = value; }
		}
		protected PanelWidget m_shapePalette;

		public TreeNodeWidget ColorWidget
		{
			get { return m_colorWidget; }
			set { m_colorWidget = value; }
		}
		protected TreeNodeWidget m_colorWidget;

		public PanelWidget ColorPalette
		{
			get { return m_colorPalette; }
			set { m_colorPalette = value; }
		}
		protected PanelWidget m_colorPalette;

		public TreeNodeWidget IconWidget
		{
			get { return m_iconWidget; }
			set { m_iconWidget = value; }
		}
		protected TreeNodeWidget m_iconWidget;

		public PanelWidget IconPalette
		{
			get { return m_iconPalette; }
			set { m_iconPalette = value; }
		}
		protected PanelWidget m_iconPalette;

		protected ButtonWidget m_lockButton;
		protected ButtonWidget m_paletteButton;

		protected ButtonWidget m_hotspotButton;
		protected ButtonWidget m_postitButton;

		protected ButtonWidget m_polygonButton;
		protected ButtonWidget m_polylineButton;

		protected ButtonWidget m_freehandButton;

		public WhiteboardWidget(string name, string basePath) : base(name)
		{
            m_basePath = basePath;

			// Probably should put this in a try catch block or move out of constructor
			BuildForm();
		}

		/// <summary>
		/// Adds all out subwidgets
		/// </summary>
		protected void BuildForm()
		{
			m_rootWidget = new SimpleTreeNodeWidget();
			m_rootWidget.Name = "CollabSpace";
			m_rootWidget.IsRadioButton = true;
			m_rootWidget.Enabled = true;
			m_rootWidget.EnableCheck = false;
			m_rootWidget.IsChecked = false;
			m_rootWidget.Expanded = true;

			// Settings for sharing whiteboard info
//			m_sharingWidget = new SimpleTreeNodeWidget();
//			m_sharingWidget.Name = "Settings";
//			m_sharingWidget.IsRadioButton = true;
//			m_sharingWidget.Enabled = true;
//			m_sharingWidget.EnableCheck = false;
//			m_sharingWidget.IsChecked = true;
//			Add(m_sharingWidget);
//
//			m_cameraWidget = new SimpleTreeNodeWidget();
//			m_cameraWidget.Name = "Allow Camera Sharing";
//			m_cameraWidget.Enabled = true;
//			m_cameraWidget.EnableCheck = false;
//			m_cameraWidget.IsChecked = false;
//			m_sharingWidget.Add(m_cameraWidget);
//
//			m_whiteboardWidget = new SimpleTreeNodeWidget();
//			m_whiteboardWidget.Name = "Allow Whiteboard Sharing";
//			m_whiteboardWidget.Enabled = true;
//			m_whiteboardWidget.EnableCheck = false;
//			m_whiteboardWidget.IsChecked = false;
//			m_sharingWidget.Add(m_whiteboardWidget);

			// Drawing tool widgets
			m_drawWidget = new SimpleTreeNodeWidget();
			m_drawWidget.Name = "Drawing Tools";
			m_drawWidget.IsRadioButton = true;
			m_drawWidget.Enabled = true;
			m_drawWidget.EnableCheck = false;
			m_drawWidget.IsChecked = false;
			m_drawWidget.Expanded = true;
			m_rootWidget.Add(m_drawWidget);

			m_shapeWidget = new SimpleTreeNodeWidget();
			m_shapeWidget.Name = "Shape Palette";
			m_shapeWidget.IsRadioButton = true;
			m_shapeWidget.Enabled = true;
			m_shapeWidget.EnableCheck = false;
			m_shapeWidget.IsChecked = false;
			m_shapeWidget.Expanded = true;
			m_drawWidget.Add(m_shapeWidget);

			m_shapePalette = new PanelWidget("Shape Box");
			m_shapePalette.Location = new System.Drawing.Point(0,0);
			m_shapePalette.WidgetSize = new System.Drawing.Size(104, 64);
			m_shapePalette.HeaderEnabled = false;
			m_shapeWidget.Add(m_shapePalette);

			m_lockButton = new ButtonWidget();
			m_lockButton.Location = new System.Drawing.Point(4,4);
			m_lockButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_open.png";
			m_lockButton.WidgetSize = new System.Drawing.Size(16,16);
			m_lockButton.CountHeight = true;
			m_lockButton.CountWidth = true;
			m_lockButton.LeftClickAction = new MFW3D.NewWidgets.MouseClickAction(this.PerformLock);

			m_shapePalette.Add(m_lockButton);

			m_paletteButton = new ButtonWidget();
			m_paletteButton.Location = new System.Drawing.Point(24,4);
            m_paletteButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\palette.png";
			m_paletteButton.WidgetSize = new System.Drawing.Size(16,16);
			m_paletteButton.CountHeight = true;
			m_paletteButton.CountWidth = true;
			// m_paletteButton.LeftClickAction = new MouseClickAction(this.PerformColorPalette);

			// m_shapePalette.Add(m_paletteButton);

			m_hotspotButton = new ButtonWidget();
			m_hotspotButton.Location = new System.Drawing.Point(4,44);
            m_hotspotButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\flag_red.png";
			m_hotspotButton.WidgetSize = new System.Drawing.Size(16,16);
			m_hotspotButton.CountHeight = true;
			m_hotspotButton.CountWidth = true;
			m_hotspotButton.LeftClickAction = new MFW3D.NewWidgets.MouseClickAction(this.PerformHotspot);

			m_shapePalette.Add(m_hotspotButton);

			m_postitButton = new ButtonWidget();
			m_postitButton.Location = new System.Drawing.Point(24,44);
            m_postitButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\comment.png";
			m_postitButton.WidgetSize = new System.Drawing.Size(16,16);
			m_postitButton.CountHeight = true;
			m_postitButton.CountWidth = true;
			// m_postitButton.LeftClickAction = new MouseClickAction(this.PerformColorPalette);

			// m_shapePalette.Add(m_postitButton);

			m_polygonButton = new ButtonWidget();
			m_polygonButton.Location = new System.Drawing.Point(44,44);
            m_polygonButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\shape_handles.png";
			m_polygonButton.WidgetSize = new System.Drawing.Size(16,16);
			m_polygonButton.CountHeight = true;
			m_polygonButton.CountWidth = true;
			m_polygonButton.LeftClickAction = new MFW3D.NewWidgets.MouseClickAction(this.PerformPolygon);

			m_shapePalette.Add(m_polygonButton);

			m_polylineButton = new ButtonWidget();
			m_polylineButton.Location = new System.Drawing.Point(64,44);
            m_polylineButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\chart_line.png";
			m_polylineButton.WidgetSize = new System.Drawing.Size(16,16);
			m_polylineButton.CountHeight = true;
			m_polylineButton.CountWidth = true;
			m_polylineButton.LeftClickAction = new MFW3D.NewWidgets.MouseClickAction(this.PerformPolyline);

			m_shapePalette.Add(m_polylineButton);

			m_freehandButton = new ButtonWidget();
			m_freehandButton.Location = new System.Drawing.Point(84,44);
            m_freehandButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\pencil.png";
			m_freehandButton.WidgetSize = new System.Drawing.Size(16,16);
			m_freehandButton.CountHeight = true;
			m_freehandButton.CountWidth = true;
			m_freehandButton.LeftClickAction = new MFW3D.NewWidgets.MouseClickAction(this.PerformFreehand);

			m_shapePalette.Add(m_freehandButton);


			m_colorWidget = new SimpleTreeNodeWidget();
			m_colorWidget.Name = "Color Palette";
			m_colorWidget.IsRadioButton = true;
			m_colorWidget.Enabled = false;
			m_colorWidget.EnableCheck = false;
			m_colorWidget.IsChecked = false;
			m_drawWidget.Add(m_colorWidget);

			m_colorPalette = new PanelWidget("Color Box");
			m_colorPalette.Location = new System.Drawing.Point(0,0);
			m_colorPalette.WidgetSize = new System.Drawing.Size(125, 80);
			m_colorPalette.HeaderEnabled = false;
			m_colorWidget.Add(m_colorPalette);

			m_iconWidget = new SimpleTreeNodeWidget();
			m_iconWidget.Name = "Icon Palette";
			m_iconWidget.IsRadioButton = true;
			m_iconWidget.Enabled = true;
			m_iconWidget.EnableCheck = false;
			m_iconWidget.IsChecked = false;
			m_drawWidget.Add(m_iconWidget);

			//m_iconWidget.Add(m_iconPalette);

			Add(m_rootWidget);
		}

		#region button actions

		public void PerformLock(System.Windows.Forms.MouseEventArgs e)
		{
			if (m_whiteboardLayer != null)
			{
				m_whiteboardLayer.DrawLock = !m_whiteboardLayer.DrawLock;
				if (m_whiteboardLayer.DrawLock)
					m_lockButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_edit.png";
				else
                    m_lockButton.ImageName = m_basePath + @"\Plugins\Whiteboard\" + @"\images\icons\lock_open.png";
			}

			Logger.Write(1, "DRAW", "", "Lock Button Pressed");
		}

		public void PerformHotspot(System.Windows.Forms.MouseEventArgs e)
		{
			if (m_whiteboardLayer != null)
			{
				ResetDrawingIcons();
				m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Hotspot;
			}

			Logger.Write(1, "DRAW", "", "Hotspot Button Pressed");
		}

		public void PerformPolygon(System.Windows.Forms.MouseEventArgs e)
		{
			if (m_whiteboardLayer != null)
			{
				ResetDrawingIcons();
				m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Polygon;
			}

			Logger.Write(1, "DRAW", "", "Polygon Button Pressed");
		}

		public void PerformPolyline(System.Windows.Forms.MouseEventArgs e)
		{
			if (m_whiteboardLayer != null)
			{
				ResetDrawingIcons();
				m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Polyline;
			}

			Logger.Write(1, "DRAW", "", "Polyline Button Pressed");
		}

		public void PerformFreehand(System.Windows.Forms.MouseEventArgs e)
		{
			if (m_whiteboardLayer != null)
			{
				ResetDrawingIcons();
				m_whiteboardLayer.DrawingMode = DrawLayer.DrawMode.Freehand;
			}

			Logger.Write(1, "DRAW", "", "Freehand Button Pressed");
		}

		public void ResetDrawingIcons()
		{
		}

		#endregion
	}
}
