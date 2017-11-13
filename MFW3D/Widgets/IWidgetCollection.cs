using System;
using System.Collections;
using System.Windows.Forms;

using MFW3D;

namespace MFW3D.NewWidgets
{
	/// <summary>
	/// Collection of IWidgets
	/// </summary>
	public interface IWidgetCollection
	{
		#region 方法
		void BringToFront(int index);
		void BringToFront(IWidget widget);
		void Add(IWidget widget);
		void Clear();
		void Insert(IWidget widget, int index);
		IWidget RemoveAt(int index);
		void Remove (IWidget widget);
		#endregion

		#region 属性
		int Count{get;}
		#endregion

		#region 索引器
		IWidget this[int index] {get;set;}
		#endregion
	}

}
