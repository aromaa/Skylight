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
    public class FlashPolicyFileRequestListener
    {
        private Socket Server;
        private AsyncCallback AcceptCallback;
        private bool Listening = false;

        public FlashPolicyFileRequestListener(string ip, int port)
        {
            this.AcceptCallback = new AsyncCallback(this.AcceptConnection);
            this.Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Server.NoDelay = true;
            this.Server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            this.Server.Listen(1000);

            Logging.WriteLine("Listening for fpfr on port: " + port);
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

                    new FlashPolicyFileRequestConnection(socket).Start();
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
