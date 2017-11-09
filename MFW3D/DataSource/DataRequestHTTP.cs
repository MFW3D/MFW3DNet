using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using Utility;

namespace MFW3D.DataSource
{
    /// <summary>
    /// An implementation of the DataRequest class for HTTP style URLs. Data is read asynchronously
    /// into a local memory buffer, and the DataRequest's content stream is fed from memory (unless
    /// it could be fulfilled from Cache anyway).
    /// </summary>
    public class DataRequestHTTP : DataRequest
    {
        #region HTTP Request Data
        private WebRequest m_webRequest;
        // async result handle
        private IAsyncResult m_requestResult;
        private IAsyncResult m_readResult;
        // the async stream from the web resource to our local memory buffer
        private Stream m_responseStream;
        // the private memory buffer we're reading into
        private byte[] m_buffer;
        // how much we've already read into the buffer
        private int m_bytesRead;
        #endregion

        #region DataRequest Property Overrides
        public override float Progress
        {
            get
            { 
                if (m_buffer == null)
                    return 0;
                else
                    return (float)(100.0f * m_bytesRead / (float)m_buffer.Length); 
            }
        }
        #endregion

        public DataRequestHTTP(DataRequestDescriptor request)
            : base(request)
        {
        }

        /// <summary>
        /// Initiate an asynchronous request
        /// </summary>
        public override void Start()
        {
#if VERBOSE
            Log.Write(Log.Levels.Verbose, "DataRequest: starting async request for " + m_request.Source);
#endif
            m_state = DataRequestState.InProcess;
            m_webRequest = WebRequest.Create(m_request.Source);
            m_webRequest.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.BypassCache);
            m_webRequest.Proxy = WebRequest.GetSystemWebProxy();

            m_requestResult = m_webRequest.BeginGetResponse(new AsyncCallback(ResponseCallback), this);
        }

        /// <summary>
        /// Asynchronous response callback. Called upon completion of the WebRequest.
        /// </summary>
        /// <param name="asyncResult">Result state.</param>
        private static void ResponseCallback(IAsyncResult asyncResult)
        {
            DataRequestHTTP dataRequest = asyncResult.AsyncState as DataRequestHTTP;
            try
            {
                WebResponse response = dataRequest.m_webRequest.EndGetResponse(asyncResult);

#if VERBOSE
                Log.Write(Log.Levels.Verbose, "DataRequest: response received for " + dataRequest.m_request.Source);
                Log.Write(Log.Levels.Verbose, "DataRequest: Content Length = " + response.ContentLength);
                Log.Write(Log.Levels.Verbose, "DataRequest: Content Type = " + response.ContentType);
                Log.Write(Log.Levels.Verbose, "DataRequest: starting async read...");
#endif
                if (response.ContentLength > 0)
                {
                    dataRequest.m_headers = response.Headers;
                    dataRequest.m_responseStream = response.GetResponseStream();
                    dataRequest.m_buffer = new byte[response.ContentLength];
                    dataRequest.m_bytesRead = 0;
                    dataRequest.m_readResult = dataRequest.m_responseStream.BeginRead(dataRequest.m_buffer, 0, (int)(response.ContentLength), new AsyncCallback(ReadCallback), dataRequest);
                }
                else
                {
                    response.Close();
                    //TODO: hack to get responses with < 0 content length to work, which includes TerraServer layers
                    using (System.Net.WebClient client = new WebClient())
                    {
                        dataRequest.m_buffer = client.DownloadData(response.ResponseUri.OriginalString);
                        dataRequest.m_bytesRead = dataRequest.m_buffer.Length;
                        dataRequest.m_contentStream = new MemoryStream(dataRequest.m_buffer);
                        dataRequest.m_state = DataRequestState.Finished;
                    }
                }
            }
            catch (System.Net.WebException ex)
            {
                Log.Write(Log.Levels.Warning, "DataRequest: exception caught trying to access " + dataRequest.Source);
                Log.Write(ex);
                dataRequest.m_state = DataRequestState.Error;
            }
        }

        /// <summary>
        /// Asynchronous read callback. Called upon completion of stream reading.
        /// </summary>
        /// <param name="asyncResult">Result state.</param>
        private static void ReadCallback(IAsyncResult asyncResult)
        {
            DataRequestHTTP dataRequest = asyncResult.AsyncState as DataRequestHTTP;

#if VERBOSE
            Log.Write(Log.Levels.Verbose, "DataRequest: async read finished for " + dataRequest.m_request.Source);
            Log.Write(Log.Levels.Verbose, "DataRequest: completed? is " + asyncResult.IsCompleted);
#endif
            int newBytes = dataRequest.m_responseStream.EndRead(asyncResult);
            m_totalBytes += newBytes;
            dataRequest.m_bytesRead += newBytes;
            if (dataRequest.m_bytesRead < dataRequest.m_buffer.Length)
            {
#if VERBOSE
                Log.Write(Log.Levels.Verbose, "DataRequest: not complete, rescheduling for the rest. bytes read so far: " + dataRequest.m_bytesRead);
#endif
                dataRequest.m_responseStream.BeginRead(dataRequest.m_buffer, dataRequest.m_bytesRead, dataRequest.m_buffer.Length - dataRequest.m_bytesRead, new AsyncCallback(ReadCallback), dataRequest);
                return;
            }
            dataRequest.m_responseStream.Close();
#if VERBOSE
            Log.Write(Log.Levels.Verbose, "DataRequest: stream wrote " + dataRequest.m_bytesRead + " bytes");
#endif

            dataRequest.m_contentStream = new MemoryStream(dataRequest.m_buffer);

            dataRequest.m_state = DataRequestState.Finished;
        }
    }
}
