using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using MFW3D;
using MFW3D.Renderable;

namespace MFW3D.CMPlugins.ExternalLayerManager
{
	/// <summary>
	/// Summary description for ExternalLayerManager.
	/// </summary>
	public class ExternalLayerManager : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		MenuItem parentMenuItem = null;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.PropertyGrid propertyGrid1;
		WorldWindow m_WorldWindow = null;
        private TreeNode selectedNode;

		System.Timers.Timer m_UpdateTimer = new System.Timers.Timer(500);
        private Button nodeUpButton;
        private Button nodeDownButton;

		System.Collections.Hashtable m_NodeHash = new Hashtable();

		public ExternalLayerManager(WorldWindow ww, MenuItem menuItem)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			parentMenuItem = menuItem;

			m_WorldWindow = ww;

			
			
			m_UpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_UpdateTimer_Elapsed);
			m_UpdateTimer.Start();
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			m_UpdateTimer.Stop();
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			
			base.Dispose( disposing );
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			Dispose(true);
			parentMenuItem.Checked = false;
			base.OnClosing (e);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            this.nodeUpButton = new System.Windows.Forms.Button();
            this.nodeDownButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.treeView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView1.CheckBoxes = true;
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.treeView1.ForeColor = System.Drawing.Color.White;
            this.treeView1.LineColor = System.Drawing.Color.White;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(254, 529);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // splitter1
            // 
            this.splitter1.Location = new System.Drawing.Point(254, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(3, 529);
            this.splitter1.TabIndex = 1;
            this.splitter1.TabStop = false;
            // 
            // propertyGrid1
            // 
            this.propertyGrid1.CommandsForeColor = System.Drawing.Color.White;
            this.propertyGrid1.CommandsLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.propertyGrid1.HelpBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.propertyGrid1.HelpForeColor = System.Drawing.Color.Snow;
            this.propertyGrid1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.propertyGrid1.Location = new System.Drawing.Point(260, 12);
            this.propertyGrid1.Name = "propertyGrid1";
            this.propertyGrid1.Size = new System.Drawing.Size(358, 331);
            this.propertyGrid1.TabIndex = 2;
            this.propertyGrid1.ToolbarVisible = false;
            this.propertyGrid1.ViewBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.propertyGrid1.ViewForeColor = System.Drawing.Color.White;
            this.propertyGrid1.Click += new System.EventHandler(this.propertyGrid1_Click);
            // 
            // nodeUpButton
            // 
            this.nodeUpButton.Location = new System.Drawing.Point(392, 349);
            this.nodeUpButton.Name = "nodeUpButton";
            this.nodeUpButton.Size = new System.Drawing.Size(110, 23);
            this.nodeUpButton.TabIndex = 3;
            this.nodeUpButton.Text = "Move Layer Up";
            this.nodeUpButton.UseVisualStyleBackColor = true;
            this.nodeUpButton.Click += new System.EventHandler(this.nodeUpButton_Click);
            // 
            // nodeDownButton
            // 
            this.nodeDownButton.Location = new System.Drawing.Point(508, 349);
            this.nodeDownButton.Name = "nodeDownButton";
            this.nodeDownButton.Size = new System.Drawing.Size(110, 23);
            this.nodeDownButton.TabIndex = 4;
            this.nodeDownButton.Text = "Move Layer Down";
            this.nodeDownButton.UseVisualStyleBackColor = true;
            this.nodeDownButton.Click += new System.EventHandler(this.nodeDownButton_Click);
            // 
            // ExternalLayerManager
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(111)))), ((int)(((byte)(111)))), ((int)(((byte)(111)))));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(630, 529);
            this.Controls.Add(this.nodeDownButton);
            this.Controls.Add(this.nodeUpButton);
            this.Controls.Add(this.propertyGrid1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.treeView1);
            this.DoubleBuffered = true;
            this.ForeColor = System.Drawing.Color.Snow;
            this.Name = "ExternalLayerManager";
            this.Text = "ExternalLayerManager";
            this.TopMost = true;
            this.ResumeLayout(false);

		}
		#endregion

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if(e.Node != null && e.Node.Tag != null)
			{
				RenderableObjectInfo roi = (RenderableObjectInfo)e.Node.Tag;
				this.propertyGrid1.SelectedObject = roi.Renderable;
                this.selectedNode = e.Node;
			}
		}

		private string GetAbsoluteRenderableObjectPath(MFW3D.Renderable.RenderableObject ro)
		{
			if(ro.ParentList != null)
			{
				return GetAbsoluteRenderableObjectPath(ro.ParentList) + "//" + ro.Name;
			}
			else
			{
				return ro.Name;
			}
		}

		private string GetAbsoluteTreeNodePath(TreeNode tn)
		{
			if(tn.Parent != null)
			{
				return GetAbsoluteTreeNodePath(tn.Parent) + "//" + tn.Text;
			}
			else
			{
				return tn.Text;
			}
		}

		private void updateNode(TreeNode tn)
		{

			RenderableObjectInfo roi = (RenderableObjectInfo)tn.Tag;

			roi.LastSpotted = System.DateTime.Now;
			if(tn.Checked != roi.Renderable.IsOn)
			{
				tn.Checked = roi.Renderable.IsOn;
				//treeView1.BeginInvoke(new UpdateCheckStateNodeDelegate(this.UpdateCheckStateNode), new object[] {tn, roi.Renderable.IsOn});
			}

			if(roi.Renderable is MFW3D.Renderable.RenderableObjectList)
			{
				MFW3D.Renderable.RenderableObjectList rol = (MFW3D.Renderable.RenderableObjectList)roi.Renderable;
				for(int i = 0; i < rol.Count; i++)
				{
					MFW3D.Renderable.RenderableObject childRo = (MFW3D.Renderable.RenderableObject)rol.ChildObjects[i];
					string absolutePath = GetAbsoluteRenderableObjectPath(childRo);

					TreeNode correctNode = (TreeNode)m_NodeHash[absolutePath];
					if(correctNode == null)
					{
						correctNode = new TreeNode(childRo.Name);
						RenderableObjectInfo curRoi = new RenderableObjectInfo();
						curRoi.Renderable = childRo;
						correctNode.Tag = curRoi;

						m_NodeHash.Add(absolutePath, correctNode);
						treeView1.BeginInvoke(new UpdateChildNodeDelegate(this.UpdateChildNodeTree), new object[] {tn, correctNode});
					}

					updateNode(correctNode);
					
				}
			}
		}

		private void m_UpdateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			try
			{
				System.DateTime updateStart = System.DateTime.Now;

				for(int i = 0; i < m_WorldWindow.CurrentWorld.RenderableObjects.Count; i++)
				{
					MFW3D.Renderable.RenderableObject curRo = (MFW3D.Renderable.RenderableObject)m_WorldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];
					if(i >= this.treeView1.Nodes.Count)
					{
						// Add a node
						TreeNode correctNode = new TreeNode(curRo.Name);
						RenderableObjectInfo curRoi = new RenderableObjectInfo();
						curRoi.Renderable = curRo;
						correctNode.Tag = curRoi;

						m_NodeHash.Add(correctNode.Text, correctNode);
                        this.treeView1.BeginInvoke(new AddTableTreeDelegate(this.AddTableTree), new object[] { correctNode });
						updateNode(correctNode);
					}
					else
					{
						//compare nodes
						TreeNode curTn = this.treeView1.Nodes[i];

						RenderableObjectInfo curRoi = (RenderableObjectInfo)curTn.Tag;
						if(curRoi.Renderable != null && curRoi.Renderable.Name == curRo.Name)
						{
							updateNode(curTn);
							continue;
						}
						else
						{
							if(!m_NodeHash.Contains(curRo.Name))
							{	
								//add it
								curRoi = new RenderableObjectInfo();
								curRoi.Renderable = curRo;
								curTn = new TreeNode(curRo.Name);
								curTn.Tag = curRoi;

								m_NodeHash.Add(curTn.Text, curTn);
								this.treeView1.BeginInvoke(new InsertTableTreeDelegate(this.InsertTableTree), new object[] {i, curTn});
							}
							else
							{
								curTn = (TreeNode)m_NodeHash[curRo.Name];
								try
								{
                                    treeView1.BeginInvoke(new RemoveTableTreeDelegate(this.RemoveTableTree), new object[] { curTn });
								}
								catch
								{}
							
								treeView1.BeginInvoke(new InsertTableTreeDelegate(this.InsertTableTree), new object[] {i, curTn});
							}
						}

						updateNode(curTn);
					}
				
				}

				for(int i = m_WorldWindow.CurrentWorld.RenderableObjects.Count; i < this.treeView1.Nodes.Count; i++)
				{
					this.treeView1.BeginInvoke(new RemoveAtTableTreeDelegate(this.RemoveAtTableTree), new object[] {i});
				}

				System.Collections.ArrayList deletionList = new ArrayList();
				foreach(TreeNode tn in m_NodeHash.Values)
				{
					RenderableObjectInfo roi = (RenderableObjectInfo)tn.Tag;
					if(roi == null || roi.Renderable == null || roi.LastSpotted < updateStart)
					{
						deletionList.Add(GetAbsoluteRenderableObjectPath(roi.Renderable));
					}
				}

				foreach(string key in deletionList)
				{
					m_NodeHash.Remove(key);
				}
			}
			catch
			{}
		}

		
		delegate 
			void UpdateChildNodeDelegate(TreeNode parent, TreeNode child);

		private void UpdateChildNodeTree(TreeNode parent, TreeNode child)
		{
			try
			{
				parent.Nodes.Add(child);
			}
			catch{}
		}


		delegate 
			void AddTableTreeDelegate(TreeNode tn);

		private void AddTableTree(TreeNode tn)
		{
			try
			{
				this.treeView1.Nodes.Add(tn);
			}
			catch{}
		}

		delegate 
			void InsertTableTreeDelegate(int index, TreeNode tn);

		private void InsertTableTree(int index, TreeNode tn)
		{
			try
			{
				this.treeView1.Nodes.Insert(index, tn);
			}
			catch{}
		}

		delegate 
			void ReplaceTableTreeDelegate(int index, TreeNode tn);

		private void ReplaceTableTree(int index, TreeNode tn)
		{
			try
			{
				this.treeView1.Nodes[index] = tn;
			}
			catch{}
		}

		delegate 
			void RemoveAtTableTreeDelegate(int index);

		private void RemoveAtTableTree(int index)
		{
			try
			{
				this.treeView1.Nodes.RemoveAt(index);
			}
			catch{}
		}

		delegate 
			void RemoveTableTreeDelegate(TreeNode tn);

		private void RemoveTableTree(TreeNode tn)
		{
			try
			{
				this.treeView1.Nodes.Remove(tn);
			}
			catch{}
		}

		private void treeView1_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			RenderableObjectInfo roi = (RenderableObjectInfo)e.Node.Tag;
			if(roi != null)
			{
				roi.Renderable.IsOn = e.Node.Checked;
			}
		}

		class RenderableObjectInfo
		{
			public DateTime LastSpotted = System.DateTime.Now;
			public MFW3D.Renderable.RenderableObject Renderable = null;

		}

        /// <summary>
        /// Layer selected for GetFeature request.
        /// Returns a QTS if the selected items is a WMS layer, otherwise returns null.
        /// </summary>
        public QuadTileSet KeyLayer
        {
            get
            {
                RenderableObjectInfo roi = (RenderableObjectInfo)treeView1.SelectedNode.Tag;

                if (!(roi.Renderable is QuadTileSet))
                    return null;

                QuadTileSet qts = (QuadTileSet)roi.Renderable;

                if (!(qts.ImageStores[0] is MFW3D.Net.Wms.WmsImageStore))
                    return null;

                return qts;
            }
        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void nodeUpButton_Click(object sender, EventArgs e)
        {
            if (selectedNode != null && selectedNode.Index != 0)
            {
                RenderableObjectInfo roi = (RenderableObjectInfo)treeView1.SelectedNode.Tag;
                TreeNode parentNode = selectedNode.Parent;
                TreeNode aboveNode = parentNode.Nodes[selectedNode.Index - 1];
                TreeNode cloneNode = (TreeNode)selectedNode.Clone();
                int index = selectedNode.Index;
                parentNode.Nodes.RemoveAt(index);
                parentNode.Nodes.RemoveAt(index - 1);
                parentNode.Nodes.Insert(index - 1, cloneNode);
                parentNode.Nodes.Insert(index, aboveNode);
                
                //Propagate to base tree
                RenderableObject selectedRenderable = roi.Renderable;
                RenderableObjectList parentList = selectedRenderable.ParentList;
                if (parentList != null)
                {
                    int indexRender = parentList.ChildObjects.IndexOf(selectedRenderable);
                    RenderableObject clonedRenderable = selectedRenderable;
                    RenderableObject aboveRenderable = (RenderableObject)parentList.ChildObjects[index - 1];
                    //Need to implement RemoveAt and InsertAt
                    parentList.ChildObjects.RemoveAt(indexRender);
                    parentList.ChildObjects.RemoveAt(indexRender - 1);
                    parentList.ChildObjects.Insert(indexRender - 1, clonedRenderable);
                    parentList.ChildObjects.Insert(indexRender, aboveRenderable);
                }
            }
        }

        private void nodeDownButton_Click(object sender, EventArgs e)
        {
            if (selectedNode != null && selectedNode != selectedNode.Parent.LastNode)
            {
                RenderableObjectInfo roi = (RenderableObjectInfo)treeView1.SelectedNode.Tag;
                TreeNode parentNode = selectedNode.Parent;
                TreeNode belowNode = parentNode.Nodes[selectedNode.Index + 1];
                TreeNode cloneNode = (TreeNode)selectedNode.Clone();
                int index = selectedNode.Index;
                parentNode.Nodes.RemoveAt(index + 1);
                parentNode.Nodes.RemoveAt(index);
                parentNode.Nodes.Insert(index, belowNode);
                parentNode.Nodes.Insert(index + 1, cloneNode);
                
                //Propagate to base tree
                RenderableObject selectedRenderable = roi.Renderable;
                RenderableObjectList parentList = selectedRenderable.ParentList;
                if (parentList != null)
                {
                    int indexRender = parentList.ChildObjects.IndexOf(selectedRenderable);
                    RenderableObject clonedRenderable = selectedRenderable;
                    RenderableObject belowRenderable = (RenderableObject)parentList.ChildObjects[index + 1];
                    //Need to implement RemoveAt and InsertAt
                    parentList.ChildObjects.RemoveAt(indexRender + 1);
                    parentList.ChildObjects.RemoveAt(indexRender);
                    parentList.ChildObjects.Insert(indexRender, belowRenderable);
                    parentList.ChildObjects.Insert(indexRender + 1, clonedRenderable);
                    
                }
            }
        }
	}

	public class ExternalLayerManagerLoader : MFW3D.PluginEngine.Plugin
	{
		MenuItem m_MenuItem;
		ExternalLayerManager m_Form = null;
        DrawArgs drawArgs;
        int queryBoxOffset = 200;

		/// <summary>
		/// Plugin entry point 
		/// </summary>
		public override void Load() 
		{
            Global.worldWindow.MouseUp += new MouseEventHandler(WorldWindow_MouseUp);
            drawArgs = Global.worldWindow.DrawArgs;
		}

        void WorldWindow_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right)
                return;

            if (m_Form == null)
                return;

            if (!m_Form.Visible)
                return;

            if (m_Form.KeyLayer == null)
                return;

            QuadTileSet qts = m_Form.KeyLayer;

            MFW3D.Net.Wms.WmsImageStore wms = (MFW3D.Net.Wms.WmsImageStore)qts.ImageStores[0];

            string requestUrl = wms.ServerGetMapUrl;
            string[] layerNameParts = wms.WMSLayerName.Split('&');
            string layerName = layerNameParts[0];
            string getMapParts = 
            requestUrl += "?QUERY_LAYERS=" + layerName + "&SERVICE=WMS&VERSION=" + wms.Version + "&REQUEST=GetFeatureInfo&SRS=EPSG:4326&FEATURE_COUNT=10&INFO_FORMAT=text/plain&EXCEPTIONS=text/plain";

            // From Servir-Viz...
            Angle LowerLeftX;
            Angle LowerLeftY;
            Angle UpperRightX;
            Angle UpperRightY;
            string minx, miny, maxx, maxy;
            char[] degreeChar = { '?'};

            int mouseX = e.X;
            int mouseY = e.Y;

            int queryBoxWidth = 2 * queryBoxOffset + 1;
            int queryBoxHeight = 2 * queryBoxOffset + 1;

            int queryX = queryBoxOffset + 1;
            int queryY = queryBoxOffset + 1;

            drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(mouseX - queryBoxOffset, mouseY + queryBoxOffset, out LowerLeftY, out LowerLeftX, drawArgs.CurrentWorld);
            drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(mouseX + queryBoxOffset, mouseY - queryBoxOffset, out UpperRightY, out UpperRightX, drawArgs.CurrentWorld);

            //drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(0, 0 + drawArgs.screenHeight, out LowerLeftY, out LowerLeftX, drawArgs.CurrentWorld);
            //drawArgs.WorldCamera.PickingRayIntersectionWithTerrain(drawArgs.screenWidth, 0, out UpperRightY, out UpperRightX, drawArgs.CurrentWorld);

            minx = LowerLeftX.ToString().Contains("NaN") ? "-180.0" : LowerLeftX.ToString().TrimEnd(degreeChar);
            miny = LowerLeftY.ToString().Contains("NaN") ? "-90.0" : LowerLeftY.ToString().TrimEnd(degreeChar);
            maxx = UpperRightX.ToString().Contains("NaN") ? "180.0" : UpperRightX.ToString().TrimEnd(degreeChar);
            maxy = UpperRightY.ToString().Contains("NaN") ? "90.0" : UpperRightY.ToString().TrimEnd(degreeChar);

            // request has to include a bbox and the requested pixel coords relative to that box...
            requestUrl += "&layers=" + wms.WMSLayerName;
            requestUrl += "&WIDTH=" + queryBoxWidth.ToString() + "&HEIGHT=" + queryBoxHeight.ToString();
            requestUrl += "&BBOX=" + minx + "," + miny + "," + maxx + "," + maxy;
            requestUrl += "&X=" + queryX.ToString() + "&Y=" + queryY.ToString();

            if (!World.Settings.WorkOffline)
            {
                MFW3D.Net.WebDownload dl = new MFW3D.Net.WebDownload(requestUrl);
                System.IO.FileInfo fi = new System.IO.FileInfo("GetFeatureInfo_response.txt");
                dl.DownloadFile(fi.FullName);
                //dl.SavedFilePath = fi.FullName;
                //dl.BackgroundDownloadFile(GetFeatureDlComplete);


                if (World.Settings.UseInternalBrowser)
                {
                    SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                    InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                    browser.NavigateTo(fi.FullName);
                }
                else
                {
                    System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                    psi.FileName = fi.FullName;
                    psi.Verb = "open";
                    psi.UseShellExecute = true;

                    psi.CreateNoWindow = true;
                    System.Diagnostics.Process.Start(psi);
                }
            }
        }

        private void GetFeatureDlComplete(MFW3D.Net.WebDownload dl)
        {

            if (World.Settings.UseInternalBrowser)
            {
                SplitContainer sc = (SplitContainer)drawArgs.parentControl.Parent.Parent;
                InternalWebBrowserPanel browser = (InternalWebBrowserPanel)sc.Panel1.Controls[0];
                browser.NavigateTo(dl.SavedFilePath);
            }
            else
            {
                System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
                psi.FileName = dl.SavedFilePath;
                psi.Verb = "open";
                psi.UseShellExecute = true;

                psi.CreateNoWindow = true;
                System.Diagnostics.Process.Start(psi);
            }
        }
		/// <summary>
		/// Unload our plugin
		/// </summary>
		public override void Unload() 
		{
			if(m_MenuItem!=null)
			{
				m_MenuItem.Dispose();
				m_MenuItem = null;
			}

			if(m_Form != null)
			{
				m_Form.Dispose();
				m_Form = null;
			}
		}
	
		void menuItemClicked(object sender, EventArgs e)
		{
			if(m_Form != null && m_Form.Visible)
			{
				m_Form.Visible = false;
				m_Form.Dispose();
				m_Form = null;
				
				m_MenuItem.Checked = false;
			}
			else
			{
				m_Form = new ExternalLayerManager(Global.worldWindow, m_MenuItem);
				m_Form.Visible = true;
				m_MenuItem.Checked = true;
			}
		}
	}
}
