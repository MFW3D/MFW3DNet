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

        #region ��ȡ����
        public RenderableObjectList DstileLayers
        {
            get{return dstileLayers;}
        }
        #endregion

        public override void Load()
        {
            //��ק�¼�
            Global.worldWindow.DragEnter += new DragEventHandler(WorldWindow_DragEnter);
            Global.worldWindow.DragDrop += new DragEventHandler(WorldWindow_DragDrop);
            
            int mergeOrder = 0;
            //��Ӷ�Ӧ��ͼ��
            dstileLayers = new RenderableObjectList("Dstile Layers");

            //����GUI�ͱ���
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

        #region ��ק����
        /// <summary>
        /// ����Ƿ���kml��kmz�ļ�
        /// </summary>
        private void WorldWindow_DragEnter(object sender, DragEventArgs e)
        {
            if (DragDropIsValid(e))
                e.Effect = DragDropEffects.All;
        }

        /// <summary>
        /// ���� kml/kmz�ļ��������ļ�
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
        /// ȷ���˲���Ƿ���Դ�����ɾ������
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
