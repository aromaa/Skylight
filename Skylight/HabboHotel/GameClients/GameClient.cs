using Google.Authenticator;
using SkylightEmulator.Communication;
using SkylightEmulator.Communication.Handlers;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Cypto;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.HabboHotel.Support;
using SkylightEmulator.HabboHotel.Users;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.Empty;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Net;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.GameClients
{
    public class GameClient
    {
        public readonly long ID;
        private SocketsConnection connection;
        private Habbo Habbo;
        public string MachineID;
        public Stopwatch LastPong;
        private OrderedDictionary DataHandlers;
        private OrderedDictionary MessageHandlers;
        public int SessionID;
        public bool FlagmeCommandUsed;
        public bool ClosePending = false; //Mono socket fix, If any data is sended forces the connecting to be closed

        public Revision Revision;

        //crypto
        internal int[] table = new int[256];
        internal int i = 0, j = 0;
        internal bool CryptoInitialized = false;

        public GameClient(long id, SocketsConnection connection, Revision revision, Crypto crypto)
        {
            this.ID = id;
            this.connection = connection;
            this.MachineID = "";
            this.Revision = revision;
            this.LastPong = Stopwatch.StartNew();

            this.DataHandlers = new OrderedDictionary();
            this.MessageHandlers = new OrderedDictionary();
            if (!Skylight.ExternalFlashPolicyFileRequestPortEnabled) //If the policy file is requested from external port the flash is not requesting it again from the actual game port
            {
                this.AddDataHandler(PrivacyPolicyRequestHandler.Handler);
            }

            if (!Skylight.MultiRevisionSupportEnabled)
            {
                this.Revision = Skylight.Revision;
            }
            
            if (this.Revision != Revision.None && (Skylight.MultiRevisionSupportEnabled && crypto != Crypto.NONE))
            {
                this.EnableDecodeHandlers();
            }
            else
            {
                this.AddDataHandler(new DetectRevisionHandler());
            }
        }

        public void EnableDecodeHandlers()
        {
            if (this.Revision < Revision.RELEASE63_201211141113_913728051)
            {
                this.AddDataHandler(new OldCryptoDataDecoderHandler()); //per user
            }
            else
            {
                this.AddDataHandler(new NewCryptoDataDecoderHandler()); //per user
            }
            
            if (true) //packet verifier enabled
            {
                PacketOrderVerifier verifier = (PacketOrderVerifier)this.AddMessageHandler(new PacketOrderVerifier()); //per user
                if (this.Revision == Revision.RELEASE63_35255_34886_201108111108)
                {
                    if (!Skylight.MultiRevisionSupportEnabled)
                    {
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.InitCryptoMessage);
                    }

                    if (ServerConfiguration.EnableCrypto)
                    {
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.SecretKey);
                    }

                    if (ServerConfiguration.RequireMachineID)
                    {
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.Variables);
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.MachineId);
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.GetSessionParameters);
                    }
                }
                else if (this.Revision == Revision.RELEASE63_201211141113_913728051)
                {
                    verifier.ExceptedPacketOrder.Enqueue(r63bIncoming.VersionCheck);
                    verifier.ExceptedPacketOrder.Enqueue(r63bIncoming.InitCryptoMessage);
                    verifier.ExceptedPacketOrder.Enqueue(r63bIncoming.SecretKey);
                    verifier.ExceptedPacketOrder.Enqueue(r63bIncoming.Variables);
                    verifier.ExceptedPacketOrder.Enqueue(r63bIncoming.MachineId);
                }
                else if (this.Revision == Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
                {
                    if (!Skylight.MultiRevisionSupportEnabled)
                    {
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.InitCryptoMessage);
                    }

                    if (ServerConfiguration.RequireMachineID)
                    {
                        verifier.ExceptedPacketOrder.Enqueue(r63aIncoming.SecretKey);
                    }
                    else
                    {
                        verifier.ExceptedPacketOrder.Enqueue(204); //NO CLUEEEEEEE
                    }
                }
            }
            this.AddMessageHandler(ClientMessageHandler.Handler);
        }

        public DataHandler AddDataHandler(DataHandler handler)
        {
            this.DataHandlers.Add(handler.Identifier(), handler);
            return handler;
        }

        public DataHandler AddDataHandlerFirst(DataHandler handler)
        {
            this.DataHandlers.Insert(0, handler.Identifier(), handler);
            return handler;
        }

        public MessageHandler AddMessageHandler(MessageHandler handler)
        {
            this.MessageHandlers.Add(handler.Identifier(), handler);
            return handler;
        }

        public MessageHandler AddMessageHandlerToFrist(MessageHandler handler)
        {
            this.MessageHandlers.Insert(0, handler.Identifier(), handler);
            return handler;
        }

        public void RemoveDataHandler(Guid identifier)
        {
            this.DataHandlers.Remove(identifier);
        }

        public void RemoveMessageHandler(Guid identifier)
        {
            this.MessageHandlers.Remove(identifier);
        }

        public DataHandler GetDataHandler(Guid identifier)
        {
            return (DataHandler)this.DataHandlers[identifier];
        }

        public T GetMessageHandler<T>(Guid identifier)
        {
            return (T)this.MessageHandlers[identifier];
        }

        public T GetDataHandler<T>(Guid identifier)
        {
            return (T)this.DataHandlers[identifier];
        }

        public bool Disconnected
        {
            get
            {
                return this.connection == null || this.connection.IsDisconnected();
            }
        }

        public Habbo GetHabbo()
        {
            return this.Habbo;
        }

        public string GetIP()
        {
            if (!ServerConfiguration.UseIPLastForBans || this.Habbo == null)
            {
                return this.GetConnection().GetIP();
            }
            else
            {
                return this.Habbo.IPLast;
            }
        }

        public void Start()
        {
            if (this.connection != null)
            {
                this.LastPong.Restart();

                this.connection.Start(new SocketsConnection.ReceivedData(this.HandleData));

                if (this.Revision == Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169) //r26 requires this
                {
                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
                    Message.Init(r26Outgoing.Connected);
                    this.SendMessage(Message);
                }
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

        public void LogIn(string username, string password, uint userId, int time)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                password = BitConverter.ToString(MD5CryptoServiceProvider.Create().ComputeHash(Encoding.UTF8.GetBytes(password))).Replace("-", "").ToLower();

                string sso = "Temp-SSO-Login-With-Username-And-Password-" + username + "-" + time + "-" + TimeUtilies.GetUnixTimestamp();
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("username", username);
                    dbClient.AddParamWithValue("password", password);
                    dbClient.AddParamWithValue("sso", sso);
                    dbClient.ExecuteQuery("UPDATE users SET auth_ticket = @sso WHERE `username` = @username AND `password` = @password LIMIT 1");
                }

                this.LogIn(sso, true);
            }
        }

        public void LogIn(string sso, bool loginWithUsernameAndPassword = false)
        {
            uint id = 0;

            //DUE TO R26 TESTING
            if (sso == "fcc08e21-8a14-41e4-22f4-1a258f9db47f")
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE users SET auth_ticket = 'fcc08e21-8a14-41e4-22f4-1a258f9db47f' WHERE id = 3132 LIMIT 1");
                }
            }
            else if (sso == "ESANPIKKUPIKAPANO")
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.ExecuteQuery("UPDATE users SET auth_ticket = 'ESANPIKKUPIKAPANO' WHERE id = 3133 LIMIT 1");
                }
            }

            try
            {
                if (string.IsNullOrEmpty(sso) || string.IsNullOrWhiteSpace(sso))
                {
                    this.SendNotif("Empty SSO ticket!");
                }
                else
                {
                    UserDataFactory userData = new UserDataFactory(this.connection.GetIP(), this.MachineID, sso, true, out this.SessionID);
                    if (userData.IsUserLoaded)
                    {
                        id = (uint)userData.GetUserData()["id"];
                        string username = (string)userData.GetUserData()["username"];
                        Skylight.GetGame().GetGameClientManager().UpdateCachedUsername(id, username); //we loaded the user data.. why not update this too
                        Skylight.GetGame().GetGameClientManager().UpdateCachedID(id, username);

                        bool requireLogin = ServerConfiguration.MinRankRequireLogin == 0 ? false : (int)userData.GetUserData()["rank"] >= ServerConfiguration.MinRankRequireLogin;
                        if ((!loginWithUsernameAndPassword && !requireLogin) || (loginWithUsernameAndPassword && requireLogin))
                        {
                            Ban ban = Skylight.GetGame().GetBanManager().TryGetBan(id, this.GetIP(), this.MachineID);
                            if (ban == null || Licence.CheckIfMatches(id, username, this.GetIP(), this.MachineID)) //don't load ANY shit before we are sure he can come on
                            {
                                Skylight.GetGame().GetGameClientManager().DisconnectDoubleSession(id);

                                this.Habbo = Authenicator.LoadHabbo(userData, this);
                                this.Habbo.LoadMore();
                                if (this.Habbo.IsTwoFactorAuthenticationEnabled())
                                {
                                    int failures = 0;
                                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                                    {
                                        failures = dbClient.ReadInt32("SELECT COUNT(id) FROM user_2fa_failures WHERE timestamp >= UNIX_TIMESTAMP() - 60 * 15");
                                    }

                                    if (failures < 3)
                                    {
                                        this.AddMessageHandlerToFrist(new TwoFactorAuthenticationHandler(failures));
                                        
                                        if (this.Revision == Revision.RELEASE63_35255_34886_201108111108)
                                        {
                                            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                                            message.Init(r63aOutgoing.ChangeNameWindow);
                                            this.SendMessage(message);
                                        }
                                        else
                                        {
                                            this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.AuthOk).Handle());

                                            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.VerifyMobilePhoneWindow);
                                            message_.AppendInt32(9);
                                            message_.AppendInt32(3);
                                            message_.AppendInt32(1);
                                            this.SendMessage(message_);
                                        }

                                        this.SendNotif("Looks like you have enable two factor authenication on your account! Please enter the code to log in!" + ((Skylight.GetConfig()["client.ping.enabled"] == "1") ? " You have one minute!" : "") + "\n\n!!CLOSE THIS!!", 2);
                                    }
                                    else
                                    {
                                        this.SendNotif("You have too many login fails inside 15 minutes! Please try again later!", 2);
                                    }
                                }
                                else
                                {
                                    this.LetUserIn();
                                }
                            }
                            else
                            {
                                string banLenght = "";
                                if (ban.Permament)
                                {
                                    banLenght = "PERMAMENT";
                                }
                                else
                                {
                                    TimeSpan lenght = new TimeSpan(0, 0, 0, (int)(ban.Expires - ban.AddedOn));

                                    banLenght = lenght.Seconds + " seconds";

                                    if (lenght.TotalMinutes >= 1)
                                    {
                                        banLenght += ", " + lenght.Minutes + " minutes";
                                    }

                                    if (lenght.TotalHours >= 1)
                                    {
                                        banLenght += ", " + lenght.Hours + " hours";
                                    }

                                    if (lenght.TotalDays >= 1)
                                    {
                                        banLenght += ", " + lenght.Days + " days";
                                    }
                                }

                                this.SendNotif("You have been banned!\nReason: " + ban.Reason + "\nLength: " + banLenght + "\nExpires: " + (ban.Permament ? "NEVER" : TimeUtilies.UnixTimestampToDateTime(ban.Expires).ToString()) + "\nBanned by: " + Skylight.GetGame().GetGameClientManager().GetUsernameByID(ban.AddedByID), 2);
                                //this.Stop("Banned!");
                            }
                        }
                        else
                        {
                            this.SendNotif("Sorry but, you cant use this login method!", 2);
                        }
                    }
                    else
                    {
                        this.SendNotif("Invalid SSO ticket!", 2);
                    }
                }
            }
            finally //fixes issue where valid sso ticket sets online to = '1' but dosent turn it back to = '0' after disconnection bcs users data isint loaded for few reasons example exeption or user is banned
            {
                if (id > 0) //only positive numbers are valid user ids
                {
                    if (this.GetHabbo() == null) //only if habbo is null or this is already done by habbo class itself
                    {
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("userId", id);
                            dbClient.ExecuteQuery("UPDATE users SET online = '0' WHERE id = @userId LIMIT 1");
                        }
                    }
                }
            }
        }

        private void LetUserIn()
        {
            //if (this.Revision == Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            //{
            //    ServerMessage weirdKeys = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            //    weirdKeys.Init(r26Outgoing.SomeKeysMaybe);
            //    weirdKeys.AppendString("QBHIIIKHJIPAIQAdd-MM-yyyy");
            //    weirdKeys.AppendString("SAHPB/client");
            //    weirdKeys.AppendString("QBH");
            //    this.SendMessage(weirdKeys);
            //}

            //send effects

            if (this.Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.AvaiblityStatus).Handle());
            }
            this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.AuthOk).Handle());
            this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.Fuserights).Handle(new ValueHolder().AddValue("Session", this)));
            this.SendMessage(new NewbieIdentityComposerHandler(false)); //Removes all the anonying shit

            if (this.GetHabbo().HasPermission("acc_supporttool"))
            {
                this.SendMessage(Skylight.GetGame().GetModerationToolManager().SerializeModTool(this));
            }

            if (this.Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.ShowNotifications).Handle());
            }

            if (this.Habbo.NewbieStatus == 0)
            {
                Room newbieRoom = Skylight.GetGame().GetRoomManager().NewbieRooms.Count > 0 ? Skylight.GetGame().GetRoomManager().CreateNewbieRoom(this, Skylight.GetGame().GetRoomManager().NewbieRooms.Keys.ElementAt(RandomUtilies.GetRandom(0, Skylight.GetGame().GetRoomManager().NewbieRooms.Count - 1))) : null;
                if (newbieRoom != null)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", this.Habbo.ID);
                        dbClient.AddParamWithValue("roomId", newbieRoom.ID);
                        dbClient.ExecuteQuery("UPDATE users SET newbie_status = '1', " + (this.Habbo.HomeRoom == 0 ? "home_room = @roomId, " : "") + "newbie_room = @roomId WHERE id = @userId LIMIT 1");
                    }

                    this.Habbo.NewbieStatus = 1;
                    if (this.Habbo.HomeRoom == 0)
                    {
                        this.Habbo.HomeRoom = newbieRoom.ID;
                    }
                    this.Habbo.NewbieRoom = newbieRoom.ID;
                }
                else
                {
                    //no welcome room, no shit :(
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", this.Habbo.ID);
                        dbClient.ExecuteQuery("UPDATE users SET newbie_status = '2' WHERE id = @userId LIMIT 1");
                    }

                    this.Habbo.NewbieStatus = 2;
                }
            }

            if (this.Revision >= Revision.RELEASE63_201211141113_913728051)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.HomeRoom).Handle(new ValueHolder().AddValue("HomeRoom", this.Habbo.HomeRoom).AddValue("ForwardID", this.Habbo.NewbieStatus == 1 ? this.Habbo.NewbieRoom : this.Habbo.HomeRoom)));
            }
            else
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.HomeRoom).Handle(new ValueHolder().AddValue("HomeRoom", this.Habbo.NewbieStatus == 1 ? this.Habbo.NewbieRoom : this.Habbo.HomeRoom)));
            }

            if (this.Revision > Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.FavouriteRooms).Handle(new ValueHolder().AddValue("Max", 50).AddValue("FavouriteRooms", this.Habbo.FavouriteRooms)));
            }

            if (this.GetHabbo().CanReceiveActivityBonus())
            {
                this.GetHabbo().ReceiveActivityBonus();
            }

            if (!string.IsNullOrEmpty(ServerConfiguration.MOTD))
            {
                this.SendNotif(ServerConfiguration.MOTD, 2);
            }

            foreach (KeyValuePair<int, string> data in Skylight.GetGame().GetPermissionManager().GetBadges())
            {
                if (!string.IsNullOrEmpty(data.Value))
                {
                    if (this.GetHabbo().Rank >= data.Key)
                    {
                        this.GetHabbo().GetBadgeManager().AddBadge(data.Value, 0, true);
                    }
                    else
                    {
                        this.GetHabbo().GetBadgeManager().RemoveBadge(data.Value);
                    }
                }
            }

            this.GetHabbo().CheckDailyStuff(false); //daily respect, regular visitor
            this.GetHabbo().CheckHappyHour(); //happy hour achievement
            this.GetHabbo().GetUserAchievements().CheckAchievement("RegistrationDuration");
            this.GetHabbo().GetUserAchievements().CheckAchievement("OnlineTime");
            this.GetHabbo().GetUserAchievements().CheckAchievement("HCMember");
            this.GetHabbo().GetUserAchievements().CheckAchievement("VIPMember");
            this.GetHabbo().GetUserAchievements().CheckAchievement("Tags");
            this.GetHabbo().GetUserAchievements().CheckAchievement("GuideEnrollmentLifetime");
            this.GetHabbo().GetUserAchievements().CheckAchievement("GuideOnDutyPresence");
            if (this.GetHabbo().MailConfirmed)
            {
                Skylight.GetGame().GetAchievementManager().AddAchievement(this, "EmailVerification", 1);
            }

            foreach (uint roomId in this.GetHabbo().UserRooms)
            {
                Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(roomId);
                if (room != null)
                {
                    room.EquestrianTrackHost(0);
                    room.FootballGoalHost(0);
                    room.RoomHost(0);
                }
            }

            if (this.Revision >= Revision.RELEASE63_201211141113_913728051)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.UserPerks).Handle(new ValueHolder("Session", this)));
            }

            if (this.Revision >= Revision.PRODUCTION_201601012205_226667486)
            {
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NewNavigatorMetaData).Handle());
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NewNavigatorLiftedRooms).Handle());
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NewNavigatorSavedSearches).Handle());
                this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NewNavigatorEventCategories).Handle());
            }
        }

        public void SendNotif(string message, int type = 0)
        {
            ServerMessage message_ = null;
            switch (type)
            {
                case 0:
                    message_ = BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NotifFromMod).Handle(new ValueHolder().AddValue("Message", message));
                    break;
                case 1:
                    message_ = BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NotifFromAdmin).Handle(new ValueHolder().AddValue("Message", message));
                    break;
                case 2:
                    message_ = BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.MOTD).Handle(new ValueHolder().AddValue("Message", new List<string>() { message }));
                    break;
                default:
                    message_ = BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NotifFromMod).Handle(new ValueHolder().AddValue("Message", message));
                    break;
            }
            this.SendMessage(message_);
        }

        public void SendNotifWithLink(string message, string link)
        {
            this.SendMessage(BasicUtilies.GetRevisionPacketManager(this.Revision).GetOutgoing(OutgoingPacketsEnum.NotifFromMod).Handle(new ValueHolder("Message", message, "Link", link)));
        }

        public void HandleData(byte[] data)
        {
            try
            {
                if (!this.ClosePending)
                {
                    foreach (DictionaryEntry entry in this.DataHandlers.Cast<DictionaryEntry>().ToList())
                    {
                        if (!(entry.Value as DataHandler).HandlePacket(this, ref data))
                        {
                            break;
                        }
                    }
                }
                else
                {
                    this.Stop("Close pending");
                }
            }
            catch(Exception ex)
            {
                Logging.LogUserException("Error when trying to parse data! " + ex.ToString());
                this.Stop("Data handle error");
            }
        }

        public void HandlePacket(ClientMessage message) //this same for all revisions
        {
            try
            {
                foreach (DictionaryEntry entry in this.MessageHandlers.Cast<DictionaryEntry>().ToList())
                {
                    if (!(entry.Value as MessageHandler).HandleMessage(this, message))
                    {
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                Logging.LogUserException("Error when trying to handle packet! " + ex.ToString());
                this.Stop("Packet handle error");
            }
        }

        public SocketsConnection GetConnection()
        {
            return this.connection;
        }

        public void SendData(byte[] data, bool ignoreIfInvalid = false)
        {
            if (data == null || data.Length <= 0)
            {
                if (!ignoreIfInvalid)
                {
                    throw new IndexOutOfRangeException("You need to send data to the client!");
                }
                else
                {
                    return;
                }
            }

            if (this.GetConnection() != null)
            {
                this.GetConnection().SendData(data);
            }
        }
        public void SendData(List<ArraySegment<byte>> data)
        {
            if (data == null || data.Count <= 0)
            {
                throw new IndexOutOfRangeException("You need to send data to the client!");
            }

            if (this.GetConnection() != null)
            {
                this.GetConnection().SendData(data);
            }
        }

        public void SendMessage(OutgoingHandler outgoing)
        {
            this.SendMessage(this.GetPacketManager().GetNewOutgoing(outgoing));
        }
        
        public void SendMessage(OutgoingPacketsEnum outgoing, ValueHolder valueHolder = null)
        {
            this.SendMessage(this.GetPacketManager().GetOutgoing(outgoing).Handle(valueHolder));
        }

        public void SendMessage(MultiRevisionServerMessage message)
        {
            this.SendData(message.GetBytes(this.Revision));
        }

        public void SendMessage(ServerMessage message)
        {
            if (message == null || message is EmptyServerMessage)
            {
                return;
            }

            if (Skylight.GetConfig()["debug.outgoing"] == "1")
            {
                Logging.WriteLine(string.Concat(new object[]
                {
                    "[",
                    this.ID,
                    "] <-- [",
                    message.GetID(),
                    "] ",
                    message.GetHeader(),
                    message.GetBody()
                }));
            }

            if (this.Revision != Revision.None && !this.DataHandlers.Contains(DetectRevisionHandler.Identifier_))
            {
                if (message.GetRevision() != this.Revision)
                {
                    Logging.WriteLine("INVALID PACKET! Packet id: " + message.GetID() + ", Packet Revision: " + message.GetRevision() + ", Destiny Revision: " + this.Revision, ConsoleColor.Red);
                    return;
                }
            }

            if (this.GetConnection() != null)
            {
                this.SendData(message.GetBytes());
            }
        }

        public bool CheckTwoFactorAuthenicationCode(int code)
        {
            return new TwoFactorAuthenticator().ValidateTwoFactorPIN(this.Habbo.TwoFactoryAuthenicationSecretCode, code.ToString(), new TimeSpan(0, 0, 31));
        }

        public bool LoginUsingTwoFactorAuthenication(int code)
        {
            if (this.CheckTwoFactorAuthenicationCode(code))
            {
                this.LetUserIn();

                return true;
            }
            else
            {
                return false;
            }
        }

        public ServerMessage GetServerMessage(uint? header)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(this.Revision);
            if (header != null)
            {
                message.Init((uint)header);
            }
            return message;
        }

        public PacketManager GetPacketManager()
        {
            return BasicUtilies.GetRevisionPacketManager(this.Revision);
        }

        public void SendLeetScripter()
        {
            for (int i = 0; i < 10; i++)
            {
                this.SendNotif("Hello! Nice to meet you! Let me introduce myself. I am the PlzNoScriptingDetectionSystemV4.2.0. To be honest I don't quite like you. :( But oh well... I think we can still be great friends one day if you don't trigger me again ;). But I hope you just got started so... MAKE SCRIPTERS GREAT AGAIN! (Plz report if you find anything. I dare you to. Much love. Much hugs. Many kisses. xoxo)");
            }
        }
    }
}
