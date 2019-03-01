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
    public class RCONListener
    {
        private Socket Server;
        private AsyncCallback AcceptCallback;
        private bool Listening = false;
        private HashSet<string> AllowedIPs;

        public RCONListener(string ip, int port, string allowedIps)
        {
            this.AcceptCallback = new AsyncCallback(this.AcceptConnection);
            this.Server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            this.Server.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            this.Server.Listen(1000);

            this.AllowedIPs = new HashSet<string>();
            foreach(string ip_ in allowedIps.Split(';'))
            {
                this.AllowedIPs.Add(ip_);
            }

            Logging.WriteLine("Listening for RCON on port: " + port);
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
                    EndPoint ip = socket.RemoteEndPoint;
                    string ip_ = ip.ToString().Split(':')[0];

                    if (this.AllowedIPs.Contains(ip_)) //allowed
                    {
                        new RCONHandler(socket);
                    }
                    else //not allowed
                    {
                        socket.Close();
                    }
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
