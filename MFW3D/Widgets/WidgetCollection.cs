using System;

namespace MFW3D.NewWidgets
{
	/// <summary>
	/// Summary description for WidgetCollection.
	/// </summary>
	public class WidgetCollection : MFW3D.NewWidgets.IWidgetCollection
	{
		System.Collections.ArrayList m_ChildWidgets = new System.Collections.ArrayList();
		
		public WidgetCollection()
		{
		}

		#region Methods
		public void BringToFront(int index)
		{
			MFW3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as MFW3D.NewWidgets.IWidget;
			if(currentWidget != null)
			{
				m_ChildWidgets.RemoveAt(index);
				m_ChildWidgets.Insert(0, currentWidget);
			}
		}

		public void BringToFront(MFW3D.NewWidgets.IWidget widget)
		{
			int foundIndex = -1;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				MFW3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as MFW3D.NewWidgets.IWidget;
				if(currentWidget != null)
				{		
					if(currentWidget == widget)
					{
						foundIndex = index;
						break;
					}
				}	
			}

			if(foundIndex > 0)
			{
				BringToFront(foundIndex);
			}	
		}

		public void Add(MFW3D.NewWidgets.IWidget widget)
		{
			m_ChildWidgets.Add(widget);
		}

		public void Clear()
		{
			m_ChildWidgets.Clear();
		}

		public void Insert(MFW3D.NewWidgets.IWidget widget, int index)
		{
			if(index <= m_ChildWidgets.Count)
			{
				m_ChildWidgets.Insert(index, widget);
			}
			//probably want to throw an indexoutofrange type of exception
		}

		public MFW3D.NewWidgets.IWidget RemoveAt(int index)
		{
			if(index < m_ChildWidgets.Count)
			{
				MFW3D.NewWidgets.IWidget oldWidget = m_ChildWidgets[index] as MFW3D.NewWidgets.IWidget;
				m_ChildWidgets.RemoveAt(index);
				return oldWidget;
			}
			else
			{
				return null;
			}
		}

		public void Remove(MFW3D.NewWidgets.IWidget widget)
		{
			int foundIndex = -1;

			for(int index = 0; index < m_ChildWidgets.Count; index++)
			{
				MFW3D.NewWidgets.IWidget currentWidget = m_ChildWidgets[index] as MFW3D.NewWidgets.IWidget;
				if(currentWidget != null)
				{		
					if(currentWidget == widget)
					{
						foundIndex = index;
						break;
					}
				}	
			}

			if(foundIndex >= 0)
			{
				m_ChildWidgets.RemoveAt(foundIndex);
			}
		}
		#endregion

		#region Properties
		public int Count
		{
			get
			{
				return m_ChildWidgets.Count;
			}
		}

		#endregion

		#region Indexers
		public MFW3D.NewWidgets.IWidget this[int index]
		{
			get
			{
				return m_ChildWidgets[index] as MFW3D.NewWidgets.IWidget;
			}
			set
			{
				m_ChildWidgets[index] = value;
			}
		}
		#endregion

	}
}
