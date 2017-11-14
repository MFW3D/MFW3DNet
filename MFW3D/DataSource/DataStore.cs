//#define VERBOSE
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using Utility;

namespace MFW3D.DataSource
{
    /// <summary>
    /// A generic class for asynchronous, caching local and remote file access.
    /// Replaces WebDownload etc.
    /// </summary>
    public class DataStore
    {
        #region ≥…‘±
        /// <summary>
        /// The currently pending request descriptors. Requests are kept sorted according to priority.
        /// </summary>
        static private ArrayList m_pendingRequests = new ArrayList(15);
        /// <summary>
        /// Currently active request descriptors.
        /// </summary>
        static private ArrayList m_activeRequests = new ArrayList(5);
        /// <summary>
        /// The finished requests. Designed as a hash table to allow fast check for finished requests.
        /// </summary>
        static private ArrayList m_finishedRequests = new ArrayList();
        /// <summary>
        /// The maximum number of simultaneously active requests. This better not be zero.
        /// </summary>
        static private int m_maxActiveRequests = 5;
        /// <summary>
        /// The thread safe locking object.
        /// </summary>
        static private Object m_lock = new Object();
        #endregion

        #region  Ù–‘

        static public ArrayList ActiveRequests
        {
            get { return m_activeRequests; }
        }

        static public ArrayList PendingRequests
        {
            get { return m_pendingRequests; }
        }

        /// <summary>
        /// The number of currently pending (inactive) requests
        /// </summary>
        static public int PendingRequestCount
        {
            get
            {
                return m_pendingRequests.Count;
            }
        }

        /// <summary>
        /// The number of currently active (downloading) requests
        /// </summary>
        static public int ActiveRequestCount
        {
            get
            {
                return m_activeRequests.Count;
            }
        }
        #endregion

        /// <summary>
        /// Put a new request in the queue. Returns a DataRequestDescriptor matching the request. If there is
        /// already a request pending for the same source, the existing descriptor is returned.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        static public DataRequest Request(DataRequestDescriptor request)
        {
            // TODO: actually parse the request and differentiate between different DataRequest types
            DataRequestHTTP drd = new DataRequestHTTP(request);

            // before anything, try to fulfill the request from cache
#if VERBOSE
            Log.Write(Log.Levels.Verbose, "DataStore: New request for " + request.Source);
#endif
/*
            if (drd.TryCache())
            {
#if VERBOSE
                Log.Write(Log.Levels.Verbose, "DataStore: Request fulfilled from cache, no queue required");
#endif
                if (drd.RequestDescriptor.CompletionCallback != null)
                {
#if VERBOSE
                    Log.Write(Log.Levels.Verbose, "DataStore: calling completion callback...");
#endif
                    drd.RequestDescriptor.CompletionCallback(drd);
                }
                return drd;
            }
*/
            lock (m_lock)
            {
#if VERBOSE
                Log.Write(Log.Levels.Verbose, "DataStore: Request enqueued.");
#endif
                m_pendingRequests.Insert(0,drd);
            }
            return drd;
        }

        /// <summary>
        /// Handle pending and active data requests. Call this function regularly from the background worker thread.
        /// </summary>
        static public void Update()
        {
#if VERBOSE
            if(PendingRequestCount > 0 || ActiveRequestCount > 0)
                Log.Write(Log.Levels.Verbose, "DataStore: Update() : " + PendingRequestCount + " requests pending, " + ActiveRequestCount + " active.");
#endif
            // clean out finished requests.
            for(int i =0; i < m_activeRequests.Count; i++)
            {
                DataRequestHTTP dr = m_activeRequests[i] as DataRequestHTTP;

                if (dr.State != DataRequestState.InProcess)
                {
#if VERBOSE
                    Log.Write(Log.Levels.Verbose, "DataStore: removing request " + dr.Source + " in state " + dr.State.ToString());
#endif
                    if (dr.State == DataRequestState.Finished && dr.RequestDescriptor.CompletionCallback != null)
                    {
#if VERBOSE
                        Log.Write(Log.Levels.Verbose, "DataStore: calling completion callback...");
#endif
                        dr.RequestDescriptor.CompletionCallback(dr);
                    }
                    else if (dr.State == DataRequestState.Error)
                    {
                        dr.State = DataRequestState.Delayed;
                        dr.NextTry = DateTime.Now + TimeSpan.FromSeconds(120);
                        Log.Write(Log.Levels.Warning, "DataStore: request " + dr.Source + " has error, delaying until " + dr.NextTry.ToLongTimeString());
                        m_pendingRequests.Add(dr);
                    }
                    m_activeRequests.Remove(dr);
                }
            }

            lock(m_lock)
            {
                // if we're ready to activate new requests, first sort them according to current priority
                if (ActiveRequestCount < m_maxActiveRequests)
                {
                    foreach (DataRequest dr in m_pendingRequests)
                    {
                        dr.UpdatePriority();
                        //if (dr.Priority < 0)
                        //    dr.Cancel();

                        if (dr.State == DataRequestState.Delayed && dr.NextTry < DateTime.Now)
                            dr.State = DataRequestState.NoCache;
                    }

                    // also clean up cancelled requests
                    for (int i = 0; i < PendingRequestCount; i++)
                    {
                        DataRequest dr = m_pendingRequests[i] as DataRequest;

                        if (dr.State == DataRequestState.Queued)
                        {
                            if (dr.TryCache())
                            {
                                dr.RequestDescriptor.CompletionCallback(dr);
                                m_pendingRequests.Remove(dr);
                            }
                            else
                                dr.State = DataRequestState.NoCache;
                        }
                        if (dr.State == DataRequestState.Cancelled)
                            m_pendingRequests.Remove(dr);
                    }

                    m_pendingRequests.Sort();
                }
            }

            // see if we can start any more requests
            while ((PendingRequestCount > 0) && (ActiveRequestCount < m_maxActiveRequests))
            {
                // look for first pending request

                DataRequest drd = null;
                lock (m_lock)
                {
                    drd = m_pendingRequests[0] as DataRequest;

                    // if the top priority request is delayed, we won't find anything better.
                    // so we can break out immediately.
                    if (drd.State != DataRequestState.NoCache || drd.NextTry > DateTime.Now)
                        break;

                    m_pendingRequests.RemoveAt(0);
                }

#if VERBOSE
                Log.Write(Log.Levels.Verbose, "DataStore: Activating request for " + drd.Source);
#endif
//                if (!drd.TryCache())
                {
                    drd.Start();

                    m_activeRequests.Add(drd);
                }
            }
        }
    }
}
