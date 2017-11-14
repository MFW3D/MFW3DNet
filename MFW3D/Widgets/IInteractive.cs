using System;
using System.Collections;
using System.Windows.Forms;

using MFW3D;

namespace MFW3D.NewWidgets
{
	/// <summary>
	/// �����ί��
	/// </summary>
	public delegate void MouseClickAction(System.Windows.Forms.MouseEventArgs e);

	/// <summary>
	/// ���ڽ����û������ʵ��
	/// </summary>
	public interface IInteractive
	{
		#region ����

		/// <summary>
		/// ����������
		/// </summary>
		MouseClickAction LeftClickAction{set; get;}

		/// <summary>
		/// �Ҽ��������
		/// </summary>
		MouseClickAction RightClickAction{set; get;}

		#endregion

		#region ����

		/// <summary>
		/// ��갴�¾��
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseDown(MouseEventArgs e);

		/// <summary>
		/// ���̧���¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseUp(MouseEventArgs e);

		/// <summary>
		/// ����ƶ��¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseMove(MouseEventArgs e);

		/// <summary>
		/// �������¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>				
		bool OnMouseWheel(MouseEventArgs e);
		
		/// <summary>
		/// �������¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseEnter(EventArgs e);
		
		/// <summary>
		/// ����뿪�¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnMouseLeave(EventArgs e);

		/// <summary>
		/// ���������¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyDown(KeyEventArgs e);
		
		/// <summary>
		/// ����̧���¼�
		/// </summary>
		/// <param name="e">Event args</param>
		/// <returns>If this widget handled this event</returns>
		bool OnKeyUp(KeyEventArgs e);

        /// <summary>
        /// �¼������¼�
        /// </summary>
        /// <param name="e">Event Args</param>
        /// <returns></returns>
        bool OnKeyPress(KeyPressEventArgs e);

		#endregion
	}
}
