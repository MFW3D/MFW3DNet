using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WorldWind;
using WorldWind.Renderable;

namespace DstileGUI
{
    public class DstilePlugin : WorldWind.PluginEngine.Plugin
    {
        private DstileFrontEnd frontend;
        private RenderableObjectList dstileLayers;

        #region 获取方法
        public RenderableObjectList DstileLayers
        {
            get{return dstileLayers;}
        }
        #endregion

        public override void Load()
        {
            //拖拽事件
            Global.worldWindow.DragEnter += new DragEventHandler(WorldWindow_DragEnter);
            Global.worldWindow.DragDrop += new DragEventHandler(WorldWindow_DragDrop);
            
            int mergeOrder = 0;
            //添加对应的图层
            dstileLayers = new RenderableObjectList("Dstile Layers");

            //创建GUI和背景
            frontend = new DstileFrontEnd(this);
            frontend.Visible = false;
            Global.worldWindow.CurrentWorld.RenderableObjects.Add(dstileLayers);
        }
        
        private void showFrontEnd()
        {
            if (frontend == null)
            {
                frontend = new DstileFrontEnd(this);
            }
            if (frontend.Visible == false)
            {
                frontend.Visible = true;
            }
        }

        public override void Unload()
        {
            
            //Dispose GUI
            if(frontend != null)
                frontend.Dispose();
            frontend = null;

            // Disable Drag&Drop functionality
            Global.worldWindow.DragEnter -= new DragEventHandler(WorldWindow_DragEnter);
            Global.worldWindow.DragDrop -= new DragEventHandler(WorldWindow_DragDrop);

            //TODO: Save if needed any present DSTile layers to
            //master xml
            //TODO: Remove Dstile added layers from Layer Manager
            Global.worldWindow.CurrentWorld.RenderableObjects.Remove(dstileLayers);
            dstileLayers.Dispose();
            dstileLayers = null;


            base.Unload();
        }

        #region 多拽方法
        /// <summary>
        /// 检查是否是kml或kmz文件
        /// </summary>
        private void WorldWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
                e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// 处理 kml/kmz文件来加载文件
        /// </summary>
        private void WorldWindow_DragDrop(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
            {
                // transfer the filenames to a string array
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && File.Exists(files[0]))
                {
                    showFrontEnd();
                    frontend.locateData(files[0]);
                }
                    
            }
        }

        /// <summary>
        /// 确定此插件是否可以处理已删除的项
        /// </summary>
        private static bool DragDropIsValid(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
            {
                if (((string[])e.Data.GetData(DataFormats.FileDrop)).Length == 1)
                {
                    string extension = Path.GetExtension(((string[])e.Data.GetData(DataFormats.FileDrop))[0]).ToLower(CultureInfo.InvariantCulture);
                    if ((extension == ".tiff") || (extension == ".tif") || (extension == ".img") || (extension == ".jpg") || (extension == ".sid"))
                        return true;
                }
            }
            return false;
        }
        #endregion

    }
}
