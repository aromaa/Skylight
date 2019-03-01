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
    public class SocketsConnection : IDisposable
    {
        public delegate void ReceivedData(byte[] data);

        private readonly long ID;
        private readonly string IP;
        private Socket Socket;
        private byte[] Buffer;
        private AsyncCallback ReceiveCallback;
        private AsyncCallback SendCallback;
        private SocketsConnection.ReceivedData ReceivedDataDelegate;
        private bool Disconnected = false;
        private string DisconnectReason = null;

        public SocketsConnection(long id, Socket socket, EndPoint ip)
        {
            this.ID = id;
            this.Socket = socket;
            this.IP = ip.ToString().Split(':')[0];
        }

        ~SocketsConnection()
        {
            this.Disconnect("Finalizer");
        }

        public bool IsDisconnected()
        {
            return this.Disconnected;
        }

        public long GetID()
        {
            return this.ID;
        }

        public string GetIP()
        {
            return this.IP;
        }

        public void Start(SocketsConnection.ReceivedData delegate_)
        {
            this.Buffer = new byte[1024];
            this.ReceivedDataDelegate = delegate_;
            this.ReceiveCallback = new AsyncCallback(this.ClientReceivedData);
            this.SendCallback = new AsyncCallback(this.ClientSendedData);

            this.AcceptData();
        }

        public void SendData(byte[] data)
        {
            if (!this.Disconnected)
            {
                try
                {
                    this.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, this.SendCallback, this);
                }
                catch
                {
                    this.Disconnect("Failed beging send data");
                }
            }
        }
        public void SendData(List<ArraySegment<byte>> data)
        {
            if (!this.Disconnected)
            {
                try
                {
                    this.Socket.BeginSend(data, SocketFlags.None, this.SendCallback, this);
                }
                catch
                {
                    this.Disconnect("Failed beging send data");
                }
            }
        }


        public void AcceptData()
        {
            if (!this.Disconnected)
            {
                try
                {
                    this.Socket.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, this.ReceiveCallback, this);
                }
                catch
                {
                    this.Disconnect("Failed beging receive data");
                }
            }
        }

        public void ClientReceivedData(IAsyncResult ar)
        {
            if (!this.Disconnected)
            {
                try
                {
                    int num = 0;
                    try
                    {
                        num = this.Socket.EndReceive(ar);
                    }
                    catch
                    {
                        this.Disconnect("Receiving data failed");
                        return;
                    }

                    if (num > 0)
                    {
                        byte[] packet = new byte[num];
                        Array.Copy(this.Buffer, packet, num);
                        this.HandleReceivedData(packet);
                    }
                    else
                    {
                        this.Disconnect("Receiving data failed, received 0 byte");
                    }
                }
                catch
                {
                    this.Disconnect("Failed receive data");
                }
                finally
                {
                    this.AcceptData();
                }
            }
        }

        public void HandleReceivedData(byte[] data)
        {
            this.ReceivedDataDelegate?.Invoke(data);
        }

        public void ClientSendedData(IAsyncResult ar)
        {
            if (!this.Disconnected)
            {
                try
                {
                    this.Socket.EndSend(ar);
                }
                catch
                {
                    this.Disconnect("Failed send data");
                }
            }
        }

        public void Disconnect(string reason)
        {
            if (reason != null)
            {
                this.DisconnectReason = reason;
            }

            this.Dispose();
        }

        public void Dispose()
        {
            if (!this.Disconnected)
            {
                this.Disconnected = true;

                try
                {
                    this.Socket.Shutdown(SocketShutdown.Both);
                    this.Socket.Close();
                    this.Socket.Dispose();

                    if (this.Buffer != null)
                    {
                        Array.Clear(this.Buffer, 0, this.Buffer.Length);
                    }
                }
                catch
                {

                }

                this.Buffer = null;
                this.ReceiveCallback = null;
                this.SendCallback = null;
                this.ReceivedDataDelegate = null;

                Skylight.GetSocketsManager().Disconnection(this.ID);

                if (Skylight.GetConfig()["emu.messages.connections"] == "1")
                {
                    Logging.WriteLine(">> Connection Dropped [" + this.ID + "] from [" + this.GetIP() + "] for reason: " + this.DisconnectReason);
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
