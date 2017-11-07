using System;
using System.Collections.Generic;
using System.Text;

using WorldWind;
using WorldWind.NewWidgets;

namespace NASA.Plugins
{
    public class LayerManager : WorldWind.PluginEngine.Plugin
    {
        FormWidget m_layerManagerForm = null;
        SimpleTreeNodeWidget m_activeLayersNode = null;
        SimpleTreeNodeWidget m_allLayersNode = null;
        System.Timers.Timer m_updateTimer = null;
        
        public override void Load()
        {
            try
            {
                m_layerManagerForm = new FormWidget("Layer Manager");
                m_layerManagerForm.Location = new System.Drawing.Point(150, 150);

                SimpleTreeNodeWidget tnw = new SimpleTreeNodeWidget("Layer Manager");
                tnw.Expanded = true;
                tnw.ParentWidget = m_layerManagerForm;

                m_activeLayersNode = new SimpleTreeNodeWidget("Active Layers");
                m_activeLayersNode.ParentWidget = tnw;
                m_allLayersNode = new SimpleTreeNodeWidget("All Layers");
                m_allLayersNode.ParentWidget = tnw;

                tnw.ChildWidgets.Add(m_activeLayersNode);
                tnw.ChildWidgets.Add(m_allLayersNode);

                m_layerManagerForm.ChildWidgets.Add(tnw);
                //m_layerManagerForm.ChildWidgets.Add(m_activeLayersNode);
                //m_layerManagerForm.ChildWidgets.Add(m_allLayersNode);
                
                DrawArgs.NewRootWidget.ChildWidgets.Add(m_layerManagerForm);

                m_updateTimer = new System.Timers.Timer(100);
                m_updateTimer.Elapsed += new System.Timers.ElapsedEventHandler(m_updateTimer_Elapsed);
                m_updateTimer.Start();
            
            }
            catch (Exception ex)
            {
                Utility.Log.Write(ex);
            }
            base.Load();
        }

        void m_updateTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            List<WorldWind.Renderable.RenderableObject> activeList = new List<WorldWind.Renderable.RenderableObject>();
            
            for(int i = 0; i < Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects.Count; i++)
            {
                WorldWind.Renderable.RenderableObject renderable = (WorldWind.Renderable.RenderableObject)Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];

                List<WorldWind.Renderable.RenderableObject> childActiveList = getActiveLayers(renderable);
                for (int j = 0; j < childActiveList.Count; j++)
                {
                    activeList.Add(childActiveList[j]);
                }
            }

            for (int i = 0; i < activeList.Count; i++)
            {

                if (m_activeLayersNode.ChildWidgets.Count == i)
                {
                    SimpleTreeNodeWidget node = new SimpleTreeNodeWidget(activeList[i].Name);
                    node.Tag = activeList[i];
                    node.ParentWidget = m_activeLayersNode;
                    node.OnCheckStateChanged += new CheckStateChangedHandler(node_OnCheckStateChanged);
                    m_activeLayersNode.ChildWidgets.Add(node);
                }
                else
                {
                    SimpleTreeNodeWidget currentNode = (SimpleTreeNodeWidget)m_activeLayersNode.ChildWidgets[i];
                    WorldWind.Renderable.RenderableObject nodeRenderable = (WorldWind.Renderable.RenderableObject)currentNode.Tag;

                    if (activeList[i] != nodeRenderable)
                    {
                        SimpleTreeNodeWidget node = new SimpleTreeNodeWidget(activeList[i].Name);
                        node.Tag = activeList[i];
                        node.ParentWidget = m_activeLayersNode;
                        node.OnCheckStateChanged += new CheckStateChangedHandler(node_OnCheckStateChanged);
                        m_activeLayersNode.ChildWidgets.Insert(node, i);
                    }
                }
            }

            if (m_activeLayersNode.ChildWidgets.Count != activeList.Count)
            {
                while (m_activeLayersNode.ChildWidgets.Count > activeList.Count)
                {
                    m_activeLayersNode.ChildWidgets.RemoveAt(m_activeLayersNode.ChildWidgets.Count - 1);
                }
            }

