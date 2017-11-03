//----------------------------------------------------------------------------
// NAME: Terrain Manager Plug-in
// VERSION: 0.1
// DESCRIPTION: Allows the integration and switching on and off of terrain layers  
// DEVELOPER: Isaac Mann
// WEBSITE: http:\\www.apogee.com.au
// REFERENCES: 
//----------------------------------------------------------------------------
//
// Plugin was developed by Apogee Imaging International
// This file is in the Public Domain, and comes with no warranty. 

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Timers;
using System.Windows.Forms;

using WorldWind.Terrain;

namespace WorldWind
{
    public partial class TerrainManagerForm : Form
    {
        private Hashtable m_NodeHash = new Hashtable();
        private MenuItem parentMenuItem;
        private TreeNode selectedNode;
        private System.Timers.Timer m_updateTimer = new System.Timers.Timer(5000);
        private WorldWindow m_worldWindow;
        
        private bool rootNodeDone = false;

        public TerrainManagerForm(WorldWindow ww, MenuItem menuItem)
        {
            InitializeComponent();
            parentMenuItem = menuItem;
            m_worldWindow = ww;
            m_updateTimer.Elapsed += new ElapsedEventHandler(m_updateTimerElapsed);
            m_updateTimer.Start();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            m_updateTimer.Stop();
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        private void AddRootNode()
        {
            TerrainAccessorInfo tai = new TerrainAccessorInfo();
            tai.Accessor = m_worldWindow.CurrentWorld.TerrainAccessor;
            TreeNode node = new TreeNode(tai.Accessor.Name);
            node.Tag = tai;
            node.Checked = tai.Accessor.IsOn;
            m_NodeHash.Add(node.Text, node);
            this.treeView1.BeginInvoke(new AddTableRootTreeDelegate(
                this.AddTableRootTree), new object[] { node });
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
            Dispose(true);
            parentMenuItem.Checked = false;
            base.OnClosing(e);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag != null)
            {
                TerrainAccessorInfo taInfo = (TerrainAccessorInfo)e.Node.Tag;
                this.propertyGrid1.SelectedObject = taInfo.Accessor;
                selectedNode = e.Node;
            }
        }

        private string GetAbsoluteTreeNodePath(TreeNode tn)
        {
            if (tn.Parent != null)
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
            TerrainAccessorInfo taInfo = (TerrainAccessorInfo)tn.Tag;
            taInfo.LastSpotted = DateTime.Now;
            if (tn.Checked != taInfo.Accessor.IsOn)
            {
                tn.Checked = taInfo.Accessor.IsOn;
            }

            TerrainAccessor ta = (TerrainAccessor)taInfo.Accessor;
            TreeNode correctNode = (TreeNode)m_NodeHash[ta.Name];
            if (correctNode == null)
            {
                correctNode = new TreeNode(ta.Name);
                TerrainAccessorInfo currentTai = new TerrainAccessorInfo();
                currentTai.Accessor = ta;
                correctNode.Tag = currentTai;
                m_NodeHash.Add(ta.Name, correctNode);
                treeView1.BeginInvoke(new UpdateChildNodeDelegate(this.UpdateChildNodeTree),
                    new object[] { tn, correctNode });
            }
        }

        private void m_updateTimerElapsed(object sneder, ElapsedEventArgs e)
        {
            try
            {
                if (!rootNodeDone)
                {
                    AddRootNode();
                    rootNodeDone = true;
                }
                DateTime updateStart = DateTime.Now;
                for (int i = 0; i < m_worldWindow.CurrentWorld.TerrainAccessor.SubsetCount; i++)
                {
                    TerrainAccessor currentTA = (TerrainAccessor)m_worldWindow.
                        CurrentWorld.TerrainAccessor.HighResSubsets[i];
                    if (i >= this.treeView1.Nodes.Count)
                    {
                        // More than what is currently in the system, add a node
                        TreeNode correctNode = new TreeNode(currentTA.Name);
                        TerrainAccessorInfo taInfo = new TerrainAccessorInfo();
                        taInfo.Accessor = currentTA;
                        correctNode.Tag = taInfo;

                        m_NodeHash.Add(correctNode.Text, correctNode);
                        this.treeView1.BeginInvoke(new AddTableTreeDelegate(this.AddTableTree), 
                            new object[] { correctNode });

                        updateNode(correctNode);
                    }
                    else
                    {
                        // Compare and update nodes
                        TreeNode currentTn = this.treeView1.Nodes[i];
                        TerrainAccessorInfo taInfo = (TerrainAccessorInfo)currentTn.Tag;
                        if (taInfo.Accessor != null && taInfo.Accessor.Name == currentTA.Name)
                        {
                            updateNode(currentTn);
                            continue;
                        }
                        else
                        {
                            if (!m_NodeHash.Contains(currentTA.Name))
                            {
                                // add it
                                taInfo = new TerrainAccessorInfo();
                                taInfo.Accessor = currentTA;
                                currentTn = new TreeNode(currentTA.Name);
                                currentTn.Tag = taInfo;

                                m_NodeHash.Add(currentTn.Text, currentTn);
                                this.treeView1.BeginInvoke(new InsertTableTreeDelegate(
                                    this.InsertTableTree), new object[] { i, currentTn });
                            }
                            else
                            {
                                currentTn = (TreeNode)m_NodeHash[currentTA.Name];
                                try
                                {
                                    treeView1.BeginInvoke(new RemoveTableTreeDelegate(
                                        this.RemoveTableTree), new object[] { currentTn });
                                }
                                catch
                                { }
                                treeView1.BeginInvoke(new InsertTableTreeDelegate(
                                    this.InsertTableTree), new object[] { i, currentTn });
                            }
                        }
                        updateNode(currentTn);
                    }
                }

                for (int i = m_worldWindow.CurrentWorld.TerrainAccessor.HighResSubsets.Length;
                    i < this.treeView1.Nodes.Count; i++)
                {
                    this.treeView1.BeginInvoke(new RemoveAtTableTreeDelegate(
                        this.RemoveAtTableTree),new object[] { i });
                }

                ArrayList deletionList = new ArrayList();
                foreach (TreeNode tn in m_NodeHash.Values)
                {
                    TerrainAccessorInfo taInfo = (TerrainAccessorInfo)tn.Tag;
                    if (taInfo == null || taInfo.Accessor == null ||
                            taInfo.LastSpotted < updateStart)
                    {
                        deletionList.Add(taInfo.Accessor.Name);
                    } 
                }

                foreach (string key in deletionList)
                    m_NodeHash.Remove(key);


                // Compare the nodes to the previously selected node
                // and restore the selection to the same node
                treeView1.BeginInvoke(new SetPreviousNodeDelegate(
                    this.SetPreviousNode), new object[] { selectedNode });
            }
            catch 
            { }
        }

