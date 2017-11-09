using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using MFW3D;
using MFW3D.Net;
using MFW3D.Renderable;

namespace MFW3D
{
	/// <summary>
	/// Summary description for DynamicClouds.
	/// </summary>
	public class DynamicClouds : MFW3D.PluginEngine.Plugin 
	{
		//RenderableObjectList m_wavingFlagsList = null;
		const string remoteImageUrl = "http://worldwind26.arc.nasa.gov/GlobalCloudsArchive/PastDay";
		const string archiveImageDirectoryPath = "Plugins\\DynamicClouds\\Images";
		DynamicCloudLayer m_dynamicCloudLayer = null;

		public override void Load() 
		{

            if (Global.worldWindow.CurrentWorld.IsEarth)
            {
                m_dynamicCloudLayer = new DynamicCloudLayer(
                    "Dynamic Clouds",
                    Global.worldWindow.CurrentWorld,
                    remoteImageUrl,
                    Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\" + archiveImageDirectoryPath);

                Global.worldWindow.CurrentWorld.RenderableObjects.Add(m_dynamicCloudLayer);
            }
				
		}
		

		public override void Unload() 
		{
			
		}	
	}
}
