using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Messages;
using SkylightEmulator.Net;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.GameClients
{
    public class GameClient
    {
        private readonly long ID;
        private SocketsConnection connection;
        private DateTime PongOK;
        private Habbo Habbo;

        public GameClient(long id, SocketsConnection connection)
        {
            this.ID = id;
            this.connection = connection;
        }
        
        public bool Disconnected
        {
            get
            {
                return this.connection.IsDisconnected();
            }
        }

        public Habbo GetHabbo()
        {
            return this.Habbo;
        }

        public void Start()
        {
            if (this.connection != null)
            {
                this.PongOK = DateTime.Now;
                SocketsConnection.ReceivedData receivedDataDelegate = new SocketsConnection.ReceivedData(this.HandleData);
                this.connection.Start(receivedDataDelegate);
            }
        }

        public void Stop(string reason)
        {
            if (this.connection != null)
            {
                this.connection.Disconnect(reason);
            }
        }

        public void HandleDisconnection()
        {
            if (this.GetHabbo() != null)
            {
                this.GetHabbo().HandleDisconnection();
            }
        }

        public void LogIn(string sso)
        {
            if (string.IsNullOrWhiteSpace(sso))
            {
                this.SendNotif("Empty SSO ticket!");
            }
            else
            {
                UserDataFactory userData = new UserDataFactory(this.connection.GetIP(), sso, true);
                if (userData.IsUserLoaded)
                {
                    Habbo habbo = Authenicator.LoadHabbo(userData, this);
                    //CHECK BAN ON HERE!!!!
                    Skylight.GetGame().GetGameClientManager().DisconnectDoubleSession(habbo.ID);
                    this.Habbo = habbo;

                    ServerMessage Fuserights = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    Fuserights.Init(r63aOutgoing.Fuserights);
                    if (ServerConfiguration.EveryoneVIP)
                    {
                        Fuserights.AppendInt32(2);
                    }
                    else
                    {
                        //if (this.GetHabbo().method_20().HasSubscription("habbo_club"))
                        if (false)
                        {
                            Fuserights.AppendInt32(1);
                        }
                        else
                        {
                            Fuserights.AppendInt32(0);
                        }
                    }

                    //if (this.GetHabbo().HasFuse("acc_anyroomowner"))
                    if (false)
                    {
                        Fuserights.AppendInt32(7);
                    }
                    else
                    {
                        //if (this.GetHabbo().HasFuse("acc_anyroomrights"))
                        if (false)
                        {
                            Fuserights.AppendInt32(5);
                        }
                        else
                        {
                            //if (this.GetHabbo().HasFuse("acc_supporttool"))
                            if (false)
                            {
                                Fuserights.AppendInt32(4);
                            }
                            else
                            {
                                if (ServerConfiguration.EveryoneVIP /* got HC/VIP */)
                                {
                                    Fuserights.AppendInt32(2);
                                }
                                else
                                {
                                    Fuserights.AppendInt32(0);
                                }
                            }
                        }
                    }
                    this.SendMessage(Fuserights);

                    //send effects

                    ServerMessage AvaiblityStatus = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    AvaiblityStatus.Init(r63aOutgoing.AvaiblityStatus);
                    AvaiblityStatus.AppendInt32(1);
                    AvaiblityStatus.AppendInt32(0);
                    this.SendMessage(AvaiblityStatus);

                    ServerMessage AuthOK = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    AuthOK.Init(r63aOutgoing.AuthenicationOK);
                    this.SendMessage(AuthOK);

                    //if (this.GetHabbo().HasFuse("acc_supporttool"))
                    if (false)
                    {
                        //show mod tool and that kind stuff
                    }

                    ServerMessage Logging = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    Logging.Init(r63aOutgoing.Logging);
                    Logging.AppendBoolean(true);
                    this.SendMessage(Logging);

                    ServerMessage HomeRoom = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    HomeRoom.Init(r63aOutgoing.HomeRoom);
                    HomeRoom.AppendUInt(this.GetHabbo().HomeRoom);
                    this.SendMessage(HomeRoom);

                    ServerMessage FavouriteRooms = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    FavouriteRooms.Init(r63aOutgoing.FavouriteRooms);
                    FavouriteRooms.AppendInt32(30);
                    FavouriteRooms.AppendInt32(this.GetHabbo().FavouriteRooms.Count);
                    foreach (uint current in this.GetHabbo().FavouriteRooms)
                    {
                        FavouriteRooms.AppendUInt(current);
                    }
                    this.SendMessage(FavouriteRooms);

                    //activity bonus

                    this.SendNotif("This emulator is still in HUGE development state :)");
                }
                else
                {
                    this.SendNotif("Invalid SSO ticket!");
                }
            }
        }

        public void SendNotif(string message)
        {
            this.SendNotif(0, message);
        }

        public void SendNotif(int type, string message)
        {
            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            switch(type)
            {
                case 0:
                    message_.Init(r63aOutgoing.SendNotif);
                    message_.AppendStringWithBreak(message);
                    break;
                default:
                    message_.Init(r63aOutgoing.SendNotif);
                    break;
            }
            this.SendMessage(message_);
        }

        public void HandleData(byte[] data)
        {
            if (data[0] == 64)
            {
                int i = 0;
                while (i < data.Length)
                {
                    try
                    {
                        int packetLenght = Base64Encoding.DecodeInt32(new byte[]
						{
							data[i++],
							data[i++],
							data[i++]
						});

                        uint packetId = Base64Encoding.DecodeUInt32(new byte[]
						{
							data[i++],
							data[i++]
						});

                        byte[] packet = new byte[packetLenght - 2];
                        for (int j = 0; j < packet.Length; j++)
                        {
                            packet[j] = data[i++];
                        }

                        ClientMessage message = BasicUtilies.GetRevisionClientMessage(Skylight.Revision);
                        message.Init(packetId, packet);

                        if (message != null)
                        {
                            IncomingPacket incomingPacket;
                            if (Skylight.GetPacketManager().HandleIncoming(message.GetID(), out incomingPacket))
                            {
                                incomingPacket.Handle(this, message);
                            }

                            try
                            {
                                if (Skylight.GetConfig()["debug"] == "1")
                                {
                                    Logging.WriteLine(string.Concat(new object[]
									{
										"[",
										this.ID,
										"] --> [",
										message.GetID(),
										"] ",
										message.GetHeader(),
										message.GetBody()
									}));
                                }
                            }
                            catch
                            {
                            }

                            try
                            {
                                if (Skylight.GetConfig()["show.unhandled.packets"] == "1")
                                {
                                    if (!Skylight.GetPacketManager().PacketExits(packetId))
                                    {
                                        try
                                        {
                                            Logging.WriteLine(string.Concat(new object[]
									    {
										    "Packet dont exit: [",
										    this.ID,
										    "] --> [",
										    message.GetID(),
										    "] ",
									    	message.GetHeader(),
									    	message.GetBody()
									    }));
                                        }
                                        catch
                                        {
                                            Console.WriteLine("Packet dont exit: " + packetId);
                                        }
                                    }
                                }
                            }
                            catch
                            {

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Stop("Error parse packets: " + ex.ToString());
                        if (ex.GetType() == typeof(IndexOutOfRangeException))
                            return;

                        Logging.LogException("Error: " + ex.ToString());
                    }
                }
            }
            else
            {
                this.SendData(Skylight.GetDefaultEncoding().GetBytes(CrossdomainPolicy.GetXmlPolicy()));
            }
        }

        public SocketsConnection GetConnection()
        {
            return this.connection;
        }

        public void SendData(byte[] data)
        {
            if (this.GetConnection() != null)
            {
                this.GetConnection().SendData(data);
            }
        }

        public void SendMessage(ServerMessage message)
        {
            if (this.GetConnection() != null)
            {
                this.GetConnection().SendData(message.GetBytes());
            }
        }
    }
}
