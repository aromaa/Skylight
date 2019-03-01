using SkylightEmulator.Core;
using SkylightEmulator.Utilies;
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
        private Revision Revision;
        private Crypto Crypto;

        public SocketsListener(SocketsManager manager, string ip, int port, Revision revision, Crypto crypto)
        {
            this.SocketsManager = manager;
            this.Revision = revision;
            this.Crypto = crypto;

            this.AcceptCallback = new AsyncCallback(this.AcceptConnection);
            this.Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Server.NoDelay = true;
            this.Server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            this.Server.Listen(1000);

            if (revision == Revision.None)
            {
                Logging.WriteLine("Listening for connections on port: " + port);
            }
            else
            {
                Logging.WriteLine("Listening for connections on port: " + port + " with specified revision: " + revision);
            }
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
                    socket.NoDelay = true;
                    
                    this.SocketsManager.Connection(socket, socket.RemoteEndPoint, this.Revision, this.Crypto);
                }
                catch (Exception ex)
                {
                    Logging.LogException(ex.ToString());
                }
                finally
                {
                    this.AcceptConnection();
                }
            }
        }
    }
}
