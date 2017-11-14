using System;
using System.ComponentModel;
using System.Collections;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Utility;
using System.Threading;

namespace MFW3D.Renderable
{
	/// <summary>
	/// 图层管理树，管理节点
	/// </summary>
	public class RenderableObjectList : RenderableObject
	{
        protected ReaderWriterLock m_childrenRWLock = new ReaderWriterLock();
		protected ArrayList m_children = new ArrayList();

        protected ReaderWriterLock m_delRWLock = new ReaderWriterLock();
        protected ArrayList m_delList = new ArrayList();
        protected bool m_delAll = false;

		string m_DataSource = null;
		TimeSpan m_RefreshInterval = TimeSpan.MaxValue;

		World m_ParentWorld = null;
		Cache m_Cache = null;
		System.Timers.Timer m_RefreshTimer = null;
		public bool ShowOnlyOneLayer;
		
		public bool DisableExpansion
		{
			get{ return m_disableExpansion; }
			set{ m_disableExpansion = value; }
		}
        private bool m_disableExpansion = false;

		public System.Timers.Timer RefreshTimer
		{
			get
			{
				return m_RefreshTimer;
			}
		}
		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.RenderableObjectList"/> class.
		/// </summary>
		/// <param name="name"></param>
		public RenderableObjectList(string name) : base(name, new Vector3(0,0,0), new Quaternion())
		{
			this.isSelectable = true;
		}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dataSource"></param>
        /// <param name="refreshInterval"></param>
        /// <param name="parentWorld"></param>
        /// <param name="cache"></param>
		public RenderableObjectList(
			string name, 
			string dataSource, 
			TimeSpan refreshInterval,
			World parentWorld,
			Cache cache
			) : base(name, new Vector3(0,0,0), new Quaternion())
		{
			isSelectable = true;
			m_DataSource = dataSource;
			m_RefreshInterval = refreshInterval;

			m_ParentWorld = parentWorld;
			m_Cache = cache;

			m_RefreshTimer = new System.Timers.Timer(
				refreshInterval.Hours * 60 * 60 * 1000 +
				refreshInterval.Minutes * 60 * 1000 + 
				refreshInterval.Seconds * 1000
				);
			m_RefreshTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_RefreshTimer_Elapsed);
		}

		public void StartRefreshTimer()
		{
			if(m_RefreshTimer != null)
			{
				m_RefreshTimer.Start();
			}
		}

        /// <summary>
        /// 返回第一个显示的对象名字
        /// </summary>
        /// <example>Get the placenames LayerSet.
        /// <code>
        /// RenderableObject placenames = CurrentWorld.RenderableObjects.GetObject("Placenames"));
        /// </code></example>
        /// <param name="name">The name to search for</param>
        /// <returns>The first <c>RenderableObject</c> that matched the specified name, or <c>nullk</c> if none was found.</returns>
        public virtual RenderableObject GetObject(string name)
        {
            RenderableObject result = null;
            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (ro.Name.Equals(name))
                    {
                        result = ro;
                        break;
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }
            return result;
        }

