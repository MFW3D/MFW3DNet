using MFW3D.Net;
using System;
using System.IO;
using Utility;

namespace MFW3D.Renderable
{
	public class GeoSpatialDownloadRequest : IDisposable
	{
		public float ProgressPercent;
		WebDownload download;
		string m_localFilePath;
		string m_url;
		QuadTile m_quadTile;
        ImageStore m_imageStore;

		/// <summary>
		/// Initializes a new instance of the <see cref= "T:WorldWind.Renderable.GeoSpatialDownloadRequest"/> class.
		/// </summary>
		/// <param name="quadTile"></param>
		public GeoSpatialDownloadRequest(QuadTile quadTile, ImageStore imageStore, string localFilePath, string downloadUrl)
		{
			m_quadTile = quadTile;
			m_url = downloadUrl;
			m_localFilePath = localFilePath;
            m_imageStore = imageStore;
            
		}

		/// <summary>
		/// Whether the request is currently being downloaded
		/// </summary>
		public bool IsDownloading
		{
			get
			{
				return (download != null);
			}
		}

		public bool IsComplete
		{
			get
			{
				if(download==null)
					return true;
				return download.IsComplete;
			}
		}

        public string LocalFilePath
        {
            get { return m_localFilePath; }
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

		private void DownloadComplete(WebDownload downloadInfo)
		{
//            Log.Write(Log.Levels.Debug+1, "GSDR", "Download completed for " + downloadInfo.Url);
            try
			{
				downloadInfo.Verify();

				//m_quadTile.QuadTileSet.NumberRetries = 0;

				// Rename temp file to real name
				File.Delete(m_localFilePath);
				File.Move(downloadInfo.SavedFilePath, m_localFilePath);

				// Make the quad tile reload the new image
				m_quadTile.DownloadRequests.Remove(this);
// ### ??!??				m_quadTile.Initialize();
			}
			catch(System.Net.WebException caught)
			{
				System.Net.HttpWebResponse response = caught.Response as System.Net.HttpWebResponse;
                /*
                 * null response
                 */
                if (response == null)
                {
                    m_quadTile.QuadTileSet.RecordFailedRequest(this);
                }
                /* 4xx - Client error
                 * 400 Bad Request
                 * 401 Unauthorized
                 * 403 Forbidden
                 * 404 Not Found
                 * 206 Partial Content
                 * 200 OK && Content length == 0
                 */
                else if(response.StatusCode==System.Net.HttpStatusCode.NotFound || 
                    response.StatusCode==System.Net.HttpStatusCode.Unauthorized ||
                    response.StatusCode==System.Net.HttpStatusCode.Forbidden ||
                    response.StatusCode==System.Net.HttpStatusCode.BadRequest ||
                    response.StatusCode == System.Net.HttpStatusCode.PartialContent ||
                    (response.StatusCode == System.Net.HttpStatusCode.OK && 
                        response.ContentLength == 0))
				{
                    m_quadTile.QuadTileSet.RecordFailedRequest(this);
                    
				}
                /* 
                 * 5xx - Server Error
                 * 500 Internal Server Error
                 * 501 Not Implemented
                 * 502 Bad Gateway
                 * 503 Service Unavailable
                 */
                else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError ||
                        response.StatusCode == System.Net.HttpStatusCode.NotImplemented ||
                        response.StatusCode == System.Net.HttpStatusCode.BadGateway ||
                        response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable
                        )
                {
                    //server problem, directly start timeout for the layer, rather than counter per tile


                    TimeSpan waitTime = TimeSpan.FromSeconds(120);
                    //if retry-after is specified then wait for that length of time before retrying
                    String retryAfter = response.GetResponseHeader("Retry-After");
                    if (retryAfter != null && !retryAfter.Equals(String.Empty))
                    {
                        Log.Write(Log.Levels.Verbose, "GSDR", "Retry-After response header " + retryAfter);
                        try
                        {
                            //try to convert
                            double retryAfterNumber = Convert.ToDouble(retryAfter);
                            waitTime = TimeSpan.FromSeconds(retryAfterNumber);
                        }
                        catch (System.FormatException fe)
                        {
                            //ignore retry-after, just wait for 60 seconds
                        }
                    }
                    //wait before retrying
                    m_quadTile.QuadTileSet.setTimeoutAndWait(waitTime);
                }
                
			}
			catch
			{
				using(File.Create(m_localFilePath + ".txt"))
				{}
                if (File.Exists(downloadInfo.SavedFilePath))
                {
                    try
                    {
                        File.Delete(downloadInfo.SavedFilePath);
                    }
                    catch (Exception e)
                    {
                        Log.Write(Log.Levels.Error, "GSDR", "could not delete file " + downloadInfo.SavedFilePath + ":");
                        Log.Write(e);
                    }
                }
			}
			finally
			{
                if(download != null)
    				download.IsComplete = true;
				m_quadTile.QuadTileSet.RemoveFromDownloadQueue(this);

                // potential deadlock! -step
                // Immediately queue next download
                m_quadTile.QuadTileSet.ServiceDownloadQueue();
			}
		}

		public virtual void StartDownload()
		{


            
            // Offline check
            if (World.Settings.WorkOffline)
                return;

            Log.Write(Log.Levels.Debug, "GSDR", "Starting download for " + m_url);
            //			Log.Write(Log.Levels.Debug, "GSDR", "to be stored in "+this.m_imageStore.GetLocalPath(QuadTile));

            MFW3D.Net.Wms.WmsImageStore wmsImageStore = m_imageStore as MFW3D.Net.Wms.WmsImageStore;
            System.Net.NetworkCredential downloadCredentials = null;

            if (wmsImageStore != null)
                downloadCredentials = new System.Net.NetworkCredential(wmsImageStore.Username, wmsImageStore.Password);

            QuadTile.IsDownloadingImage = true;
            download = new WebDownload(m_url, downloadCredentials);

			download.DownloadType = DownloadType.Wms;
			download.SavedFilePath = m_localFilePath + ".tmp";
			download.ProgressCallback += new DownloadProgressHandler(UpdateProgress);
			download.CompleteCallback += new MFW3D.Net.DownloadCompleteHandler(DownloadComplete);
			download.BackgroundDownloadFile();
		}

		void UpdateProgress( int pos, int total )
		{
			if(total==0)
				// When server doesn't provide content-length, use this dummy value to at least show some progress.
				total = 50*1024; 
			pos = pos % (total+1);
			ProgressPercent = (float)pos/total;
		}

		public virtual void Cancel()
		{
			if (download!=null)
				download.Cancel();
		}

		public override string ToString()
		{
			return m_imageStore.GetLocalPath(QuadTile);
		}

		#region IDisposable Members

		public virtual void Dispose()
		{
			if(download!=null)
			{
				download.Dispose();
				download=null;
			}
			GC.SuppressFinalize(this);
		}

		#endregion
	}
}
