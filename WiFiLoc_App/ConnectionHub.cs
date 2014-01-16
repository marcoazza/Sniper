using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Data.SqlServerCe;
namespace WiFiLoc_App
{
    /// <summary>
    /// Manages open connections on a per-thread basis
    /// </summary>
    public class ConnectionHub
    {
        private static Dictionary<int, SqlCeConnection> threadConnectionMap = new Dictionary<int, SqlCeConnection>();

        private static Dictionary<int, Thread> threadMap = new Dictionary<int, Thread>();

        /// <summary>
        /// The connection map
        /// </summary>
        public static Dictionary<int, SqlCeConnection> ThreadConnectionMap
        {
            get { return ConnectionHub.threadConnectionMap; }
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        //public static ConnectionString ConnectionString
        //{
        //    get { return global::ConnectionString.Default; }
        //}

        /// <summary>
        /// Gets a connection for this thread, maintains one open one of each.
        /// </summary>
        /// <remarks>Don't do this with anything but SQL compact edition or you'll run out of connections - compact edition is not
        /// connection pooling friendly and unloads itself too often otherwise so that is why this class exists</remarks> 
        /// <returns>An open connection</returns>
        public static SqlCeConnection Connection
        {
            get
            {
                lock (threadConnectionMap)
                {
                    //do some quick maintenance on existing connections (closing those that have no thread)


                    //now issue the appropriate connection for our current thread
                    int threadId = Thread.CurrentThread.ManagedThreadId;

                    SqlCeConnection connection = null;
                    if (threadConnectionMap.ContainsKey(threadId))
                    {
                        connection = threadConnectionMap[threadId];

                    }
                    if (connection == null)
                    {
                        connection = new SqlCeConnection("datasource=" + "c:\\Users\\SEVEN\\Desktop\\Sniper\\WiFiLoc_App\\bin\\Debug\\dbLocale.sdf");
                        //connection.Connection.Open();
                        if (threadConnectionMap.ContainsKey(threadId))
                            threadConnectionMap[threadId] = connection;
                        else
                            threadConnectionMap.Add(threadId, connection);
                        if (threadMap.ContainsKey(threadId))
                            threadMap[threadId] = Thread.CurrentThread;
                        else
                            threadMap.Add(threadId, Thread.CurrentThread);

                    }
                    return connection;
                }
            }
        }
    }
}
