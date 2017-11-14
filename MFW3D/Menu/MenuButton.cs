using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.DirectX.Direct3D;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.DirectX;

namespace MFW3D.Menu
{
    public abstract class MenuButton : IMenu
    {
        #region Private Members

        private string _iconTexturePath;
        private Texture m_iconTexture;
        private System.Drawing.Size _iconTextureSize;
        string _description;
        float curSize;
        static int white = System.Drawing.Color.White.ToArgb();
        static int black = System.Drawing.Color.Black.ToArgb();
        static int transparent = Color.FromArgb(140, 255, 255, 255).ToArgb();
        int alpha;
        const int alphaStep = 30;
        const float zoomSpeed = 1.2f;

        public static float NormalSize;
        public static float SelectedSize;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref= "T:WorldWind.Menu.MenuButton"/> class.
        /// </summary>
        protected MenuButton()
        {
        }

        #region ÊôÐÔ
        public string Description
        {
            get
            {
                if (this._description == null)
                    return "N/A";
                else
                    return this._description;
            }
            set
            {
                this._description = value;
            }
        }

        public Texture IconTexture
        {
            get
            {
                return m_iconTexture;
            }
        }

        public System.Drawing.Size IconTextureSize
        {
            get
            {
                return this._iconTextureSize;
            }
        }
        #endregion

        public MenuButton(string iconTexturePath)
        {
            this._iconTexturePath = iconTexturePath;
        }

        public abstract bool IsPushed();
        public abstract void SetPushed(bool isPushed);

        public void InitializeTexture(Device device)
        {
            try
            {
                m_iconTexture = ImageHelper.LoadIconTexture(this._iconTexturePath);

                using (Surface s = m_iconTexture.GetSurfaceLevel(0))
                {
                    SurfaceDescription desc = s.Description;
                    this._iconTextureSize = new Size(desc.Width, desc.Height);
                }
            }
            catch
            {
            }
        }

        public void RenderLabel(DrawArgs drawArgs, int x, int y, int buttonHeight, bool selected, MenuAnchor anchor)
        {
            if (selected)
            {
                if (buttonHeight == curSize)
                {
                    alpha += alphaStep;
                    if (alpha > 255)
                        alpha = 255;
                }
            }
            else
            {
                alpha -= alphaStep;
                if (alpha < 0)
                {
                    alpha = 0;
                    return;
                }
            }

            int halfWidth = (int)(SelectedSize * 0.75);
            int label_x = x - halfWidth + 1;
            int label_y = (int)(y + SelectedSize) + 1;

            DrawTextFormat format = DrawTextFormat.NoClip | DrawTextFormat.Center | DrawTextFormat.WordBreak;

            if (anchor == MenuAnchor.Bottom)
            {
                format |= DrawTextFormat.Bottom;
                label_y = y - 202;
            }

            Rectangle rect = new System.Drawing.Rectangle(
                label_x,
                label_y,
                (int)halfWidth * 2,
                200);

            if (rect.Right > drawArgs.screenWidth)
            {
                rect = Rectangle.FromLTRB(rect.Left, rect.Top, drawArgs.screenWidth, rect.Bottom);
            }

            drawArgs.toolbarFont.DrawText(
                null,
                Description,
                rect,
                format,
                black & 0xffffff + (alpha << 24));

            rect.Offset(2, 0);

            drawArgs.toolbarFont.DrawText(
                null,
                Description,
                rect,
                format,
                black & 0xffffff + (alpha << 24));

            rect.Offset(0, 2);

            drawArgs.toolbarFont.DrawText(
                null,
                Description,
                rect,
                format,
                black & 0xffffff + (alpha << 24));

            rect.Offset(-2, 0);

            drawArgs.toolbarFont.DrawText(
                null,
                Description,
                rect,
                format,
                black & 0xffffff + (alpha << 24));

            rect.Offset(1, -1);

            drawArgs.toolbarFont.DrawText(
                null,
                Description,
                rect,
                format,
                white & 0xffffff + (alpha << 24));
        }

        public float CurrentSize
        {
            get { return curSize; }
        }

        [Obsolete]
        public void RenderEnabledIcon(Sprite sprite, DrawArgs drawArgs, float centerX, float topY,
                                      bool selected)
        {
            RenderEnabledIcon(sprite, drawArgs, centerX, topY, selected, MenuAnchor.Top);
        }

        public void RenderEnabledIcon(Sprite sprite, DrawArgs drawArgs, float centerX, float topY,
            bool selected, MenuAnchor anchor)
        {
            float width = selected ? MenuButton.SelectedSize : width = MenuButton.NormalSize;

            RenderLabel(drawArgs, (int)centerX, (int)topY, (int)width, selected, anchor);

            int color = selected ? white : transparent;

            float centerY = topY + curSize * 0.5f;
            this.RenderIcon(sprite, (int)centerX, (int)centerY, (int)curSize, (int)curSize, color, m_iconTexture);

            if (curSize == 0)
                curSize = width;
            if (width > curSize)
            {
                curSize = (int)(curSize * zoomSpeed);
                if (width < curSize)
                    curSize = width;
            }
            else if (width < curSize)
            {
                curSize = (int)(curSize / zoomSpeed);
                if (width > curSize)
                    curSize = width;
            }
        }

        private void RenderIcon(Sprite sprite, float centerX, float centerY,
            int buttonWidth, int buttonHeight, int color, Texture t)
        {
            int halfIconWidth = (int)(0.5f * buttonWidth);
            int halfIconHeight = (int)(0.5f * buttonHeight);

            float scaleWidth = (float)buttonWidth / this._iconTextureSize.Width;
            float scaleHeight = (float)buttonHeight / this._iconTextureSize.Height;

            sprite.Transform = Matrix.Transformation2D(
                Vector2.Empty, 0.0f,
                new Vector2(scaleWidth, scaleHeight),
                Vector2.Empty,
                0.0f,
                new Vector2(centerX, centerY));

            sprite.Draw(t,
                new Vector3(this._iconTextureSize.Width / 2.0f, this._iconTextureSize.Height / 2.0f, 0),
                Vector3.Empty,
                color);
        }

        public abstract void Update(DrawArgs drawArgs);
        public abstract void Render(DrawArgs drawArgs);
        public abstract bool OnMouseUp(MouseEventArgs e);
        public abstract bool OnMouseMove(MouseEventArgs e);
        public abstract bool OnMouseDown(MouseEventArgs e);
        public abstract bool OnMouseWheel(MouseEventArgs e);
        public abstract void OnKeyUp(KeyEventArgs keyEvent);
        public abstract void OnKeyDown(KeyEventArgs keyEvent);

        public virtual void Dispose()
        {
            if (m_iconTexture != null)
            {
                m_iconTexture.Dispose();
                m_iconTexture = null;
            }
        }
    }
}
