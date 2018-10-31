using System;
using System.Collections;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.IO;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml;
using MFW3D.Renderable;
using Utility;

namespace MFW3D.Menu
{
    public interface IEvent
    {
        void OnKeyUp(KeyEventArgs keyEvent);
        void OnKeyDown(KeyEventArgs keyEvent);
        bool OnMouseUp(MouseEventArgs e);
        bool OnMouseDown(MouseEventArgs e);
        bool OnMouseMove(MouseEventArgs e);
        bool OnMouseWheel(MouseEventArgs e);
        void Render(DrawArgs drawArgs);
        void Dispose();
    }

    public class MenuCollection : IEvent
    {
        System.Collections.ArrayList _menus = new System.Collections.ArrayList();

        #region IMenu Members

        public void OnKeyUp(KeyEventArgs keyEvent)
        {
            foreach (IEvent m in this._menus)
                m.OnKeyUp(keyEvent);
        }

        public void OnKeyDown(KeyEventArgs keyEvent)
        {
            foreach (IEvent m in this._menus)
                m.OnKeyDown(keyEvent);
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            foreach (IEvent m in this._menus)
            {
                if (m.OnMouseUp(e))
                    return true;
            }
            return false;
        }

        public bool OnMouseDown(MouseEventArgs e)
        {
            foreach (IEvent m in this._menus)
            {
                if (m.OnMouseDown(e))
                    return true;
            }
            return false;
        }

        public bool OnMouseMove(MouseEventArgs e)
        {
            foreach (IEvent m in this._menus)
            {
                if (m.OnMouseMove(e))
                    return true;
            }
            return false;
        }

        public bool OnMouseWheel(MouseEventArgs e)
        {
            foreach (IEvent m in this._menus)
            {
                if (m.OnMouseWheel(e))
                    return true;
            }
            return false;
        }

        public void Render(DrawArgs drawArgs)
        {
            foreach (IEvent m in this._menus)
                m.Render(drawArgs);
        }

        public void Dispose()
        {
            foreach (IEvent m in this._menus)
                m.Dispose();
        }

        #endregion

        public void AddMenu(IEvent menu)
        {
            lock (this._menus.SyncRoot)
            {
                this._menus.Add(menu);
            }
        }

        public void RemoveMenu(IEvent menu)
        {
            lock (this._menus.SyncRoot)
            {
                this._menus.Remove(menu);
            }
        }
    }

    public abstract class SideBarMenu : IEvent
    {
        public long Id;

        public readonly int Left;
        public int Top = 120;
        public int Right = World.Settings.layerManagerWidth;
        public int Bottom;
        public readonly float HeightPercent = 0.9f;
        private Vector2[] outlineVerts = new Vector2[5];

        public int Width
        {
            get { return Right - Left; }
            set { Right = Left + value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = Top + value; }
        }

        #region IMenu Members

        public abstract void OnKeyUp(KeyEventArgs keyEvent);
        public abstract void OnKeyDown(KeyEventArgs keyEvent);
        public abstract bool OnMouseUp(MouseEventArgs e);
        public abstract bool OnMouseDown(MouseEventArgs e);
        public abstract bool OnMouseMove(MouseEventArgs e);
        public abstract bool OnMouseWheel(MouseEventArgs e);
        public void Render(DrawArgs drawArgs)
        {
            this.Top = 120;
            this.Bottom = drawArgs.screenHeight - 1;

            MenuUtils.DrawBox(Left, Top, Right - Left, Bottom - Top, 0.0f,
                World.Settings.menuBackColor, drawArgs.device);

            RenderContents(drawArgs);

            outlineVerts[0].X = Left;
            outlineVerts[0].Y = Top;

            outlineVerts[1].X = Right;
            outlineVerts[1].Y = Top;

            outlineVerts[2].X = Right;
            outlineVerts[2].Y = Bottom;

            outlineVerts[3].X = Left;
            outlineVerts[3].Y = Bottom;

            outlineVerts[4].X = Left;
            outlineVerts[4].Y = Top;

            MenuUtils.DrawLine(outlineVerts, World.Settings.menuOutlineColor, drawArgs.device);
        }

        public abstract void RenderContents(DrawArgs drawArgs);
        public abstract void Dispose();

        #endregion

    }

