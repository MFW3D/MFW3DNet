using System;
using System.Collections;
using System.Windows.Forms;

using MFW3D;

namespace MFW3D.NewWidgets
{	
	/// <summary>
	/// DirectX GUI控件基础类
	/// </summary>
	public interface IWidget
	{
		#region 属性

		/// <summary>
		/// 控件名
		/// </summary>
		string Name{get;set;}

		/// <summary>
		/// 相对位置
		/// </summary>
		System.Drawing.Point Location {get;set;}

		/// <summary>
		/// 绝对位置
		/// </summary>
		System.Drawing.Point AbsoluteLocation{get;}

		/// <summary>
		/// 左上点坐标
		/// </summary>
		System.Drawing.Point ClientLocation{get;}

		/// <summary>
		/// 像素大小
		/// </summary>
		System.Drawing.Size WidgetSize{get;set;}

		/// <summary>
		/// 用户操作对象大小
		/// </summary>
        System.Drawing.Size ClientSize { get;set;}

		/// <summary>
		/// 控件是否可用
		/// </summary>
		bool Enabled{get;set;}

		/// <summary>
		/// 是否可见
		/// </summary>
		bool Visible{get;set;}

		/// <summary>
		/// Whether this widget should count for height calculations - HACK until we do real layout
		/// </summary>
		bool CountHeight{get; set;}

		/// <summary>
		/// Whether this widget should count for width calculations - HACK until we do real layout
		/// </summary>
		bool CountWidth{get; set;}

		/// <summary>
		/// 父控件
		/// </summary>
		IWidget ParentWidget{get;set;}

		/// <summary>
		/// 子控件
		/// </summary>
		IWidgetCollection ChildWidgets{get;set;}

		/// <summary>
		/// 附加对象
		/// </summary>
		object Tag{get;set;}
		#endregion

		#region 方法

		/// <summary>
		/// 渲染控件
		/// 渲染线程.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Render(DrawArgs drawArgs);


		/// <summary>
		/// 初始化按钮加载图片, 创建对象并指定缩放大小
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Initialize(DrawArgs drawArgs);

		#endregion
	}
}
