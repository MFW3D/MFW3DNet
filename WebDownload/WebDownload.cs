using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Xml;
using Utility;
using WorldWind;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace WorldWind.Net
{
	public delegate void DownloadProgressHandler(int bytesRead, int totalBytes);
	public delegate void DownloadCompleteHandler(WebDownload wd);
    public delegate bool VerifyServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors);

	public enum DownloadType
	{
		Unspecified,
		Wms
	}

	public class WebDownload : IDisposable
	{
		#region Static proxy properties

		static public bool Log404Errors = false;
		static public bool useWindowsDefaultProxy = true;
		static public string proxyUrl = "";
		static public bool useDynamicProxy;
		static public string proxyUserName = "";
		static public string proxyPassword = "";

		#endregion
		
		public static string UserAgent = String.Format(
			CultureInfo.InvariantCulture,
			"World Wind v{0} ({1}, {2})",
			System.Windows.Forms.Application.ProductVersion,
			Environment.OSVersion.ToString(),
			CultureInfo.CurrentCulture.Name);


		public string Url;

		/// <summary>
		/// Memory downloads fills this stream
		/// </summary>
		public Stream ContentStream;

		public string SavedFilePath;
		public bool IsComplete;

        public NetworkCredential RequestCredentials;
        public int RequestTimeout = 0;

		/// <summary>
		/// Called when data is being received.  
		/// Note that totalBytes will be zero if the server does not respond with content-length.
		/// </summary>
		public DownloadProgressHandler ProgressCallback;

		/// <summary>
		/// Called to update debug window.
		/// </summary>
		public static DownloadCompleteHandler DebugCallback;

		/// <summary>
		/// Called when a download has ended with success or failure
		/// </summary>
		public static DownloadCompleteHandler DownloadEnded;

		/// <summary>
		/// Called when download is completed.  Call Verify from event handler to throw any exception.
		/// </summary>
		public DownloadCompleteHandler CompleteCallback;
		
		public DownloadType DownloadType = DownloadType.Unspecified;
		public string ContentType;
		public int BytesProcessed;
		public int ContentLength;

		// variables to allow placefinder to use egzipped requests
		//  *default to uncompressed requests to avoid breaking other things
		public bool Compressed = false;
		public string ContentEncoding;

		/// <summary>
		/// The download start time (or MinValue if not yet started)
		/// </summary>
		public DateTime DownloadStartTime = DateTime.MinValue;

		internal HttpWebRequest request;
		internal HttpWebResponse response;
        internal Stream responseStream;
        internal byte[] readBuffer;
        internal bool asyncInProgress = false;
        internal int num_retry;
		protected Exception downloadException;
        protected X509CertificateCollection col;
        protected bool timedOut = false;
        protected int NumInQueue = -1;

		protected bool isMemoryDownload;

		/// <summary>
		/// Initializes a new instance of the <see cref="WebDownload"/> class.
		/// </summary>
		/// <param name="url">The URL to download from.</param>
		public WebDownload(string url)
		{
			this.Url = url;
		}

        public WebDownload(string url, NetworkCredential credentials)
        {
            this.Url = url;
            this.RequestCredentials = credentials;
        }



		/// <summary>
		/// Initializes a new instance of the <see cref="T:WorldWind.Net.WebDownload"/> class.
		/// </summary>
		public WebDownload()
		{
		}
        /// <summary>
        /// Whether the download is currently being processed (active).
        /// </summary>
        public int Num_Downloads_In_Queue
        {
            set
            {
                NumInQueue = value;
            }
            get
            {
                return NumInQueue;
            }
        }

		/// <summary>
		/// Whether the download is currently being processed (active).
		/// </summary>
		public bool IsDownloadInProgress
		{
			get 
			{
                return asyncInProgress;
			}
		}

		/// <summary>
		/// Contains the exception that occurred during download, or null if successful.
		/// </summary>
		public Exception Exception
		{
			get 
			{
				return downloadException;
			}
		}

		/// <summary>
		/// Asynchronous download of HTTP data to file. 
		/// </summary>
		public void BackgroundDownloadFile()
		{
			if (CompleteCallback==null)
				throw new ArgumentException("No download complete callback specified.");

            DownloadAsync();
		}
	
		/// <summary>
		/// Asynchronous download of HTTP data to file.
		/// </summary>
		public void BackgroundDownloadFile( DownloadCompleteHandler completeCallback )
		{
			CompleteCallback += completeCallback;
			BackgroundDownloadFile();
		}

        /// <summary>
        /// Asynchronous download of HTTP data to file.
        /// </summary>
        public void BackgroundDownloadFile(string destinationFile, DownloadCompleteHandler completeCallback)
        {
			SavedFilePath = destinationFile;
            CompleteCallback += completeCallback;
            BackgroundDownloadFile();
        }

        /// <summary>
		/// Download image of specified type. (handles server errors for wms type)
		/// </summary>
		public void BackgroundDownloadFile( DownloadType dlType )
		{
			DownloadType = dlType;
			BackgroundDownloadFile();
		}

		/// <summary>
		/// Asynchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void BackgroundDownloadMemory()
		{
			if (CompleteCallback==null)
				throw new ArgumentException("No download complete callback specified.");

			isMemoryDownload = true;
            DownloadAsync();
		}
	
		/// <summary>
		/// Asynchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void BackgroundDownloadMemory( DownloadCompleteHandler completeCallback )
		{
			CompleteCallback += completeCallback;
			BackgroundDownloadMemory();
		}
	
		/// <summary>
		/// Download image of specified type. (handles server errors for WMS type)
		/// </summary>
		/// <param name="dlType">Type of download.</param>
		public void BackgroundDownloadMemory( DownloadType dlType )
		{
			DownloadType = dlType;
			BackgroundDownloadMemory();
		}

		/// <summary>
		/// Synchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void DownloadMemory()
		{
			isMemoryDownload = true;
			Download();
		}

		/// <summary>
		/// Download image of specified type. (handles server errors for WMS type)
		/// </summary>
		public void DownloadMemory( DownloadType dlType )
		{
			DownloadType = dlType;
			DownloadMemory();
		}

		/// <summary>
		/// HTTP downloads to memory.
		/// </summary>
		/// <param name="progressCallback">The progress callback.</param>
		public void DownloadMemory( DownloadProgressHandler progressCallback )
		{
			ProgressCallback += progressCallback;
			DownloadMemory();
		}

		/// <summary>
		/// Synchronous download of HTTP data to in-memory buffer. 
		/// </summary>
		public void DownloadFile( string destinationFile )
		{
			SavedFilePath = destinationFile;

			Download();
		}

		/// <summary>
		/// Download image of specified type to a file. (handles server errors for WMS type)
		/// </summary>
		public void DownloadFile( string destinationFile, DownloadType dlType )
		{
			DownloadType = dlType;
			DownloadFile(destinationFile);
		}

		/// <summary>
		/// Saves a http in-memory download to file.
		/// </summary>
		/// <param name="destinationFilePath">File to save the downloaded data to.</param>
		public void SaveMemoryDownloadToFile(string destinationFilePath )
		{
			if(ContentStream==null)
				throw new InvalidOperationException("No data available.");

			// Cache the capabilities on file system
			ContentStream.Seek(0,SeekOrigin.Begin);
			using(Stream fileStream = File.Create(destinationFilePath))
			{
				if(ContentStream is MemoryStream)
				{
					// Write the MemoryStream buffer directly (2GB limit)
					MemoryStream ms = (MemoryStream)ContentStream;
					fileStream.Write(ms.GetBuffer(), 0, (int)ms.Length);
				}
				else
				{
					// Block copy
					byte[] buffer = new byte[4096];
					while(true)
					{
						int numRead = ContentStream.Read(buffer, 0, buffer.Length);
						if(numRead<=0)
							break;
						fileStream.Write(buffer,0,numRead);
					}
				}
			}
			ContentStream.Seek(0,SeekOrigin.Begin);
		}

		/// <summary>
		/// Aborts the current download. 
		/// </summary>
		public void Cancel()
		{
			// completed downloads can't be cancelled.
			if (IsComplete)
				return;

            if (asyncInProgress)
			{
                asyncInProgress = false;
                Log.Write(Log.Levels.Debug, "Cancelling async download for " + this.Url);

                if(request != null)
                    request.Abort();
                if(response != null)
                    response.Close();
            }

            // make sure the response stream is closed.
            if(responseStream != null)
                responseStream.Close();

            // cancelled downloads must still be verified to set
            // the proper error bits and avoid immediate re-download
            /* NO. This is WRONG since the above request.Abort() or response.Close() 
             * may still be in flight.
            if (CompleteCallback == null)
            {
                Verify();
            }
            else
            {
                try
                {
                    CompleteCallback(this);
                }
                catch
                {

                }
            }
             */
            OnDebugCallback(this);
            OnDownloadEnded(this);
            // it's not really complete, but done with...
            IsComplete = true;
        }

		/// <summary>
		/// Notify event subscribers of download progress.
		/// </summary>
		/// <param name="bytesRead">Number of bytes read.</param>
		/// <param name="totalBytes">Total number of bytes for request or 0 if unknown.</param>
		private void OnProgressCallback(int bytesRead, int totalBytes)
		{
			if (ProgressCallback != null)
			{
                if (bytesRead > totalBytes) totalBytes = bytesRead;
				ProgressCallback(bytesRead, totalBytes);
			}
		}

		/// <summary>
		/// Called with detailed information about the download.
		/// </summary>
		/// <param name="wd">The WebDownload.</param>
		private static void OnDebugCallback(WebDownload wd)
		{
			if (DebugCallback != null)
			{
				DebugCallback(wd);
			}
		}

		/// <summary>
		/// Called when downloading has ended.
		/// </summary>
		/// <param name="wd">The download.</param>
		private static void OnDownloadEnded(WebDownload wd)
		{
			if (DownloadEnded != null)
			{
				DownloadEnded(wd);
			}
		}
        /// <summary>
        /// Called to Build the instance HttpWebRequest request with the proxy parameters
        /// and SSL/X509 with the WW user certificate
        /// </summary>
        /// 
        protected HttpWebRequest BuildRequest()
        {
            // Create the request object.
            request = (HttpWebRequest)WebRequest.Create(Url);
            request.UserAgent = UserAgent;

            request.Credentials = RequestCredentials;
            if (RequestTimeout != 0)
                request.Timeout = RequestTimeout;

        

            


            if (this.Compressed)
            {
                request.Headers.Add("Accept-Encoding", "gzip,deflate");
            }
            
            request.Proxy = ProxyHelper.DetermineProxyForUrl(
                Url,
                useWindowsDefaultProxy,
                useDynamicProxy,
                proxyUrl,
                proxyUserName,
                proxyPassword);

            X509Store certStore = new X509Store(StoreLocation.CurrentUser);

            certStore.Open(OpenFlags.OpenExistingOnly);

            col = new X509CertificateCollection(certStore.Certificates);
            certStore.Close();

            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(VerifyServerCertificate);
            request.ClientCertificates = col;



            return request;
        }
        /// <summary>
        /// Callback with the SSL/X509 security response
        /// </summary>

        bool VerifyServerCertificate(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            if (sslPolicyErrors == SslPolicyErrors.None) return true;

            foreach (X509ChainStatus s in chain.ChainStatus)
            {
                // allows expired certificates
                if (string.Equals(s.Status.ToString(), "NotTimeValid",
                    StringComparison.OrdinalIgnoreCase))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Called to Build the instance Stream ContentStream used as the buffer respository to the downloads
        /// </summary>
        protected void BuildContentStream()
        {
            // create content stream from memory or file
            if (isMemoryDownload && ContentStream == null)
            {
                ContentStream = new MemoryStream();
            }
            else
            {
                // Download to file
                string targetDirectory = Path.GetDirectoryName(SavedFilePath);
                if (targetDirectory.Length > 0)
                    Directory.CreateDirectory(targetDirectory);
                ContentStream = new FileStream(SavedFilePath, FileMode.Create);
            }
        }

		/// <summary>
		/// Synchronous HTTP download
		/// </summary>
		protected void Download()
		{
            
            Log.Write(Log.Levels.Debug, "Starting sync download for " + this.Url);

            //Cookie foo = new Cookie();
            //request.CookieContainer.Add(foo);

            //request.co
            Debug.Assert(Url.StartsWith("http"));
			DownloadStartTime = DateTime.Now;
			try
			{
				try
				{
					// If a registered progress-callback, inform it of our download progress so far.
					OnProgressCallback(0, 1);
					OnDebugCallback(this);

                    BuildContentStream();
                    request = BuildRequest();

                    // TODO: probably better done via BeginGetResponse() / EndGetResponse() because this may block for a while
                    // causing warnings in thread abortion.
					using (response = request.GetResponse() as HttpWebResponse)
					{
                        
						// only if server responds 200 OK
						if (response.StatusCode == HttpStatusCode.OK)
						{
							ContentType = response.ContentType;
							ContentEncoding = response.ContentEncoding;

							
							// Find the data size from the headers.
							string strContentLength = response.Headers["Content-Length"];
							if (strContentLength != null)
							{
								ContentLength = int.Parse(strContentLength, CultureInfo.InvariantCulture);
							}



							byte[] readBuffer = new byte[1500];
							using (Stream responseStream = response.GetResponseStream())
							{
								while (true)
                                {
									//  Pass do.readBuffer to BeginRead.
									int bytesRead = responseStream.Read(readBuffer, 0, readBuffer.Length);
									if (bytesRead <= 0)
										break;

									//TODO: uncompress responseStream if necessary so that ContentStream is always uncompressed
									//  - at the moment, ContentStream is compressed if the requesting code sets
									//    download.Compressed == true, so ContentStream must be decompressed 
									//    by whatever code is requesting the gzipped download
									//  - this hack only works for requests made using the methods that download to memory,
									//    requests downloading to file will result in a gzipped file
									//  - requests that do not explicity set download.Compressed = true should be unaffected

									ContentStream.Write(readBuffer, 0, bytesRead);

									BytesProcessed += bytesRead;

//
// * foreground downloads don't report progress
//									// If a registered progress-callback, inform it of our download progress so far.
//									OnProgressCallback(BytesProcessed, ContentLength);
//									OnDebugCallback(this);
//
								}
							}

						}
					}

					HandleErrors();
				}
                catch (ThreadAbortException)
                {
                    // re-throw to avoid it being caught by the catch-all below
                    Log.Write(Log.Levels.Verbose, "Re-throwing ThreadAbortException.");
                    throw;
                }
				catch (System.Configuration.ConfigurationException)
				{
					// is thrown by WebRequest.Create if App.config is not in the correct format
					// TODO: don't know what to do with it
					throw;
				}
				catch (Exception caught)
				{
					try
					{
						// Remove broken file download
						if (ContentStream != null)
						{
							ContentStream.Close();
							ContentStream = null;
						}
						if (SavedFilePath != null && SavedFilePath.Length > 0)
						{
							File.Delete(SavedFilePath);
						}
					} 
					catch(Exception) 
					{
					}
					SaveException(caught);
				}

				if (ContentLength == 0)
				{
					ContentLength = BytesProcessed;
					// If a registered progress-callback, inform it of our completion
					OnProgressCallback(BytesProcessed, ContentLength);
				}

				if (ContentStream is MemoryStream)
				{
					ContentStream.Seek(0, SeekOrigin.Begin);
				}
				else if (ContentStream != null)
				{
					ContentStream.Close();
					ContentStream = null;
				}
			
				OnDebugCallback(this);
				
				if (CompleteCallback == null)
				{
					Verify();
				}
				else
				{
					CompleteCallback(this);
				}
			}
			catch (ThreadAbortException)
			{
                Log.Write(Log.Levels.Verbose, "Download aborted.");
            }
			finally
			{
				IsComplete = true;
			}

			OnDownloadEnded(this);
		}
        /// <summary>
        /// Async HTTP response Callback
        /// IAsyncResult asyncResult (input) -> data necesary to access the webdonwload instance, because 
        /// This is a static method and has not the Webdownload instance
        /// </summary>
        private static void AsyncResponseCallback(IAsyncResult asyncResult)
        {
            WebDownload webDL = (WebDownload)asyncResult.AsyncState;



            try
            {
                webDL.response = webDL.request.EndGetResponse(asyncResult) as HttpWebResponse;

                // only if server responds 200 OK
                if (webDL.response.StatusCode == HttpStatusCode.OK)
                {
                    webDL.ContentType = webDL.response.ContentType;
                    webDL.ContentEncoding = webDL.response.ContentEncoding;

                    // Find the data size from the headers.
                    string strContentLength = webDL.response.Headers["Content-Length"];
                    if (strContentLength != null)
                    {
                        webDL.ContentLength = int.Parse(strContentLength, CultureInfo.InvariantCulture);
                    }
                    // If if (webDL.ContentLength <= 0) ?????

                    webDL.responseStream = webDL.response.GetResponseStream();

                    IAsyncResult result = webDL.responseStream.BeginRead(webDL.readBuffer, 0, webDL.readBuffer.Length, new AsyncCallback(AsyncReadCallback), webDL);
                    return;
                }
                else
                {
                    Exception myException = new Exception(webDL.response.StatusCode.ToString());

                    webDL.SaveException(myException);

                    webDL.AsyncFinishDownload();
                }

            }
            catch (WebException e)
            {
                if (webDL.timedOut == false)
                {

                    // request cancelled.
                    Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): WebException: " + e.Status.ToString());
                    webDL.SaveException(e);
                    webDL.AsyncFinishDownload();
                    //webDL.Cancel();
                }
            }
            catch (ArgumentNullException e)
            {
                // request cancelled.
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): ArgumentNullException: " + e.Message);
                webDL.SaveException(e);
                webDL.AsyncFinishDownload();
            }
            catch (InvalidOperationException e)
            {
                // request cancelled.
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): InvalidOperationException: " + e.Message);
                webDL.SaveException(e);
                webDL.AsyncFinishDownload();
            }
            catch (ArgumentException e)
            {
                // request cancelled.
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): ArgumentException: " + e.Message);
                webDL.SaveException(e);
                webDL.AsyncFinishDownload();
            }
            catch (NullReferenceException e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): NullReferenceException: " + e.Message);
                webDL.SaveException(e);
                webDL.AsyncFinishDownload();

            }
            catch (Exception e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncResponseCallback(): Exception: " + e.Message);
                webDL.SaveException(e);
                //Cancel();
                webDL.AsyncFinishDownload();

            }

           

        }
        /// <summary>
        /// Async HTTP Read Callback
        /// IAsyncResult asyncResult (input) -> data necesary to access the webdonwload instance, because 
        /// This is a static method and has not the Webdownload instance
        /// </summary>
        private static void AsyncReadCallback(IAsyncResult asyncResult)
        {
            WebDownload webDL = (WebDownload)asyncResult.AsyncState;

            Stream responseStream = webDL.responseStream;
            try
            {
                int read = responseStream.EndRead(asyncResult);
                if (read > 0)
                {
                    
                    webDL.ContentStream.Write(webDL.readBuffer, 0, read);
                    webDL.BytesProcessed += read;
                    webDL.OnProgressCallback(webDL.BytesProcessed, webDL.ContentLength);
                    
                    IAsyncResult asynchronousResult = responseStream.BeginRead(webDL.readBuffer, 0, webDL.readBuffer.Length, new AsyncCallback(AsyncReadCallback), webDL);
                   
                    return;
                }
                else
                {
                    if (webDL.BytesProcessed <= 0)
                    {
                        Exception myException = new Exception("400 No tile");

                        webDL.SaveException(myException);

                        webDL.AsyncFinishDownload();
                        return;
                    }

                    webDL.SaveException(null);
                    webDL.AsyncFinishDownload();
                }
            }
            catch (IOException ex)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncReadCallback(): IOException: " + ex.Message.ToString() + webDL.BytesProcessed);
                if (webDL.num_retry > 5)
                {
                    webDL.SaveException(new Exception("Unable to connect to the remote server several Async Read tries have been broke"));
                    webDL.AsyncFinishDownload();
                    return;
                }
                webDL.num_retry++;

                webDL.AsyncFinishPrepareRetry();
                
                webDL.DownloadAsync();

                
            }
            catch (WebException e)
            {
                if (webDL.timedOut==false)
                {
                    // request cancelled.
                    Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncReadCallback(): WebException: " + e.Status.ToString());
                    webDL.SaveException(e);
                    webDL.AsyncFinishDownload();
                    //webDL.Cancel();
                }
            }
            catch (NullReferenceException e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncReadCallback(): NullReferenceException: " + e.Message);
                webDL.SaveException(e);
                webDL.AsyncFinishDownload();

            }
            catch (Exception e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "AsyncReadCallback(): Exception: " + e.Message);
                webDL.SaveException(e);
                //Cancel();
                webDL.AsyncFinishDownload();

            }
           

        }
        /// <summary>
        /// Async HTTP Finish function used to prepare an automatic retry of the request.
        /// This function is called because:
        /// 1) the server response was an error but server is online.
        /// 2) WW ask to Webdownload the same request several times and Webdownload get
        /// an error because the contentstream is in use by other previous request.
        /// 
        /// </summary>
        private void AsyncFinishPrepareRetry()
        {
           // if (request != null) request.Abort();
           // if (response != null) response.Close();
            if (ContentStream is MemoryStream)
            {
                ContentStream.Seek(0, SeekOrigin.Begin);
            }
            else if (ContentStream != null)
            {
                ContentStream.Close();
                ContentStream = null;
            }
            if (responseStream != null) responseStream.Close();

        }
        /// <summary>
        /// Async HTTP Finish function 
        /// The conection is finished because all is OK or any kind error is reported
        /// the error is reported in SaveException();
        /// </summary>

        private void AsyncFinishDownload()
        {
            
            if (ContentLength == 0)
            {
                ContentLength = BytesProcessed;
                // If a registered progress-callback, inform it of our completion
                OnProgressCallback(BytesProcessed, ContentLength);
            }

            if (ContentStream is MemoryStream)
            {
                ContentStream.Seek(0, SeekOrigin.Begin);
            }
            else if (ContentStream != null)
            {
                ContentStream.Close();
                ContentStream = null;
            }
            if (responseStream != null) responseStream.Close();

            OnDebugCallback(this);

            if (CompleteCallback == null)
            {
                try
                {
                    Verify();
                }
                catch
                {
                }
            }
            else
            {
                try
                {
                    CompleteCallback(this);
                }
                catch
                {
                }
            }

            OnDownloadEnded(this);
			IsComplete = true;
            
		}
        /// <summary>
        /// Async Timeout  Callback
        /// In the case of asynchronous requests, 
        /// it is the responsibility of the client application to implement
        /// its own time-out mechanism.
        /// </summary>
        
        private void TimeoutCallback(object state, bool timedOut)
        {
            if (timedOut)
            {
                this.timedOut = timedOut;
                HttpWebRequest request = state as HttpWebRequest;
                if (request != null)// && response ==null)
                {
                    request.Abort();
                    
                    if (num_retry > 5)
                    {
                        SaveException(new Exception("Unable to connect to the remote server: Timeout"));
                        AsyncFinishDownload();
                        return;
                    }

                    AsyncFinishPrepareRetry();

                    
                    num_retry++;
                    DownloadAsync();
                    
                }
            }
        }

        /// <summary>
        /// Async Download base function
        /// </summary>
        protected void DownloadAsync()
        {
            asyncInProgress = true;
            timedOut = false;
            Log.Write(Log.Levels.Debug, "Starting async download for " + this.Url);
            
            

            Debug.Assert(Url.StartsWith("http"));
            DownloadStartTime = DateTime.Now;
            try
            {
                // If a registered progress-callback, inform it of our download progress so far.
                OnProgressCallback(0, 1);
                OnDebugCallback(this);

                BuildContentStream();
                request = BuildRequest();
                request.Timeout = 10000;

                readBuffer = new byte[1500];


                IAsyncResult result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(AsyncResponseCallback), this);
                // this line implements the timeout, if there is a timeout, the callback fires 
                ThreadPool.RegisterWaitForSingleObject(result.AsyncWaitHandle, new WaitOrTimerCallback(TimeoutCallback), request, request.Timeout, true);


            }
            catch (System.Configuration.ConfigurationException)
            {
                // is thrown by WebRequest.Create if App.config is not in the correct format
                // TODO: don't know what to do with it
                throw;
            }
            catch (WebException e)
            {
                // Abort() was called.
                Utility.Log.Write(Log.Levels.Debug, "NET", "DownloadAsync(): WebException: " + e.Status.ToString());
                SaveException(e);
                // Cancel();
                AsyncFinishDownload();
            }
            catch (System.IO.IOException e)
            {
                //SaveException(new Exception("Request actually processing... This Request will be Canceled"));
                Utility.Log.Write(Log.Levels.Debug, "NET", "DownloadAsync(): IOException: " + e.Message.ToString());
                Cancel();

            }
            catch (NullReferenceException e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "DownloadAsync(): NullReferenceException: " + e.Message);
                SaveException(e);
                AsyncFinishDownload();

            }

            catch (Exception e)
            {
                Utility.Log.Write(Log.Levels.Debug, "NET", "DownloadAsync(): Exception: " + e.Message);
                SaveException(e);
                //Cancel();
                AsyncFinishDownload();

            }
            
            
        }
		


		/// <summary>
		/// Handle server errors that don't get trapped by the web request itself.
		/// </summary>
		private void HandleErrors()
		{
			// HACK: Workaround for TerraServer failing to return 404 on not found
			if(ContentStream.Length == 15)
			{
				// a true 404 error is a System.Net.WebException, so use the same text here
				Exception ex = new FileNotFoundException("The remote server returned an error: (404) Not Found.", SavedFilePath );
				SaveException(ex);
			}

			// TODO: WMS 1.1 content-type != xml
			// TODO: Move WMS logic to WmsDownload
			if (DownloadType == DownloadType.Wms && (
				ContentType.StartsWith("text/xml") ||
				ContentType.StartsWith("application/vnd.ogc.se")))
			{
				// WMS request failure
				SetMapServerError();
			}
		}

		/// <summary>
		/// If exceptions occurred they will be thrown by calling this function.
		/// </summary>
		public void Verify()
		{
            if (Exception != null)
            {
                //these occur regularly - ignore
                //no - let these errors also rise up to the request to handle (ie a 404 will not get requested again)
                throw Exception;
            }
            else if (response == null)
            {
                return;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.PartialContent)
            {
                //treat 206 Partial Content as an Exception
                WebException partialDownloadException = new WebException("Partial Download Response", null, WebExceptionStatus.ReceiveFailure, response);
                throw partialDownloadException;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.OK && response.ContentLength == 0)
            {
                //treat 200 OK and content length == 0 as an Exception
                WebException noContentException = new WebException("No Content OK Download Response", null, WebExceptionStatus.ReceiveFailure, response);
                throw noContentException;
            }
		}

		/// <summary>
		/// Log download error to log file
		/// </summary>
		/// <param name="exception"></param>
		private void SaveException( Exception exception )
		{
			// Save the exception 
			downloadException = exception;

			if(Exception is ThreadAbortException)
				// Don't log canceled downloads
				return;

			if(Log404Errors)
			{
				Log.Write(Log.Levels.Error, "HTTP", "Error: " + Url );
				Log.Write(Log.Levels.Error+1, "HTTP", "     : " + exception.Message );
			}
		}

		/// <summary>
		/// Reads the xml response from the server and throws an error with the message.
		/// </summary>
		private void SetMapServerError()
		{
			try
			{
				XmlDocument errorDoc = new XmlDocument();
				ContentStream.Seek(0,SeekOrigin.Begin);
				errorDoc.Load(ContentStream);
				string msg = "";
				foreach( XmlNode node in errorDoc.GetElementsByTagName("ServiceException"))
					msg += node.InnerText.Trim()+Environment.NewLine;
				SaveException( new WebException(msg.Trim()) );
			}
			catch(XmlException)
			{
				SaveException( new WebException("An error occurred while trying to download " + request.RequestUri.ToString()+".") );
			}
		}

		#region IDisposable Members

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or
		/// resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
            if (asyncInProgress)
            {
                this.Cancel();
            }

			if(request!=null)
			{
				request.Abort();
				request = null;
			}

			if (ContentStream != null)
			{
				ContentStream.Close();
				ContentStream=null;
			}
            // VE BUG lines commented
		//	if(DownloadStartTime != DateTime.MinValue)
		//		OnDebugCallback(this);

			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