    public class LayerManagerButton : MenuButton
    {
        World _parentWorld;
        LayerManagerMenu lmm;

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.LayerManagerButton"/> class.
        /// </summary>
        /// <param name="iconImagePath"></param>
        /// <param name="parentWorld"></param>
        public LayerManagerButton(
            string iconImagePath,
            World parentWorld)
            : base(iconImagePath)
        {
            this._parentWorld = parentWorld;
            this.Description = "Layer Manager";
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool IsPushed()
        {
            return World.Settings.showLayerManager;
        }

        public override void Update(DrawArgs drawArgs)
        {
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
        }

        public override void OnKeyUp(KeyEventArgs keyEvent)
        {
        }

        public override bool OnMouseDown(MouseEventArgs e)
        {
            if (IsPushed())
                return this.lmm.OnMouseDown(e);
            else
                return false;
        }

        public override bool OnMouseMove(MouseEventArgs e)
        {
            if (lmm != null && IsPushed())
                return this.lmm.OnMouseMove(e);
            else
                return false;
        }

        public override bool OnMouseUp(MouseEventArgs e)
        {
            if (this.IsPushed())
                return this.lmm.OnMouseUp(e);
            else
                return false;
        }

        public override bool OnMouseWheel(MouseEventArgs e)
        {
            if (this.IsPushed())
                return this.lmm.OnMouseWheel(e);
            else
                return false;
        }

        public override void Render(DrawArgs drawArgs)
        {
            if (IsPushed())
            {
                if (lmm == null)
                    lmm = new LayerManagerMenu(_parentWorld, this);

                lmm.Render(drawArgs);
            }
        }

        public override void SetPushed(bool isPushed)
        {
            World.Settings.showLayerManager = isPushed;
        }
    }

    public class LayerManagerMenu : SideBarMenu
    {
        public int DialogColor = System.Drawing.Color.Gray.ToArgb();
        public int TextColor = System.Drawing.Color.White.ToArgb();
        public LayerMenuItem MouseOverItem;
        public int ScrollBarSize = 20;
        public int ItemHeight = 20;
        World _parentWorld;
        MenuButton _parentButton;
        bool showScrollbar;
        int scrollBarPosition;
        float scrollSmoothPosition; // Current position of scroll when smooth scrolling (scrollBarPosition=target)
        int scrollGrabPositionY; // Location mouse grabbed scroll
        bool isResizing;
        bool isScrolling;
        int leftBorder = 2;
        int rightBorder = 1;
        int topBorder = 25;
        int bottomBorder = 1;
        Microsoft.DirectX.Direct3D.Font headerFont;
        Microsoft.DirectX.Direct3D.Font itemFont;
        Microsoft.DirectX.Direct3D.Font wingdingsFont;
        Microsoft.DirectX.Direct3D.Font worldwinddingsFont;
        ArrayList _itemList = new ArrayList();
        Microsoft.DirectX.Vector2[] scrollbarLine = new Vector2[2];
        public ContextMenu ContextMenu;

        /// <summary>
        /// Client area X position of left side
        /// </summary>
        public int ClientLeft
        {
            get
            {
                return Left + leftBorder;
            }
        }

        /// <summary>
        /// Client area X position of right side
        /// </summary>
        public int ClientRight
        {
            get
            {
                int res = Right - rightBorder;
                if (showScrollbar)
                    res -= ScrollBarSize;
                return res;
            }
        }

        /// <summary>
        /// Client area Y position of top side
        /// </summary>
        public int ClientTop
        {
            get
            {
                return Top + topBorder + 1;
            }
        }

        /// <summary>
        /// Client area Y position of bottom side
        /// </summary>
        public int ClientBottom
        {
            get
            {
                return Bottom - bottomBorder;
            }
        }

        /// <summary>
        /// Client area width
        /// </summary>
        public int ClientWidth
        {
            get
            {
                int res = Right - rightBorder - Left - leftBorder;
                if (showScrollbar)
                    res -= ScrollBarSize;
                return res;
            }
        }

