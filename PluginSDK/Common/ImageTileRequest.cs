using System;
using System.IO;
using System.Net;
using WorldWind.Renderable;

namespace WorldWind.Net
{
	/// <summary>
	/// �ֿ�ͼƬ����
	/// </summary>
	public class ImageTileRequest : GeoSpatialDownloadRequest
	{
		QuadTile m_quadTile;
		public ImageTileRequest(object owner, QuadTile quadTile) : 
			base( owner, quadTile.ImageTileInfo.Uri )
		{
			m_quadTile = quadTile;
			download.DownloadType = DownloadType.Wms;
			SaveFilePath = QuadTile.ImageTileInfo.ImagePath;
		}

		/// <summary>
		/// ��ǰ��������߽�
		/// </summary>
		public override float West
		{
			get 
			{
				return (float)m_quadTile.West;
			}
		}

		/// <summary>
		/// ��ǰ����Ķ��߽�
		/// </summary>
		public override float East
		{
			get 
			{
				return (float)m_quadTile.East;
			}
		}

		/// <summary>
		/// ����ı��߽�
		/// </summary>
		public override float North
		{
			get
			{
				return (float)m_quadTile.North;
			}
		}

		/// <summary>
		/// ����ı��߽�
		/// </summary>
		public override float South
		{
			get 
			{
				return (float)m_quadTile.South;
			}
		}

		/// <summary>
		/// �������ɫ
		/// </summary>
		public override int Color
		{
			get
			{
				return World.Settings.downloadProgressColor;
			}
		}
		public QuadTile QuadTile
		{
			get
			{
				return m_quadTile;
			}
		}
		public double TileWidth 
		{
			get
			{
				return m_quadTile.East - m_quadTile.West;
			}
		}

		/// <summary>
		/// ͼ��������ɵĻص�
		/// </summary>
		protected override void DownloadComplete()
		{
			try
			{
				download.Verify();

				if(download.SavedFilePath != null && File.Exists(download.SavedFilePath))
					// Rename from .xxx.tmp -> .xxx
					File.Move(download.SavedFilePath, SaveFilePath);

				// Make the quad tile reload the new image
				m_quadTile.isInitialized = false;
				QuadTile.DownloadRequest = null;
			}
			catch(WebException caught)
			{
				System.Net.HttpWebResponse response = caught.Response as System.Net.HttpWebResponse;
				if(response!=null && response.StatusCode==System.Net.HttpStatusCode.NotFound)
					FlagBadFile();
			}
			catch(IOException)
			{
				FlagBadFile();
			}	
		}

        /// <summary>
        /// ����һ�����ļ�����ʾ��ǰ��������ĳЩԭ�����ò����á�
        /// </summary>
        void FlagBadFile()
		{
			// ����һ����ʶ��ʧ�ļ����ı�
			File.Create(SaveFilePath + ".txt");
			try
			{
				if(File.Exists(SaveFilePath))
					File.Delete(SaveFilePath);
			}
			catch 
			{
			}
		}

        /// <summary>
        /// ͨ��ռ��һ�����Ļ�ռ䣨���أ�����������ص������Ҫ�ԡ�
        /// </summary>
        public override float CalculateScore()
		{
			float screenArea = QuadTile.BoundingBox.CalcRelativeScreenArea(QuadTile.QuadTileArgs.Camera);
			return screenArea;
		}
	}
}
