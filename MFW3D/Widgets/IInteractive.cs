using System;
using System.Collections;
using System.Windows.Forms;

using MFW3D;

namespace MFW3D.NewWidgets
{
	/// <summary>
	/// 鼠标点击委托
	/// </summary>
	public delegate void MouseClickAction(System.Windows.Forms.MouseEventArgs e);

	/// <summary>
	/// 用于接受用户输入的实现
	/// </summary>
	public interface IInteractive
	{
		#region 属性

		/// <summary>
		/// 左键点击操作
		/// </summary>
		MouseClickAction LeftClickAction{set; get;}

		/// <summary>
		/// 右键点击操作
		/// </summary>
		MouseClickAction RightClickAction{set; get;}

		#endregion

		#region 方法

		/// <summary>
		/// 鼠标按下句柄
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseDown(MouseEventArgs e);

		/// <summary>
		/// 鼠标抬起事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseUp(MouseEventArgs e);

		/// <summary>
		/// 鼠标移动事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseMove(MouseEventArgs e);

		/// <summary>
		/// 鼠标滚轮事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>				
		bool OnMouseWheel(MouseEventArgs e);
		
		/// <summary>
		/// 鼠标进入事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseEnter(EventArgs e);
		
		/// <summary>
		/// 鼠标离开事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseLeave(EventArgs e);

		/// <summary>
		/// 按键按下事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyDown(KeyEventArgs e);
		
		/// <summary>
		/// 按键抬起事件
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyUp(KeyEventArgs e);

        /// <summary>
        /// 事件按下事件
        /// </summary>
        /// <param name="e">Event Args</param>
        /// <returns></returns>
        bool OnKeyPress(KeyPressEventArgs e);

		#endregion
	}
}
