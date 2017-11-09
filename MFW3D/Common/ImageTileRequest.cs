using System;
using System.IO;
using System.Net;
using WorldWind.Renderable;

namespace WorldWind.Net
{
	/// <summary>
	/// 分块图片请求
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
		/// 当前请求的西边界
		/// </summary>
		public override float West
		{
			get 
			{
				return (float)m_quadTile.West;
			}
		}

		/// <summary>
		/// 当前请求的东边界
		/// </summary>
		public override float East
		{
			get 
			{
				return (float)m_quadTile.East;
			}
		}

		/// <summary>
		/// 请求的北边界
		/// </summary>
		public override float North
		{
			get
			{
				return (float)m_quadTile.North;
			}
		}

		/// <summary>
		/// 请求的北边界
		/// </summary>
		public override float South
		{
			get 
			{
				return (float)m_quadTile.South;
			}
		}

		/// <summary>
		/// 请求的颜色
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
		/// 图块下载完成的回调
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
        /// 创建一个空文件，表示当前请求由于某些原因永久不可用。
        /// </summary>
        void FlagBadFile()
		{
			// 创建一个标识丢失文件的文本
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
        /// 通过占用一大块屏幕空间（像素）来计算此下载的相对重要性。
        /// </summary>
        public override float CalculateScore()
		{
			float screenArea = QuadTile.BoundingBox.CalcRelativeScreenArea(QuadTile.QuadTileArgs.Camera);
			return screenArea;
		}
	}
}
