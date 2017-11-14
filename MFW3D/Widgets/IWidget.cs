using System;
using System.Collections;
using System.Windows.Forms;

using MFW3D;

namespace MFW3D.NewWidgets
{	
	/// <summary>
	/// DirectX GUI�ؼ�������
	/// </summary>
	public interface IWidget
	{
		#region ����

		/// <summary>
		/// �ؼ���
		/// </summary>
		string Name{get;set;}

		/// <summary>
		/// ���λ��
		/// </summary>
		System.Drawing.Point Location {get;set;}

		/// <summary>
		/// ����λ��
		/// </summary>
		System.Drawing.Point AbsoluteLocation{get;}

		/// <summary>
		/// ���ϵ�����
		/// </summary>
		System.Drawing.Point ClientLocation{get;}

		/// <summary>
		/// ���ش�С
		/// </summary>
		System.Drawing.Size WidgetSize{get;set;}

		/// <summary>
		/// �û����������С
		/// </summary>
        System.Drawing.Size ClientSize { get;set;}

		/// <summary>
		/// �ؼ��Ƿ����
		/// </summary>
		bool Enabled{get;set;}

		/// <summary>
		/// �Ƿ�ɼ�
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
		/// ���ؼ�
		/// </summary>
		IWidget ParentWidget{get;set;}

		/// <summary>
		/// �ӿؼ�
		/// </summary>
		IWidgetCollection ChildWidgets{get;set;}

		/// <summary>
		/// ���Ӷ���
		/// </summary>
		object Tag{get;set;}
		#endregion

		#region ����

		/// <summary>
		/// ��Ⱦ�ؼ�
		/// ��Ⱦ�߳�.
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Render(DrawArgs drawArgs);


		/// <summary>
		/// ��ʼ����ť����ͼƬ, ��������ָ�����Ŵ�С
		/// </summary>
		/// <param name="drawArgs">The drawing arguments passed from the WW GUI thread.</param>
		void Initialize(DrawArgs drawArgs);

		#endregion
	}
}
