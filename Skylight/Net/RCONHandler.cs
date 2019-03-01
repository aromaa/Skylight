using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Net.RCON.Interface;
using SkylightEmulator.Net.RCON.Outgoing.Ping;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class RCONHandler : IDisposable
    {
        private Socket Socket;
        private byte[] Buffer;
        private Queue<byte> Data;
        private BinaryReader CurrentPacket;
        private bool Disposed = false;
        private AsyncCallback ReceiveCallback;
        private AsyncCallback SendCallback;

        public RCONHandler(Socket socket)
        {
            this.Socket = socket;
            this.ReceiveCallback = new AsyncCallback(this.ReceiveData);
            this.SendCallback = new AsyncCallback(this.SendDataCallback);
            this.Buffer = new byte[1024];
            this.Data = new Queue<byte>();

            this.ListenForData();
        }

        private void ReceiveData(IAsyncResult ar)
        {
            if (!this.Disposed)
            {
                try
                {
                    int bytes = this.Socket.EndReceive(ar);
                    if (bytes > 0)
                    {
                        for (int i = 0; i < bytes; i++)
                        {
                            this.Data.Enqueue(this.Buffer[i]);
                        }

                        this.HandleData();
                    }
                    else //send nothing aka closed connection
                    {
                        this.Dispose();
                    }
                }
                catch
                {
                    this.Dispose();
                }
                finally
                {
                    this.ListenForData();
                }
            }
        }

        private void SendPacket(RCONOutgoingPacket packet)
        {
            byte[] data = packet.GetBytes();

            byte[] data_ = new byte[data.Length + 2];
            data_[0] = (byte)data.Length;
            data_[1] = (byte)(data.Length >> 8);
            for (int i = 0; i < data.Length; i++)
            {
                data_[i + 2] = data[i];
            }

            this.SendData(data_);
        }

        private void SendData(byte[] data)
        {
            if (!this.Disposed)
            {
                this.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, this.SendCallback, null);
            }
        }

        private void SendDataCallback(IAsyncResult ar)
        {
            if (!this.Disposed)
            {
                try
                {
                    this.Socket.EndSend(ar);
                }
                catch
                {
                    this.Dispose();
                }
            }
        }

        private void ListenForData()
        {
            if (!this.Disposed)
            {

                try
                {
                    this.Socket.BeginReceive(this.Buffer, 0, this.Buffer.Length, SocketFlags.None, this.ReceiveCallback, this.Socket);
                }
                catch //if we fail we dispose, if we dont we wait for data and after that dispose
                {
                    this.Dispose();
                }
            }
        }

        public void HandleData()
        {
            while (this.Data.Count >= 2 || (this.Data.Count > 0 && this.CurrentPacket != null))
            {
                if (this.CurrentPacket != null)
                {
                    if (this.CurrentPacket.BaseStream.Length != ((MemoryStream)this.CurrentPacket.BaseStream).Capacity)
                    {
                        this.CurrentPacket.BaseStream.WriteByte(this.Data.Dequeue());
                    }

                    if (this.CurrentPacket.BaseStream.Length == ((MemoryStream)this.CurrentPacket.BaseStream).Capacity)
                    {
                        this.CurrentPacket.BaseStream.Position = 0;
                        this.HandlePacket();
                        this.CurrentPacket = null;
                    }
                }
                else
                {
                    this.CurrentPacket = new BinaryReader(new MemoryStream((this.Data.Dequeue() | this.Data.Dequeue() << 8)), Encoding.Default, false);
                }
            }
        }

        private void HandlePacket()
        {
            short headerId = this.CurrentPacket.ReadInt16();

            switch (headerId)
            {
                case -1:
                    this.SendPacket(new RCONPingOutgoingPacket());
                    break;
                case 0:
                    this.HandleLegacyEmulatorCommand(this.CurrentPacket.ReadString());
                    break;
            }
        }

        private void HandleLegacyEmulatorCommand(string data)
        {
            string[] splitData = data.Split(new char[] { Convert.ToChar(1) }, 2);
            switch (splitData[0])
            {
                case "updatepoints":
                case "updatepixels":
                    {
                        this.UpdateActivityPoints(splitData[1]);
                    }
                    break;
                case "updatecredits":
                    {
                        this.UpdateCredits(splitData[1]);
                    }
                    break;
                case "reloadbans":
                    {
                        this.UpdateBans();
                    }
                    break;
            }
        }

        public void UpdateBans()
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                Skylight.GetGame().GetBanManager().LoadBans(dbClient);
            }

            Skylight.GetGame().GetBanManager().DisconnecBannedUsers();
        }

        public void UpdateActivityPoints(string data)
        {
            uint userId = uint.Parse(data);
            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
            if (target != null)
            {
                DataRow data_ = null;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", userId);
                    data_ = dbClient.ReadDataRow("SELECT activity_points FROM users WHERE id = @userId LIMIT 1");
                }

                if (data_ != null)
                {
                    int pixels;
                    Dictionary<int, int> activityPoints = new Dictionary<int, int>();
                    if (!int.TryParse((string)data_["activity_points"], out pixels))
                    {
                        foreach (string s in ((string)data_["activity_points"]).Split(';'))
                        {
                            string[] activityPointsData = s.Split(',');

                            int activityPointId;
                            int activityPointAmount;
                            if (int.TryParse(activityPointsData[0], out activityPointId))
                            {
                                if (int.TryParse(activityPointsData[1], out activityPointAmount))
                                {
                                    activityPoints.Add(activityPointId, activityPointAmount);
                                }
                            }
                        }
                    }
                    else
                    {
                        activityPoints.Add(0, pixels);
                    }

                    target.GetHabbo().ActivityPoints = activityPoints;
                    target.GetHabbo().UpdateActivityPoints(-1, false);
                }
            }
        }

        public void UpdateCredits(string data)
        {
            uint userId = uint.Parse(data);
            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
            if (target != null)
            {
                DataRow data_ = null;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", userId);
                    data_ = dbClient.ReadDataRow("SELECT credits FROM users WHERE id = @userId LIMIT 1");
                }

                if (data_ != null)
                {
                    target.GetHabbo().Credits = (int)data_["credits"];
                    target.GetHabbo().UpdateCredits(false);
                }
            }
        }

        public void Dispose()
        {
            if (!this.Disposed)
            {
                this.Disposed = true;

                try
                {
                    this.Socket.Shutdown(SocketShutdown.Both);
                    this.Socket.Close();
                    this.Socket.Dispose();
                }
                catch
                {

                }

                this.Socket = null;
                Array.Clear(this.Buffer, 0, this.Buffer.Length);
                this.Buffer = null;
                this.ReceiveCallback = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}