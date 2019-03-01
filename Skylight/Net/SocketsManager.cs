using SkylightEmulator.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class SocketsManager
    {
        private ConcurrentDictionary<long, SocketsConnection> Connections;
        private SocketsListener SocketsListener;

        private object LOCK = new object();
        private int NextID = 0;

        public SocketsManager(string ip, int port, int connectionLimit)
        {
            this.Connections = new ConcurrentDictionary<long, SocketsConnection>();
            this.SocketsListener = new SocketsListener(ip, port, this);
        }

        public long GetNextID()
        {
            lock (this.LOCK)
            {
                return this.NextID++;
            }
        }

        public void Stop()
        {
            foreach(SocketsConnection connection in this.Connections.Values)
            {
                if (connection != null)
                {
                    connection.Dispose();
                }
            }

            this.SocketsListener.Stop();
            this.SocketsListener = null;
        }

        public void Start()
        {
            this.SocketsListener.Start();
        }

        public void Connection(Socket socket, EndPoint ip)
        {
            long id = this.GetNextID();
            SocketsConnection connection = new SocketsConnection(id, socket, ip);
            if (this.Connections.TryAdd(id, connection))
            {
                Skylight.GetGame().GetGameClientManager().Connection(connection);

                if (Skylight.GetConfig()["emu.messages.connections"] == "1")
                {
                    Logging.WriteLine(">> Connection [" + id + "] from [" + connection.GetIP() + "]");
                }
            }
            else
            {

            }
        }

        public void Disconnection(SocketsConnection connection)
        {
            Skylight.GetGame().GetGameClientManager().Disconnection(connection);

            SocketsConnection connection2;
            this.Connections.TryRemove(connection.GetID(), out connection2);
        }
    }
}
