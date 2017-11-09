using System;
using System.Collections;
using System.Text;
using System.Drawing;
using MFW3D;
using MFW3D.Renderable;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;

namespace OgrVectorImporter
{
    /// <summary>
    /// 渲染消息
    /// </summary>
    public class WaitMessage : RenderableObject
    {
        #region Private members
        private string _Text = "";
        private string leadString = "Vector Importer:\n";
        private int color = Color.White.ToArgb();
        private int distanceFromCorner = 25;
        #endregion

        public WaitMessage()
                : base("Status message", Vector3.Empty, Quaternion.Identity)
        {
            this.RenderPriority = RenderPriority.Icons;
            this.IsOn = true;
        }

        public string Text
        {
            get { return _Text; }
            set { _Text = value; }
        }

        #region 渲染对象方法
        public override void Render(DrawArgs drawArgs)
        {
            // 默认的设置渲染
            Rectangle bounds = drawArgs.defaultDrawingFont.MeasureString(null, leadString + _Text, DrawTextFormat.None, 0);
            drawArgs.defaultDrawingFont.DrawText(null, leadString + _Text,
                drawArgs.screenWidth - bounds.Width - distanceFromCorner, drawArgs.screenHeight - bounds.Height - distanceFromCorner,
                color);
        }

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

        public override void Dispose()
        {
        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            return false;
        }
        #endregion
    }
}
