using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Caching;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class SocketsManager
    {
        private ConcurrentDictionary<long, SocketsConnection> Connections;
        private List<SocketsListener> Listeners;
        
        private int NextID = 0;

        public SocketsManager(string ip, int port, int connectionLimit)
        {
            this.Connections = new ConcurrentDictionary<long, SocketsConnection>();
            this.Listeners = new List<SocketsListener>() { new SocketsListener(this, ip, port, Revision.None, Crypto.BOTH) };
        }

        public void AddListener(SocketsListener listener)
        {
            this.Listeners.Add(listener);

            listener.Start();
        }

        public long GetNextID()
        {
            return Interlocked.Increment(ref this.NextID);
        }

        public void Start()
        {
            if (this.Listeners != null)
            {
                foreach (SocketsListener listener in this.Listeners)
                {
                    listener.Start();
                }
            }
        }

        public void Stop()
        {
            if (this.Listeners != null)
            {
                foreach(SocketsListener listener in this.Listeners)
                {
                    listener.Stop();
                }
            }

            if (this.Connections != null)
            {
                foreach (SocketsConnection connection in this.Connections.Values)
                {
                    if (connection != null)
                    {
                        connection.Disconnect("Listener shutdown");
                    }
                }
            }
        }

        public void Connection(Socket socket, EndPoint ip, Revision revision, Crypto crypto)
        {
            long id = this.GetNextID(); //every socket have their own unique id
            SocketsConnection connection = new SocketsConnection(id, socket, ip); //for easy to use
            if (AntiDDoSManager.OnConnection(connection)) //not blocked
            {
                if (this.Connections.TryAdd(id, connection))
                {
                    Skylight.GetGame().GetGameClientManager().Connection(connection, revision, crypto);

                    if (Skylight.GetConfig()["emu.messages.connections"] == "1")
                    {
                        Logging.WriteLine(">> Connection [" + id + "] from [" + connection.GetIP() + "]");
                    }
                }
                else
                {
                    connection.Disconnect("Connection TryAdd failed");
                }
            }
            else
            {
                connection.Disconnect("Temp blocked IP");
            }
        }

        public void Disconnection(long id)
        {
            SocketsConnection connection;
            if (this.Connections.TryRemove(id, out connection))
            {
                Skylight.GetGame().GetGameClientManager().Disconnection(id);
                AntiDDoSManager.OnDisconnect(connection);
            }
        }
    }
}