            for (int i = 0; i < Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects.Count; i++)
            {
                WorldWind.Renderable.RenderableObject renderable = (WorldWind.Renderable.RenderableObject)Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects[i];
                if (m_allLayersNode.ChildWidgets.Count == i)
                {
                    SimpleTreeNodeWidget childNode = new SimpleTreeNodeWidget(renderable.Name);
                    childNode.Tag = renderable;
                    childNode.ParentWidget = m_allLayersNode;
                    childNode.OnCheckStateChanged += new CheckStateChangedHandler(node_OnCheckStateChanged);
                    m_allLayersNode.Add(childNode);
                }

                UpdateAllLayers((SimpleTreeNodeWidget)m_allLayersNode.ChildWidgets[i], renderable);
            }

            while (m_allLayersNode.ChildWidgets.Count > Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects.Count)
            {
                m_allLayersNode.ChildWidgets.RemoveAt(Global.worldWindow.CurrentWorld.RenderableObjects.ChildObjects.Count - 1);
            }
        }

        static void node_OnCheckStateChanged(object o, bool state)
        {
            SimpleTreeNodeWidget node = (SimpleTreeNodeWidget)o;
            if (node != null && node.Tag != null && node.Tag is WorldWind.Renderable.RenderableObject)
            {
                WorldWind.Renderable.RenderableObject renderable = (WorldWind.Renderable.RenderableObject)node.Tag;
                renderable.IsOn = state;
            }
        }

        private static List<WorldWind.Renderable.RenderableObject> getActiveLayers(WorldWind.Renderable.RenderableObject renderable)
        {
            List<WorldWind.Renderable.RenderableObject> renderableList = new List<WorldWind.Renderable.RenderableObject>();

            if (renderable.IsOn)
            {
                if (renderable is WorldWind.Renderable.RenderableObjectList)
                {
                    WorldWind.Renderable.RenderableObjectList rol = (WorldWind.Renderable.RenderableObjectList)renderable;

                    for (int i = 0; i < rol.ChildObjects.Count; i++)
                    {
                        WorldWind.Renderable.RenderableObject childRenderable = (WorldWind.Renderable.RenderableObject)rol.ChildObjects[i];
                        List<WorldWind.Renderable.RenderableObject> childList = getActiveLayers(childRenderable);
                        if (childList != null && childList.Count > 0)
                        {
                            for (int j = 0; j < childList.Count; j++)
                            {
                                renderableList.Add(childList[j]);
                            }
                        }
                    }
                }
                else
                {
                    renderableList.Add(renderable);
                }
            }

            return renderableList;
        }

        private static void UpdateAllLayers(SimpleTreeNodeWidget node, WorldWind.Renderable.RenderableObject renderable)
        {
            WorldWind.Renderable.RenderableObject nodeRenderable = (WorldWind.Renderable.RenderableObject)node.Tag;
            if (nodeRenderable != renderable)
            {
                node.Name = renderable.Name;
                node.Tag = renderable;
            }

            if (node.Enabled != renderable.IsOn)
            {
                node.Enabled = renderable.IsOn;
            }

            if (renderable is WorldWind.Renderable.RenderableObjectList)
            {
                WorldWind.Renderable.RenderableObjectList rol = (WorldWind.Renderable.RenderableObjectList)renderable;

                for (int i = 0; i < rol.ChildObjects.Count; i++)
                {
                    WorldWind.Renderable.RenderableObject childRenderable = (WorldWind.Renderable.RenderableObject)rol.ChildObjects[i];
                    
                    if (node.ChildWidgets.Count == i)
                    {
                        SimpleTreeNodeWidget childNode = new SimpleTreeNodeWidget(childRenderable.Name);
                        childNode.Tag = childRenderable;
                        childNode.ParentWidget = node;
                        childNode.OnCheckStateChanged += new CheckStateChangedHandler(node_OnCheckStateChanged);

                        node.ChildWidgets.Add(childNode);
                    }

                    UpdateAllLayers((SimpleTreeNodeWidget)node.ChildWidgets[i], childRenderable);

                }

                while (node.ChildWidgets.Count > rol.ChildObjects.Count)
                {
                    rol.ChildObjects.RemoveAt(rol.ChildObjects.Count - 1);
                }
            }
            else if (node.ChildWidgets.Count > 0)
            {
                node.ChildWidgets.Clear();
            }
        }
    }
}