        /// <summary>
        /// 返回直接或间接的对象，根据名字查询
        /// </summary>
        /// <example> Get all QuadTileSets defined in this world:
        /// <code>
        /// RenderableObjectList allQTS = CurrentWorld.RenderableObjects.GetObjects(null, typeof(QuadTileSet));
        /// </code></example>
        /// <param name="name">The name of the <c>RenderableObject</c> to search for, or <c>null</c> if any name should match.</param>
        /// <param name="objectType">The object type to search for, or <c>null</c> if any type should match.</param>
        /// <returns>A list of all <c>RenderableObject</c>s that match the given search criteria (may be empty), or <c>null</c> if an error occurred.</returns>
        public virtual RenderableObjectList GetObjects(string name, Type objectType)
        {
            RenderableObjectList result = new RenderableObjectList("results");
            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (ro.GetType() == typeof(RenderableObjectList))
                    {
                        RenderableObjectList sub = ro as RenderableObjectList;

                        RenderableObjectList subres = sub.GetObjects(name, objectType);
                        foreach (RenderableObject hit in subres.ChildObjects)
                            result.Add(hit);
                    }
                    if (ro.Name.Equals(name) && ((objectType == null) || (ro.GetType() == objectType)))
                        result.Add(ro);
                }
            }
            catch
            {
                result = null;
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

            return result;

        }

		/// <summary>
		/// Enables layer with specified name
		/// </summary>
		/// <returns>False if layer not found.</returns>
        public virtual bool Enable(string name)
        {
            if (name == null || name.Length == 0)
                return true;

            string lowerName = name.ToLower();
            bool result = false;

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in m_children)
                {
                    if (ro.Name.ToLower() == lowerName)
                    {
                        ro.IsOn = true;
                        result = true;
                        break;
                    }

                    RenderableObjectList rol = ro as RenderableObjectList;
                    if (rol == null)
                        continue;

                    // Recurse down
                    if (rol.Enable(name))
                    {
                        rol.isOn = true;
                        result = true;
                        break;
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

            return result;
        }

		public virtual void TurnOffAllChildren()
		{
            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    ro.IsOn = false;
                    if (ro is RenderableObjectList)
                    {
                        RenderableObjectList list = ro as RenderableObjectList;
                        list.TurnOffAllChildren();
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }
		}

        public virtual void TurnOnAllChildren()
        {
            // Now render just the icons
            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    ro.IsOn = true;
                    if (ro is RenderableObjectList)
                    {
                        RenderableObjectList list = ro as RenderableObjectList;
                        list.TurnOnAllChildren();
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }
        }

        /// <summary>
        /// Menu action to check all children of a ROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetCheckAllChildren()
        {
            this.TurnOnAllChildren();
        }

        /// <summary>
        /// Menu action to uncheck all children of a ROL
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void GetUncheckAllChildren()
        {
            this.TurnOffAllChildren();
        }

		/// <summary>
		/// List containing the children layers
		/// </summary>
		[Browsable(false)]
		public virtual ArrayList ChildObjects
		{
			get { return this.m_children; }
		}

		/// <summary>
		/// Number of child objects.
		/// </summary>
		[Browsable(false)]
		public virtual int Count
		{
			get { return this.m_children.Count; }
		}

        public override void Initialize(DrawArgs drawArgs)
        {
            if (!this.IsOn)
                return;

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    try
                    {
                        if (ro.IsOn)
                            ro.Initialize(drawArgs);
                    }
                    catch (Exception caught)
                    {
                        Log.Write(Log.Levels.Error, "ROBJ", string.Format("{0}: {1} ({2})",
                            Name, caught.Message, ro.Name));
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

            this.isInitialized = true;
        }

        /// <summary>
        /// Update ROL - this is on the worker thread and is the only thing that writes to the child list.
        /// While this makes adds and deletes occur slower than before it eliminates both the deadlock when 
        /// a thread with a lock updates the UI when the main thread has the lock already and the non-atomic
        /// add/deletes
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Update(DrawArgs drawArgs)
        {
            // Do deletes even if we aren't turned on
            Remove();

            // Do sort even if we aren't turned on
            //if (NeedsSort)
            //    SortChildren();

            if (!this.IsOn)
                return;

            if (!this.isInitialized)
                this.Initialize(drawArgs);

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (ro.ParentList == null)
                        ro.ParentList = this;

                    if (ro.IsOn)
                    {
                        ro.Update(drawArgs);
                    }
                    else
                    {
                        // dispose renderable objects that aren't disabled
                        // TODO: would be cool to retain them for a bit to allow
                        // quick toggles without having to tear down / reload them!
                        //if (ro.isInitialized)
                        //    ro.Dispose();
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

        }

        public override bool PerformSelectionAction(DrawArgs drawArgs)
        {
            bool result = false;
            if (!this.IsOn)
                return false;

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (ro.IsOn && ro.isSelectable)
                    {
                        if (ro.PerformSelectionAction(drawArgs))
                            result = true;
                    }
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

            return result;
        }

        /// <summary>
        /// The ROL Render actually doesn't get called much because the World class used to do ROL rendering.
        /// Moved this code into Render(drawArgs, priority) so if your override Render, make sure to call either
        /// call that function OR replicate it's behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        public override void Render(DrawArgs drawArgs)
        {
            // if we dont know the current render priority then just use our own
            RenderChildren(drawArgs, this.RenderPriority);
        }

        /// <summary>
        /// This used to be done in the World class but moved here so folks could override a ROL's behavior.
        /// </summary>
        /// <param name="drawArgs"></param>
        /// <param name="priority"></param>
        public virtual void RenderChildren(DrawArgs drawArgs, RenderPriority priority)
        {
            if (!IsOn)
                return;

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                {
                    if (ro.IsOn)
                    {
                        if ((ro is RenderableObjectList) )//&& !(ro is Icons))
                        {
                            (ro as RenderableObjectList).RenderChildren(drawArgs, priority);
                        }
                        // hack to render both surface images and terrain mapped images.
                        else if (priority == RenderPriority.TerrainMappedImages)
                        {
                            if (ro.RenderPriority == RenderPriority.SurfaceImages || ro.RenderPriority == RenderPriority.TerrainMappedImages)
                            {
                                ro.Render(drawArgs);
                            }
                        }
                        else if (ro.RenderPriority == priority)
                        {
                            ro.Render(drawArgs);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex);
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }
        }

        public override XmlNode ToXml(XmlDocument worldDoc)
        {
            XmlNode childlayerSetNode = worldDoc.CreateElement("ChildLayerSet");

            XmlAttribute name = worldDoc.CreateAttribute("Name");
            name.Value = Name;
            XmlAttribute showAtStartup = worldDoc.CreateAttribute("ShowAtStartup");
            showAtStartup.Value = IsOn.ToString();
            XmlAttribute showOnlyOneLayer = worldDoc.CreateAttribute("ShowOnlyOneLayer");
            showOnlyOneLayer.Value = ShowOnlyOneLayer.ToString();

            childlayerSetNode.Attributes.Append(name);
            childlayerSetNode.Attributes.Append(showAtStartup);
            childlayerSetNode.Attributes.Append(showOnlyOneLayer);

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in ChildObjects)
                {
                    /*if (ro is RenderableObjectList)
                        childlayerSetNode.AppendChild(saveChildLayerSet((RenderableObjectList)ro, worldDoc));
                    if (ro is PolygonFeature)
                        childlayerSetNode.AppendChild(savePolygonFeature((PolygonFeature)ro, worldDoc));
                    if (ro is LineFeature)
                        childlayerSetNode.AppendChild(saveLineFeature((LineFeature)ro, worldDoc));
                    if (ro is Icons)
                        childlayerSetNode.AppendChild(saveIcons((Icons)ro, worldDoc));
                    if (ro is QuadTileSet)
                        childlayerSetNode.AppendChild(saveQuadTileSet((QuadTileSet)ro, worldDoc));*/

                    childlayerSetNode.AppendChild(ro.ToXml(worldDoc));
                }
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }

            return childlayerSetNode;
        }

        public override void Dispose()
        {
            this.isInitialized = false;

            m_childrenRWLock.AcquireReaderLock(Timeout.Infinite);
            try
            {
                foreach (RenderableObject ro in this.m_children)
                    ro.Dispose();

                if (m_RefreshTimer != null && m_RefreshTimer.Enabled)
                    m_RefreshTimer.Stop();
            }
            finally
            {
                m_childrenRWLock.ReleaseReaderLock();
            }
        }

        public bool GetReaderLock(int retrys)
        {
            for (int i = 0; i < retrys; i++)
            {
                try
                {
                    m_childrenRWLock.AcquireWriterLock(5);
                    return true;
                }
                catch
                {
                }
                // don't know if this is required
                Thread.Sleep(5);
            }
            return false;
        }

        public void ReleaseReaderLock()
        {
            m_childrenRWLock.ReleaseReaderLock();
        }


		/// <summary>
        /// Tries to get the writer lock for the children list.  Each try takes 10ms
        /// </summary>
        /// <param name="retrys">How many times to try</param>
        /// <returns></returns>
        public bool GetWriterLock(int retrys)
        {
            for (int i = 0; i < retrys; i++)
            {
                try
                {
                    m_childrenRWLock.AcquireWriterLock(5);
                    return true;
                }
                catch
                {
                }
                // don't know if this is required
                Thread.Sleep(5);
            }
            return false;
        }

        public void ReleaseWriterLock()
        {
            m_childrenRWLock.ReleaseWriterLock();
        }


        /// <summary>
        /// 添加对象到图层.  If the new object has the same name as an existing object in this 
        /// ROL it gets a number appended.  If the new object is a ROL and there was already a ROL with the same 
        /// name then the children of the new ROL gets added to the old ROL.
        /// 
        /// Not sure who uses this but the functionality was kept this way.
		/// </summary>
        public virtual void Add(RenderableObject ro)
        {
            ro.ParentList = this;

            RenderableObjectList dupList = null;
            RenderableObject duplicate = null;

            // We get a write lock here because if you get a reader lock and then upgrade
            // the data can change on us before we get the write lock.
            //
            // This is somewhat unfortunate since we spend a bit of time going through the
            // child list.
            //
            // if we can't get the writer lock in 2 seconds something is probably borked
            if (GetWriterLock(200))
            {
                try
                {
                    // find duplicate names
                    foreach (RenderableObject childRo in m_children)
                    {
                        if (childRo is RenderableObjectList &&
                            ro is RenderableObjectList &&
                            childRo.Name == ro.Name)
                        {
                            dupList = (RenderableObjectList)childRo;
                            break;
                        }
                        else if (childRo.Name == ro.Name)
                        {
                            duplicate = childRo;
                            break;
                        }
                    }


                    // if we have two ROLs with the same name, don't rename the new ROL but add the children of the new ROL to the 
                    // existing ROL.
                    if (dupList != null)
                    {
                        RenderableObjectList rol = (RenderableObjectList)ro;

                        foreach (RenderableObject childRo in rol.ChildObjects)
                        {
                            dupList.Add(childRo);
                        }
                    }
                    else
                    {
                        // Try to find an unused number for this name
                        if (duplicate != null)
                        {
                            for (int i = 1; i < 1000; i++)
                            {
                                ro.Name = string.Format("{0} [{1}]", duplicate.Name, i);
                                bool found = false;

                                foreach (RenderableObject childRo in m_children)
                                {
                                    if (childRo.Name == ro.Name)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    break;
                                }
                            }
                        }

                        // Add the new child
                        m_children.Add(ro);

                        // Resort during the next update
                        // NeedsSort = true;
                    }
                }
                finally
                {
                    m_childrenRWLock.ReleaseWriterLock();
                }
            }
            else
            {
                MessageBox.Show("Unable to add new object " + ro.Name);
            }
        }

		/// <summary>
        /// Removes specified RO from this ROL
        /// </summary>
		/// <param name="objectName">Name of object to remove</param>
        public virtual void Remove(string objectName)
        {
            m_delRWLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                m_delList.Add(objectName);
            }
            finally
            {
                m_delRWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes specified RO from this ROL
        /// </summary>
        /// <param name="ro">RO to be removed.</param>
        public virtual void Remove(RenderableObject ro)
        {
            m_delRWLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                m_delList.Add(ro);
            }
            finally
            {
                m_delRWLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// Removes all child ROs
        /// </summary>
        public virtual void RemoveAll()
        {
            m_delAll = true;
        }

        /// <summary>
        /// Removes all deleted ROs.  Called only in Update (Worker thread)
        /// </summary>
        private void Remove()
        {
            // get the writer lock for the delList because we need to clear at the end.
            // if we just do a reader lock and upgrade you can get another
            // writer in there which means we'd clear the delList before
            // we remove new items for deletion from the child list.
            m_delRWLock.AcquireWriterLock(Timeout.Infinite);
            try
            {
                // if we need to clear everyone just do it.
                if (m_delAll)
                {
                    if (GetWriterLock(10))
                    {
                        try
                        {
                            while (m_children.Count > 0)
                            {
                                RenderableObject ro = (RenderableObject)m_children[0];
                                m_children.RemoveAt(0);
                                ro.Dispose();
                            }
                        }
                        finally
                        {
                            m_childrenRWLock.ReleaseWriterLock();
                        }
                        m_delAll = false;

                        // we can safely clear the list
                        m_delList.Clear();
                    }
                    else
                    {
                        // try next update cycle
                    }
                }
                else
                {
                    // get a writer lock so we can remove from the child list
                    if (GetWriterLock(10))
                    {
                        try
                        {
                            foreach (object data in m_delList)
                            {
                                RenderableObject rod = data as RenderableObject;
                                if (rod != null)
                                {
                                    this.m_children.Remove(rod);
                                    rod.ParentList = null;
                                    rod.Dispose();
                                }
                                string objectName = data as String;
                                if (objectName != null)
                                {
                                    for (int i = 0; i < this.m_children.Count; i++)
                                    {
                                        RenderableObject ro = (RenderableObject)this.m_children[i];
                                        if (ro.Name.Equals(objectName))
                                        {
                                            this.m_children.RemoveAt(i);
                                            ro.ParentList = null;
                                            ro.Dispose();
                                            break;
                                        }
                                    }
                                }
                            }

                            // we can safely clear the list
                            m_delList.Clear();
                        }
                        finally
                        {
                            m_childrenRWLock.ReleaseWriterLock();
                        }
                    }
                    else
                    {
                        // try next update cycle
                    }
                }
            }
            finally
            {
                m_delRWLock.ReleaseWriterLock();
            }
        }

        //
        //  REMOVED because we no longer ever render all ROs in a a ROL without explicitly checking
        //  render priority anyway.
        //

        /// <summary>
        /// Sorts the children list according to priority - ONLY called in worker thread (in Update())
        /// </summary>
        //private void SortChildren()
        //{
        //    int index = 0;
        //    m_childrenRWLock.AcquireWriterLock(Timeout.Infinite);
        //    try
        //    {
        //        while (index + 1 < m_children.Count)
        //        {
        //            RenderableObject a = (RenderableObject)m_children[index];
        //            RenderableObject b = (RenderableObject)m_children[index + 1];
        //            if (a.RenderPriority > b.RenderPriority)
        //            {
        //                // Swap
        //                m_children[index] = b;
        //                m_children[index + 1] = a;
        //                index = 0;
        //                continue;
        //            }
        //            index++;
        //        }
        //    }
        //    finally
        //    {
        //        m_childrenRWLock.ReleaseWriterLock();
        //        m_needsSort = false;
        //    }
        //}

		private void UpdateRenderable(RenderableObject oldRenderable, RenderableObject newRenderable)
		{
			if(oldRenderable is Icon && newRenderable is Icon)
			{
				Icon oldIcon = (Icon)oldRenderable;
				Icon newIcon = (Icon)newRenderable;

				oldIcon.SetPosition( (float)newIcon.Latitude, (float)newIcon.Longitude, (float)newIcon.Altitude);
			}
			else if(oldRenderable is RenderableObjectList && newRenderable is RenderableObjectList)
			{
				RenderableObjectList oldList = (RenderableObjectList)oldRenderable;
				RenderableObjectList newList = (RenderableObjectList)newRenderable;

				compareRefreshLists(newList, oldList);
			}
		}

		private void compareRefreshLists(RenderableObjectList newList, RenderableObjectList curList)
		{
			ArrayList addList = new ArrayList();
			ArrayList delList = new ArrayList();

			foreach(RenderableObject newObject in newList.ChildObjects)
			{
				bool foundObject = false;
				foreach(RenderableObject curObject in curList.ChildObjects)
				{
					string xmlSource = curObject.MetaData["XmlSource"] as string;
						
					if(xmlSource != null && xmlSource == m_DataSource && newObject.Name == curObject.Name)
					{
						foundObject = true;
						UpdateRenderable(curObject, newObject);
						break;
					}
				}

				if(!foundObject)
				{
					addList.Add(newObject);
				}
			}

			foreach(RenderableObject curObject in curList.ChildObjects)
			{
				bool foundObject = false;
				foreach(RenderableObject newObject in newList.ChildObjects)
				{
					string xmlSource = newObject.MetaData["XmlSource"] as string;
					if(xmlSource != null && xmlSource == m_DataSource && newObject.Name == curObject.Name)
					{
						foundObject = true;
						break;
					}
				}

				if(!foundObject)
				{
					string src = (string)curObject.MetaData["XmlSource"];

					if(src != null || src == m_DataSource)
						delList.Add(curObject);
				}
			}

			foreach(RenderableObject o in addList)
			{
				curList.Add(o);
			}

			foreach(RenderableObject o in delList)
			{
				curList.Remove(o);
			}
		}

		bool hasSkippedFirstRefresh = false;

		private void m_RefreshTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if(!hasSkippedFirstRefresh)
			{
				hasSkippedFirstRefresh = true;
				return;
			}

			try
			{
				string dataSource = m_DataSource;

				RenderableObjectList newList = ConfigurationLoader.getRenderableFromLayerFile(dataSource, m_ParentWorld, m_Cache, false);

				if(newList != null)
				{
					compareRefreshLists(newList, this);
				}
			}
			catch(Exception ex)
			{
				Log.Write(ex);
			}
		}
    }
}
