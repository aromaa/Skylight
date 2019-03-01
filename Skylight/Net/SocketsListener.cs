using SkylightEmulator.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class SocketsListener
    {
        private SocketsManager SocketsManager;
        private Socket Server;
        private AsyncCallback AcceptCallback;
        private bool Listening = false;

        public SocketsListener(string ip, int port, SocketsManager manager)
        {
            this.SocketsManager = manager;

            this.AcceptCallback = new AsyncCallback(this.AcceptConnection);
            this.Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint bindIP = new IPEndPoint(IPAddress.Parse(ip), port);
            this.Server.Bind(bindIP);
            this.Server.Listen(1000);

            Logging.WriteLine("Listening for connections on port: " + port);
        }

        public void Start()
        {
            if (!this.Listening)
            {
                this.Listening = true;
                this.AcceptConnection();
            }
        }

        public void Stop()
        {
            if (this.Listening)
            {
                this.Listening = false;
                try
                {
                    this.Server.Close();
                }
                catch
                {

                }
                Logging.WriteLine("Listener stopped");

                this.Server = null;
                this.SocketsManager = null;
                this.AcceptCallback = null;
            }
        }

        public void AcceptConnection()
        {
            if (this.Listening)
            {
                try
                {
                    this.Server.BeginAccept(this.AcceptCallback, this.Server);
                }
                catch
                {

                }
            }
        }

        public void AcceptConnection(IAsyncResult ar)
        {
            if (this.Listening)
            {
                try
                {
                    Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);
                    EndPoint ip = socket.RemoteEndPoint;
                    this.SocketsManager.Connection(socket, ip);
                }
                catch (Exception ex)
                {
                    Logging.WriteLine("Failed to receive connection! " + ex.ToString());
                }
                finally
                {
                    this.AcceptConnection();
                }
            }
        }
    }
}
