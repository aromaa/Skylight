using Newtonsoft.Json;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Users.Inventory;
using SkylightEmulator.Messages.MUS;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Net
{
    public class MUSHandler
    {
        private Socket Socket;
        private byte[] Buffer;
        private Queue<byte> Data;
        private int? ExpectedLenght;
        private bool Disposed = false;
        private AsyncCallback ReceiveCallback;
        private AsyncCallback SendCallback;
        private uint UserID;
        private string PhotoText;

        public MUSHandler(Socket socket)
        {
            this.Socket = socket;
            this.ReceiveCallback = new AsyncCallback(this.ReceiveData);
            this.SendCallback = new AsyncCallback(this.ClientSendedData);
            this.Buffer = new byte[1024];
            this.Data = new Queue<byte>();

            this.Listen();
        }

        private void Listen()
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

        private void ReceiveData(IAsyncResult ar)
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
                    this.Dispose(); //failed read
                    return;
                }

                if (num > 0)
                {
                    for(int i = 0; i < num; i++)
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
                this.Listen();
            }
        }

        private void HandleData()
        {
            while (this.Data.Count >= 6 || this.ExpectedLenght != null)
            {
                if (this.ExpectedLenght != null)
                {
                    if (this.Data.Count >= this.ExpectedLenght)
                    {
                        byte[] data = new byte[(int)this.ExpectedLenght];
                        for(int i = 0; i < data.Length; i++)
                        {
                            data[i] = this.Data.Dequeue();
                        }

                        this.ExpectedLenght = null;
                        this.HandleMessage(new MUSClientMessage(data));
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    byte headerTag = this.Data.Dequeue();
                    if (headerTag == 'r')
                    {
                        this.Data.Dequeue(); //useless
                        this.ExpectedLenght = this.Data.Dequeue() << 24 | this.Data.Dequeue() << 16 | this.Data.Dequeue() << 8 | this.Data.Dequeue();
                    }
                }
            }
        }

        private void HandleMessage(MUSClientMessage message)
        {
            int errorCode = message.ReadInt32();
            int timestamp = message.ReadInt32();
            string subject = message.ReadEvenPaddedString();
            string senderId = message.ReadEvenPaddedString();
            string[] receivers = new string[message.ReadInt32()];
            for(int i = 0; i < receivers.Length; i++)
            {
                receivers[i] = message.ReadEvenPaddedString();
            }

            if (subject != "Logon")
            {
                short contentType = message.ReadShort();
            }

            switch(subject)
            {
                case "Logon":
                    {
                        MUSServerMessage message_ = new MUSServerMessage("Logon", MUSType.String);
                        message_.WriteEvenPaddedString("Skylight: The best emulator around");
                        this.SendMessage(message_);

                        MUSServerMessage message_2 = new MUSServerMessage("HELLO", MUSType.String);
                        message_2.WriteEvenPaddedString("");
                        this.SendMessage(message_2);
                    }
                    break;
                case "LOGIN":
                    {
                        string[] credentials = message.ReadEvenPaddedString().Split(new char[] { ' ' }, 2);

                        GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(uint.Parse(credentials[0]));
                        if (gameClient != null && gameClient.MachineID == credentials[1])
                        {
                            this.UserID = gameClient.GetHabbo().ID;
                        }
                        else
                        {
                            this.Dispose();
                        }
                    }
                    break;
                case "PHOTOTXT":
                    {
                        if (this.UserID > 0)
                        {
                            this.PhotoText = message.ReadEvenPaddedString().Substring(1);
                        }
                    }
                    break;
                case "BINDATA":
                    {
                        if (this.UserID > 0 && this.PhotoText != null)
                        {
                            Dictionary<string, MUSPropListItem> props = message.ReadPropList();

                            Photo photo = new Photo();
                            photo.CS = MUSUtils.BytesToInt(props["cs"].Data);
                            photo.Image = props["image"].Data;
                            photo.Text = this.PhotoText;
                            photo.Time = TimeUtilies.GetUnixTimestamp();

                            this.PhotoText = null;

                            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                            {
                                dbClient.AddParamWithValue("userId", this.UserID);

                                uint itemId = (uint)dbClient.ExecuteQuery("INSERT INTO items(user_id, room_id, base_item, extra_data, wall_pos) VALUES(@userId, 0, 2268736439, '', '')");
                                if (itemId > 0)
                                {
                                    string data = JsonConvert.SerializeObject(photo);

                                    dbClient.AddParamWithValue("itemId", itemId);
                                    dbClient.AddParamWithValue("data", data);
                                    dbClient.ExecuteQuery("INSERT INTO items_data(item_id, data) VALUES(@itemId, @data)");
                                }

                                MUSServerMessage message_ = new MUSServerMessage("BINDATA_SAVED", MUSType.String);
                                message_.WriteEvenPaddedString(itemId.ToString());
                                this.SendMessage(message_);

                                Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID).GetHabbo().GetInventoryManager().AddInventoryItemToHand(new InventoryItem(itemId, 2268736439, ""));
                                Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID).SendMessage(Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID).GetHabbo().GetInventoryManager().OldSchoolGetHand("last"));
                            }
                        }
                    }
                    break;
                case "GETBINDATA":
                    {
                        uint itemId = uint.Parse(message.ReadEvenPaddedString().Split(' ')[0]);

                        RoomItemPhoto roomItem = (RoomItemPhoto)Skylight.GetGame().GetGameClientManager().GetGameClientById(this.UserID).GetHabbo().GetRoomSession().GetRoom().RoomItemManager.TryGetWallItem(itemId);
                        if (roomItem != null)
                        {
                            Dictionary<string, MUSPropListItem> data = new Dictionary<string, MUSPropListItem>();
                            data.Add("image", new MUSPropListItem(MUSType.Media, roomItem.Photo.Image));
                            data.Add("time", new MUSPropListItem(MUSType.String, Skylight.GetDefaultEncoding().GetBytes(TimeUtilies.UnixTimestampToDateTime(roomItem.Photo.Time).ToString())));
                            data.Add("cs", new MUSPropListItem(MUSType.Integer, MUSUtils.IntToBytes(roomItem.Photo.CS)));

                            MUSServerMessage message_ = new MUSServerMessage("BINARYDATA", MUSType.PropList);
                            message_.WritePropList(data);
                            this.SendMessage(message_);
                        }
                    }
                    break;
            }
        }

        public void ClientSendedData(IAsyncResult ar)
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

        private void SendMessage(MUSServerMessage message)
        {
            this.SendData(message.GetBytes());
        }


        public void SendData(byte[] data)
        {
            if (!this.Disposed)
            {
                try
                {
                    this.Socket.BeginSend(data, 0, data.Length, SocketFlags.None, this.SendCallback, this);
                }
                catch
                {
                    this.Dispose();
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
                this.SendCallback = null;
            }

            GC.SuppressFinalize(this);
        }
    }
}