        delegate
            void SetPreviousNodeDelegate(TreeNode tn);

        private void SetPreviousNode(TreeNode tn)
        {
            try
            {
                if (treeView1.Nodes[0] == tn)
                    treeView1.SelectedNode = treeView1.Nodes[0];
                foreach (TreeNode n in treeView1.Nodes[0].Nodes)
                    if (n == tn)
                        treeView1.SelectedNode = n;
            }
            catch { }
        }

        delegate
            void UpdateChildNodeDelegate(TreeNode parent, TreeNode child);

        private void UpdateChildNodeTree(TreeNode parent, TreeNode child)
        {
            try
            {
                parent.Nodes[0].Nodes.Add(child);
            }
            catch { }
        }

        delegate
            void AddTableRootTreeDelegate(TreeNode tn);

        private void AddTableRootTree(TreeNode tn)
        {
            try
            {
                this.treeView1.Nodes.Add(tn);
            }
            catch { }
        }

        delegate
            void AddTableTreeDelegate(TreeNode tn);

        private void AddTableTree(TreeNode tn)
        {
            try
            {
                this.treeView1.Nodes[0].Nodes.Add(tn);
            }
            catch { }
        }

        delegate
            void InsertTableTreeDelegate(int index, TreeNode tn);

        private void InsertTableTree(int index, TreeNode tn)
        {
            try
            {
                this.treeView1.Nodes[0].Nodes.Insert(index, tn);
            }
            catch { }
        }

        delegate
            void ReplaceTableTreeDelegate(int index, TreeNode tn);

        private void ReplaceTableTree(int index, TreeNode tn)
        {
            try
            {
                this.treeView1.Nodes[0].Nodes[index] = tn;
            }
            catch { }
        }

        delegate
            void RemoveAtTableTreeDelegate(int index);

        private void RemoveAtTableTree(int index)
        {
            try
            {
                this.treeView1.Nodes[0].Nodes.RemoveAt(index);
            }
            catch { }
        }

        delegate
            void RemoveTableTreeDelegate(TreeNode tn);

        private void RemoveTableTree(TreeNode tn)
        {
            try
            {
                this.treeView1.Nodes[0].Nodes.Remove(tn);
            }
            catch { }
        }

        private void treeView1_AfterCheck(object sender, System.Windows.Forms.TreeViewEventArgs e)
        {
            TerrainAccessorInfo taInfo = (TerrainAccessorInfo)e.Node.Tag;
            if (taInfo != null)
            {
                taInfo.Accessor.IsOn = e.Node.Checked;
                this.m_worldWindow.Invalidate();
            }
        }

        class TerrainAccessorInfo
        {
            public DateTime LastSpotted = System.DateTime.Now;
            public WorldWind.Terrain.TerrainAccessor Accessor = null;

        }

        private void alwaysOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (alwaysOnTopToolStripMenuItem.Checked == false)
            {
                this.TopMost = true;
                alwaysOnTopToolStripMenuItem.Checked = true;
            }
            else
            {
                this.TopMost = false;
                alwaysOnTopToolStripMenuItem.Checked = false;
            }
        }
    }
    
    public class TerrainManager : WorldWind.PluginEngine.Plugin
    {
        private MenuItem menuItem;
        private TerrainManagerForm terrainManagerForm;

        public override void Load()
        {
            menuItem = new MenuItem("Terrain Manager");
            menuItem.Click += new EventHandler(menuItem_Click);
            base.Load();
        }

        public override void Unload()
        {
            base.Unload();
        }

        public void menuItem_Click(object sender, EventArgs e)
        {
            if (terrainManagerForm != null && terrainManagerForm.Visible)
            {
                // Already open
                terrainManagerForm.Hide();
                menuItem.Checked = false;
            }
            else
            {
                menuItem.Checked = true;
                terrainManagerForm = new TerrainManagerForm(Application.WorldWindow, menuItem);
                terrainManagerForm.Show();
            }
        }
    }
}