        /// <summary>
        /// Client area height
        /// </summary>
        public int ClientHeight
        {
            get
            {
                int res = Bottom - bottomBorder - Top - topBorder - 1;
                return res;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.LayerManagerMenu"/> class.
        /// </summary>
        /// <param name="parentWorld"></param>
        /// <param name="parentButton"></param>
        public LayerManagerMenu(World parentWorld, MenuButton parentButton)
        {
            this._parentWorld = parentWorld;
            this._parentButton = parentButton;
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
        }

        public override void Dispose()
        {
        }

        public override void OnKeyUp(KeyEventArgs keyEvent)
        {
        }

        public override bool OnMouseWheel(MouseEventArgs e)
        {
            if (e.X > this.Right || e.X < this.Left || e.Y < this.Top || e.Y > this.Bottom)
                // Outside
                return false;

            // Mouse wheel scroll
            this.scrollBarPosition -= (e.Delta / 6);
            return true;
        }

        public override bool OnMouseDown(MouseEventArgs e)
        {
            if (e.X > Right || e.X < Left || e.Y < Top || e.Y > Bottom)
                // Outside
                return false;

            if (e.X > this.Right - 5 && e.X < this.Right + 5)
            {
                this.isResizing = true;
                return true;
            }

            if (e.Y < ClientTop)
                return false;

            if (e.X > this.Right - ScrollBarSize)
            {
                int numItems = GetNumberOfUncollapsedItems();
                int totalHeight = GetItemsHeight(m_DrawArgs);
                if (totalHeight > ClientHeight)
                {
                    //int totalHeight = numItems * ItemHeight;
                    double percentHeight = (double)ClientHeight / totalHeight;
                    if (percentHeight > 1)
                        percentHeight = 1;

                    double scrollItemHeight = (double)percentHeight * ClientHeight;
                    int scrollPosition = ClientTop + (int)(scrollBarPosition * percentHeight);
                    if (e.Y < scrollPosition)
                        scrollBarPosition -= ClientHeight;
                    else if (e.Y > scrollPosition + scrollItemHeight)
                        scrollBarPosition += ClientHeight;
                    else
                    {
                        scrollGrabPositionY = e.Y - scrollPosition;
                        isScrolling = true;
                    }
                }
            }

            return true;
        }

        DrawArgs m_DrawArgs = null;

        public override bool OnMouseMove(MouseEventArgs e)
        {
            // Reset mouse over effect since mouse moved.
            MouseOverItem = null;

            if (this.isResizing)
            {
                if (e.X > 140 && e.X < 800)
                    this.Width = e.X;

                return true;
            }

            if (this.isScrolling)
            {
                int totalHeight = GetItemsHeight(m_DrawArgs);//GetNumberOfUncollapsedItems() * ItemHeight;
                double percent = (double)totalHeight / ClientHeight;
                scrollBarPosition = (int)((e.Y - scrollGrabPositionY - ClientTop) * percent);
                return true;
            }

            if (e.X > this.Right || e.X < this.Left || e.Y < this.Top || e.Y > this.Bottom)
                // Outside
                return false;

            if (Math.Abs(e.X - this.Right) < 5)
            {
                DrawArgs.MouseCursor = CursorType.SizeWE;
                return true;
            }

            if (e.X > ClientRight)
                return true;

            foreach (LayerMenuItem lmi in this._itemList)
                if (lmi.OnMouseMove(e))
                    return true;

            // Handled
            return true;
        }

        public override bool OnMouseUp(MouseEventArgs e)
        {
            if (this.isResizing)
            {
                this.isResizing = false;
                return true;
            }

            if (this.isScrolling)
            {
                this.isScrolling = false;
                return true;
            }

            foreach (LayerMenuItem lmi in this._itemList)
            {
                if (lmi.OnMouseUp(e))
                    return true;
            }

            if (e.X > this.Right - 20 && e.X < this.Right &&
                e.Y > this.Top && e.Y < this.Top + topBorder)
            {
                this._parentButton.SetPushed(false);
                return true;
            }
            else if (e.X > 0 && e.X < this.Right && e.Y > 0 && e.Y < this.Bottom)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Displays the layer manager context menu for an item.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="item"></param>
        public void ShowContextMenu(int x, int y, LayerMenuItem item)
        {
            if (ContextMenu != null)
            {
                ContextMenu.Dispose();
                ContextMenu = null;
            }
            ContextMenu = new ContextMenu();
            ContextMenu.Show(item.ParentControl, new System.Drawing.Point(x, y));
        }

        /// <summary>
        /// Calculate the number of un-collapsed items in the tree.
        /// </summary>
        public int GetNumberOfUncollapsedItems()
        {
            int numItems = 1;
            foreach (LayerMenuItem subItem in _itemList)
                numItems += subItem.GetNumberOfUncollapsedItems();

            return numItems;
        }

        public int GetItemsHeight(DrawArgs drawArgs)
        {
            int height = 20;
            foreach (LayerMenuItem subItem in _itemList)
                height += subItem.GetItemsHeight(drawArgs);

            return height;
        }

        private void updateList()
        {
            if (this._parentWorld != null && this._parentWorld.RenderableObjects != null)
            {
                for (int i = 0; i < this._parentWorld.RenderableObjects.ChildObjects.Count; i++)
                {
                    RenderableObject curObject = (RenderableObject)this._parentWorld.RenderableObjects.ChildObjects[i];

                    if (i >= this._itemList.Count)
                    {
                        LayerMenuItem newItem = new LayerMenuItem(this, curObject);
                        this._itemList.Add(newItem);
                    }
                    else
                    {
                        LayerMenuItem curItem = (LayerMenuItem)this._itemList[i];
                        if (!curItem.RenderableObject.Name.Equals(curObject.Name))
                        {
                            this._itemList.Insert(i, new LayerMenuItem(this, curObject));
                        }
                    }
                }

                int extraItems = this._itemList.Count - this._parentWorld.RenderableObjects.ChildObjects.Count;
                this._itemList.RemoveRange(this._parentWorld.RenderableObjects.ChildObjects.Count, extraItems);
            }
            else
            {
                this._itemList.Clear();
            }
        }

        public override void RenderContents(DrawArgs drawArgs)
        {
            m_DrawArgs = drawArgs;
            try
            {
                if (itemFont == null)
                {
                    itemFont = drawArgs.CreateFont(World.Settings.LayerManagerFontName,
                        World.Settings.LayerManagerFontSize, World.Settings.LayerManagerFontStyle);

                    // TODO: Fix wingdings menu problems
                    System.Drawing.Font localHeaderFont = new System.Drawing.Font("Arial", 12.0f, FontStyle.Italic | FontStyle.Bold);
                    headerFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, localHeaderFont);

                    System.Drawing.Font wingdings = new System.Drawing.Font("Wingdings", 12.0f);
                    wingdingsFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, wingdings);

                    AddFontResource(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    System.Drawing.Text.PrivateFontCollection fpc = new System.Drawing.Text.PrivateFontCollection();
                    fpc.AddFontFile(Path.Combine(Application.StartupPath, "World Wind Dings 1.04.ttf"));
                    System.Drawing.Font worldwinddings = new System.Drawing.Font(fpc.Families[0], 12.0f);
                    worldwinddingsFont = new Microsoft.DirectX.Direct3D.Font(drawArgs.device, worldwinddings);
                }

                this.updateList();

                this.worldwinddingsFont.DrawText(
                    null,
                    "E",
                    new System.Drawing.Rectangle(this.Right - 16, this.Top + 2, 20, topBorder),
                    DrawTextFormat.None,
                    TextColor);

                int numItems = GetNumberOfUncollapsedItems();
                int totalHeight = GetItemsHeight(drawArgs);//numItems * ItemHeight;
                showScrollbar = totalHeight > ClientHeight;
                if (showScrollbar)
                {
                    double percentHeight = (double)ClientHeight / totalHeight;
                    int scrollbarHeight = (int)(ClientHeight * percentHeight);

                    int maxScroll = totalHeight - ClientHeight;

                    if (scrollBarPosition < 0)
                        scrollBarPosition = 0;
                    else if (scrollBarPosition > maxScroll)
                        scrollBarPosition = maxScroll;

                    // Smooth scroll
                    const float scrollSpeed = 0.3f;
                    float smoothScrollDelta = (scrollBarPosition - scrollSmoothPosition) * scrollSpeed;
                    float absDelta = Math.Abs(smoothScrollDelta);
                    if (absDelta > 100f || absDelta < 3f)
                        // Scroll > 100 pixels and < 1.5 pixels faster
                        smoothScrollDelta = (scrollBarPosition - scrollSmoothPosition) * (float)Math.Sqrt(scrollSpeed);

                    scrollSmoothPosition += smoothScrollDelta;

                    if (scrollSmoothPosition > maxScroll)
                        scrollSmoothPosition = maxScroll;

                    int scrollPos = (int)((float)percentHeight * scrollBarPosition);

                    int color = isScrolling ? World.Settings.scrollbarHotColor : World.Settings.scrollbarColor;
                    MenuUtils.DrawBox(
                        Right - ScrollBarSize + 2,
                        ClientTop + scrollPos,
                        ScrollBarSize - 3,
                        scrollbarHeight + 1,
                        0.0f,
                        color,
                        drawArgs.device);

                    scrollbarLine[0].X = this.Right - ScrollBarSize;
                    scrollbarLine[0].Y = this.ClientTop;
                    scrollbarLine[1].X = this.Right - ScrollBarSize;
                    scrollbarLine[1].Y = this.Bottom;
                    MenuUtils.DrawLine(scrollbarLine,
                        DialogColor,
                        drawArgs.device);
                }

                this.headerFont.DrawText(
                    null, "Layer Manager",
                    new System.Drawing.Rectangle(Left + 5, Top + 1, Width, topBorder - 2),
                    DrawTextFormat.VerticalCenter, TextColor);

                Microsoft.DirectX.Vector2[] headerLinePoints = new Microsoft.DirectX.Vector2[2];
                headerLinePoints[0].X = this.Left;
                headerLinePoints[0].Y = this.Top + topBorder - 1;

                headerLinePoints[1].X = this.Right;
                headerLinePoints[1].Y = this.Top + topBorder - 1;

                MenuUtils.DrawLine(headerLinePoints, DialogColor, drawArgs.device);

                int runningItemHeight = 0;
                if (showScrollbar)
                    runningItemHeight = -(int)Math.Round(scrollSmoothPosition);

                // Set the Direct3D viewport to match the layer manager client area
                // to clip the text to the window when scrolling
                Viewport lmClientAreaViewPort = new Viewport();
                lmClientAreaViewPort.X = ClientLeft;
                lmClientAreaViewPort.Y = ClientTop;
                lmClientAreaViewPort.Width = ClientWidth;
                lmClientAreaViewPort.Height = ClientHeight;
                Viewport defaultViewPort = drawArgs.device.Viewport;
                drawArgs.device.Viewport = lmClientAreaViewPort;
                for (int i = 0; i < _itemList.Count; i++)
                {
                    if (runningItemHeight > ClientHeight)
                        // No more space for items
                        break;
                    LayerMenuItem lmi = (LayerMenuItem)_itemList[i];
                    runningItemHeight += lmi.Render(
                        drawArgs,
                        ClientLeft,
                        ClientTop,
                        runningItemHeight,
                        ClientWidth,
                        ClientBottom,
                        itemFont,
                        wingdingsFont,
                        worldwinddingsFont,
                        MouseOverItem);
                }
                drawArgs.device.Viewport = defaultViewPort;
            }
            catch (Exception caught)
            {
                Log.Write(caught);
            }
        }

        [DllImport("gdi32.dll")]
        static extern int AddFontResource(string lpszFilename);
    }

    public class LayerMenuItem
    {
        RenderableObject m_renderableObject;
        ArrayList m_subItems = new ArrayList();
        private int _x;
        private int _y;
        private int _width;

        private int _itemXOffset = 5;
        private int _expandArrowXSize = 15;
        private int _checkBoxXOffset = 15;
        private int _subItemXIndent = 15;

        int itemOnColor = Color.White.ToArgb();
        int itemOffColor = Color.Gray.ToArgb();

        private bool isExpanded;
        public Control ParentControl;
        LayerManagerMenu m_parent; // menu this item belongs in

        public RenderableObject RenderableObject
        {
            get
            {
                return m_renderableObject;
            }
        }

        /// <summary>
        /// Calculate the number of un-collapsed items in the tree.
        /// </summary>
        public int GetNumberOfUncollapsedItems()
        {
            int numItems = 1;
            if (this.isExpanded)
            {
                foreach (LayerMenuItem subItem in m_subItems)
                    numItems += subItem.GetNumberOfUncollapsedItems();
            }

            return numItems;
        }

        public int GetItemsHeight(DrawArgs drawArgs)
        {
            System.Drawing.Rectangle rect = drawArgs.defaultDrawingFont.MeasureString(
                null,
                this.m_renderableObject.Name, DrawTextFormat.None, System.Drawing.Color.White.ToArgb());

            int height = rect.Height;

            if (m_renderableObject.Description != null && m_renderableObject.Description.Length > 0)
            {
                System.Drawing.SizeF rectF = DrawArgs.Graphics.MeasureString(
                    m_renderableObject.Description,
                    drawArgs.defaultSubTitleFont,
                    _width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset)
                    );

                height += (int)rectF.Height + 15;
            }

            if (height < lastConsumedHeight)
                height = lastConsumedHeight;

            if (this.isExpanded)
            {
                foreach (LayerMenuItem subItem in m_subItems)
                    height += subItem.GetItemsHeight(drawArgs);
            }

            return height;
        }


        private string getFullRenderableObjectName(RenderableObject ro, string name)
        {
            if (ro.ParentList == null)
                return "/" + name;
            else
            {
                if (name == null)
                    return getFullRenderableObjectName(ro.ParentList, ro.Name);
                else
                    return getFullRenderableObjectName(ro.ParentList, ro.Name + "/" + name);
            }
        }


        /// <summary>
        /// Detect expand arrow mouse over
        /// </summary>
        public bool OnMouseMove(MouseEventArgs e)
        {
            if (e.Y < this._y)
                // Over 
                return false;

            if (e.X < m_parent.Left || e.X > m_parent.Right)
                return false;

            if (e.Y < this._y + 20)
            {
                // Mouse is on item
                m_parent.MouseOverItem = this;

                if (e.X > this._x + this._itemXOffset &&
                    e.X < this._x + (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset))
                {
                    if (m_renderableObject is MFW3D.Renderable.RenderableObjectList)
                        DrawArgs.MouseCursor = CursorType.Hand;
                    return true;
                }
                return false;
            }

            foreach (LayerMenuItem lmi in m_subItems)
            {
                if (lmi.OnMouseMove(e))
                {
                    // Mouse is on current item
                    m_parent.MouseOverItem = lmi;
                    return true;
                }
            }

            return false;
        }

        public bool OnMouseUp(MouseEventArgs e)
        {
            if (e.Y < this._y)
                // Above 
                return false;

            if (e.Y <= this._y + 20)
            {
                if (e.X > this._x + this._itemXOffset &&
                    e.X < this._x + (this._itemXOffset + this._width) &&
                    e.Button == MouseButtons.Right)
                {
                    m_parent.ShowContextMenu(e.X, e.Y, this);
                }

                if (e.X > this._x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset &&
                    e.X < this._x + (this._itemXOffset + this._width) &&
                    e.Button == MouseButtons.Left &&
                    m_renderableObject != null &&
                    m_renderableObject.MetaData.Contains("InfoUri"))
                {
                    string infoUri = (string)m_renderableObject.MetaData["InfoUri"];

                    if (World.Settings.UseInternalBrowser || infoUri.StartsWith(@"worldwind://"))
                    {
                        SplitContainer sc = (SplitContainer)this.ParentControl.Parent.Parent;
                        InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                        browser.NavigateTo(infoUri);
                    }
                    else
                    {
                        ProcessStartInfo psi = new ProcessStartInfo();
                        psi.FileName = infoUri;
                        psi.Verb = "open";
                        psi.UseShellExecute = true;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                }

                if (e.X > this._x + this._itemXOffset &&
                    e.X < this._x + (this._itemXOffset + this._expandArrowXSize) &&
                    m_renderableObject is MFW3D.Renderable.RenderableObjectList)
                {
                    MFW3D.Renderable.RenderableObjectList rol = (MFW3D.Renderable.RenderableObjectList)m_renderableObject;
                    if (!rol.DisableExpansion)
                    {
                        this.isExpanded = !this.isExpanded;
                        return true;
                    }
                }

                if (e.X > this._x + this._itemXOffset + this._expandArrowXSize &&
                    e.X < this._x + (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset))
                {
                    if (!m_renderableObject.IsOn && m_renderableObject.ParentList != null &&
                        m_renderableObject.ParentList.ShowOnlyOneLayer)
                        m_renderableObject.ParentList.TurnOffAllChildren();

                    m_renderableObject.IsOn = !m_renderableObject.IsOn;
                    return true;
                }
            }

            if (isExpanded)
            {
                foreach (LayerMenuItem lmi in m_subItems)
                {
                    if (lmi.OnMouseUp(e))
                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.LayerMenuItem"/> class.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="renderableObject"></param>
        public LayerMenuItem(LayerManagerMenu parent, RenderableObject renderableObject)
        {
            m_renderableObject = renderableObject;
            m_parent = parent;
        }

        private void updateList()
        {
            if (this.isExpanded)
            {
                RenderableObjectList rol = (RenderableObjectList)m_renderableObject;
                for (int i = 0; i < rol.ChildObjects.Count; i++)
                {
                    RenderableObject childObject = (RenderableObject)rol.ChildObjects[i];
                    if (i >= m_subItems.Count)
                    {
                        LayerMenuItem newItem = new LayerMenuItem(m_parent, childObject);
                        m_subItems.Add(newItem);
                    }
                    else
                    {
                        LayerMenuItem curItem = (LayerMenuItem)m_subItems[i];

                        if (curItem != null && curItem.RenderableObject != null &&
                            childObject != null &&
                            !curItem.RenderableObject.Name.Equals(childObject.Name))
                        {
                            m_subItems.Insert(i, new LayerMenuItem(m_parent, childObject));
                        }
                    }
                }

                int extraItems = m_subItems.Count - rol.ChildObjects.Count;
                if (extraItems > 0)
                    m_subItems.RemoveRange(rol.ChildObjects.Count, extraItems);
            }
        }

        int lastConsumedHeight = 20;

        public int Render(DrawArgs drawArgs, int x, int y, int yOffset, int width, int height,
            Microsoft.DirectX.Direct3D.Font drawingFont,
            Microsoft.DirectX.Direct3D.Font wingdingsFont,
            Microsoft.DirectX.Direct3D.Font worldwinddingsFont,
            LayerMenuItem mouseOverItem)
        {
            if (ParentControl == null)
                ParentControl = drawArgs.parentControl;

            this._x = x;
            this._y = y + yOffset;
            this._width = width;

            int consumedHeight = 20;

            System.Drawing.Rectangle textRect = drawingFont.MeasureString(null,
                m_renderableObject.Name,
                DrawTextFormat.None,
                System.Drawing.Color.White.ToArgb());

            consumedHeight = textRect.Height;

            if (m_renderableObject.Description != null && m_renderableObject.Description.Length > 0 && !(m_renderableObject is MFW3D.Renderable.Icon))
            {
                System.Drawing.SizeF rectF = DrawArgs.Graphics.MeasureString(
                    m_renderableObject.Description,
                    drawArgs.defaultSubTitleFont,
                    width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset)
                    );

                consumedHeight += (int)rectF.Height + 15;
            }

            lastConsumedHeight = consumedHeight;
            // Layer manager client area height
            int totalHeight = height - y;

            updateList();

            if (yOffset >= -consumedHeight)
            {
                // Part of item or whole item visible
                int color = m_renderableObject.IsOn ? itemOnColor : itemOffColor;
                if (mouseOverItem == this)
                {
                    if (!m_renderableObject.IsOn)
                        // mouseover + inactive color (black)
                        color = 0xff << 24;
                    MenuUtils.DrawBox(m_parent.ClientLeft, _y, m_parent.ClientWidth, consumedHeight, 0,
                        World.Settings.menuOutlineColor, drawArgs.device);
                }

                if (m_renderableObject is MFW3D.Renderable.RenderableObjectList)
                {
                    RenderableObjectList rol = (RenderableObjectList)m_renderableObject;
                    if (!rol.DisableExpansion)
                    {
                        worldwinddingsFont.DrawText(
                            null,
                            (this.isExpanded ? "L" : "A"),
                            new System.Drawing.Rectangle(x + this._itemXOffset, _y, this._expandArrowXSize, height),
                            DrawTextFormat.None,
                            color);
                    }
                }

                string checkSymbol = null;
                if (m_renderableObject.ParentList != null && m_renderableObject.ParentList.ShowOnlyOneLayer)
                    // Radio check
                    checkSymbol = m_renderableObject.IsOn ? "O" : "P";
                else
                    // Normal check
                    checkSymbol = m_renderableObject.IsOn ? "N" : "F";

                worldwinddingsFont.DrawText(
                        null,
                        checkSymbol,
                        new System.Drawing.Rectangle(
                        x + this._itemXOffset + this._expandArrowXSize,
                        _y,
                        this._checkBoxXOffset,
                        height),
                        DrawTextFormat.NoClip,
                        color);


                drawingFont.DrawText(
                    null,
                    m_renderableObject.Name,
                    new System.Drawing.Rectangle(
                    x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset,
                    _y,
                    width - (this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset),
                    height),
                    DrawTextFormat.None,
                    color);

                if (m_renderableObject.Description != null && m_renderableObject.Description.Length > 0 && !(m_renderableObject is MFW3D.Renderable.Icon))
                {
                    drawArgs.defaultSubTitleDrawingFont.DrawText(
                        null,
                        m_renderableObject.Description,
                        new System.Drawing.Rectangle(
                            x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset,
                            _y + textRect.Height,
                            width - (_itemXOffset + _expandArrowXSize + _checkBoxXOffset),
                            height),
                        DrawTextFormat.WordBreak,
                        System.Drawing.Color.Gray.ToArgb());
                }

                if (m_renderableObject.MetaData.Contains("InfoUri"))
                {
                    Vector2[] underlineVerts = new Vector2[2];
                    underlineVerts[0].X = x + this._itemXOffset + this._expandArrowXSize + this._checkBoxXOffset;
                    underlineVerts[0].Y = _y + textRect.Height;
                    underlineVerts[1].X = underlineVerts[0].X + textRect.Width;
                    underlineVerts[1].Y = _y + textRect.Height;

                    MenuUtils.DrawLine(underlineVerts, color, drawArgs.device);
                }
            }

            if (isExpanded)
            {
                for (int i = 0; i < m_subItems.Count; i++)
                {
                    int yRealOffset = yOffset + consumedHeight;
                    if (yRealOffset > totalHeight)
                        // No more space for items
                        break;
                    LayerMenuItem lmi = (LayerMenuItem)m_subItems[i];
                    consumedHeight += lmi.Render(
                        drawArgs,
                        x + _subItemXIndent,
                        y,
                        yRealOffset,
                        width - _subItemXIndent,
                        height,
                        drawingFont,
                        wingdingsFont,
                        worldwinddingsFont,
                        mouseOverItem);
                }
            }

            return consumedHeight;
        }
    }

    public class LayerShortcutMenuButton : MenuButton
    {
        #region к╫спЁит╠
        bool _isPushed = false;
        MFW3D.Renderable.RenderableObject _ro;
        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.LayerShortcutMenuButton"/> class.
        /// </summary>
        /// <param name="imageFilePath"></param>
        /// <param name="ro"></param>
        public LayerShortcutMenuButton(
            string imageFilePath, MFW3D.Renderable.RenderableObject ro)
            : base(imageFilePath)
        {
            this.Description = ro.Name;
            this._ro = ro;
            this._isPushed = ro.IsOn;
        }

        public override void Dispose()
        {
            base.Dispose();
        }

        public override bool IsPushed()
        {
            return this._isPushed;
        }

        public override void SetPushed(bool isPushed)
        {
            this._isPushed = isPushed;
            if (!this._ro.IsOn && this._ro.ParentList != null && this._ro.ParentList.ShowOnlyOneLayer)
                this._ro.ParentList.TurnOffAllChildren();

            this._ro.IsOn = this._isPushed;

            //HACK: Temporary fix
            if (_ro.Name == "Placenames")
                World.Settings.showPlacenames = isPushed;
            else if (_ro.Name == "Boundaries")
                World.Settings.showBoundaries = isPushed;
        }

        public override void OnKeyDown(KeyEventArgs keyEvent)
        {
        }

        public override void OnKeyUp(KeyEventArgs keyEvent)
        {

        }
        public override void Update(DrawArgs drawArgs)
        {
            if (this._ro.IsOn != this._isPushed)
                this._isPushed = this._ro.IsOn;
        }

        public override void Render(DrawArgs drawArgs)
        {
        }

        public override bool OnMouseDown(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseMove(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseUp(MouseEventArgs e)
        {
            return false;
        }

        public override bool OnMouseWheel(MouseEventArgs e)
        {
            return false;
        }
    }

    public enum MenuAnchor
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public sealed class MenuUtils
    {
        private MenuUtils() { }

        public static void DrawLine(Vector2[] linePoints, int color, Device device)
        {
            CustomVertex.TransformedColored[] lineVerts = new CustomVertex.TransformedColored[linePoints.Length];

            for (int i = 0; i < linePoints.Length; i++)
            {
                lineVerts[i].X = linePoints[i].X;
                lineVerts[i].Y = linePoints[i].Y;
                lineVerts[i].Z = 0.0f;

                lineVerts[i].Color = color;
            }

            device.TextureState[0].ColorOperation = TextureOperation.Disable;
            device.VertexFormat = CustomVertex.TransformedColored.Format;

            device.DrawUserPrimitives(PrimitiveType.LineStrip, lineVerts.Length - 1, lineVerts);
        }

        public static void DrawBox(int ulx, int uly, int width, int height, float z, int color, Device device)
        {
            CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[4];
            verts[0].X = (float)ulx;
            verts[0].Y = (float)uly;
            verts[0].Z = z;
            verts[0].Color = color;

            verts[1].X = (float)ulx;
            verts[1].Y = (float)uly + height;
            verts[1].Z = z;
            verts[1].Color = color;

            verts[2].X = (float)ulx + width;
            verts[2].Y = (float)uly;
            verts[2].Z = z;
            verts[2].Color = color;

            verts[3].X = (float)ulx + width;
            verts[3].Y = (float)uly + height;
            verts[3].Z = z;
            verts[3].Color = color;

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.TextureState[0].ColorOperation = TextureOperation.Disable;
            device.DrawUserPrimitives(PrimitiveType.TriangleStrip, verts.Length - 2, verts);
        }

        public static void DrawSector(double startAngle, double endAngle, int centerX, int centerY, int radius, float z, int color, Device device)
        {
            int prec = 7;

            CustomVertex.TransformedColored[] verts = new CustomVertex.TransformedColored[prec + 2];
            verts[0].X = centerX;
            verts[0].Y = centerY;
            verts[0].Z = z;
            verts[0].Color = color;
            double angleInc = (double)(endAngle - startAngle) / prec;

            for (int i = 0; i <= prec; i++)
            {
                verts[i + 1].X = (float)Math.Cos((double)(startAngle + angleInc * i)) * radius + centerX;
                verts[i + 1].Y = (float)Math.Sin((double)(startAngle + angleInc * i)) * radius * (-1.0f) + centerY;
                verts[i + 1].Z = z;
                verts[i + 1].Color = color;
            }

            device.VertexFormat = CustomVertex.TransformedColored.Format;
            device.TextureState[0].ColorOperation = TextureOperation.Disable;
            device.DrawUserPrimitives(PrimitiveType.TriangleFan, verts.Length - 2, verts);
        }
    }
}
