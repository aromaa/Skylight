using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class ModerationToolManager
    {
        private Dictionary<string, List<ModerationIssue>> Issues;
        private List<string> UserPresents;
        private List<string> RoomPresents;
        private Dictionary<uint, SupportTicket> SupportTickets;

        public ModerationToolManager()
        {
            this.Issues = new Dictionary<string, List<ModerationIssue>>();
            this.UserPresents = new List<string>();
            this.RoomPresents = new List<string>();
            this.SupportTickets = new Dictionary<uint,SupportTicket>();
        }

        public void LoadIssues(DatabaseClient dbClient)
        {
            Logging.Write("Loading moderation issues... ");

            DataTable issues = dbClient.ReadDataTable("SELECT issue, solution, category FROM moderation_issues");
            if (issues != null && issues.Rows.Count > 0)
            {
                foreach(DataRow dataRow in issues.Rows)
                {
                    string category = (string)dataRow["category"];

                    if (!this.Issues.ContainsKey(category))
                    {
                        this.Issues.Add(category, new List<ModerationIssue>());
                    }
                    
                    this.Issues[category].Add(new ModerationIssue((string)dataRow["issue"], (string)dataRow["solution"]));
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadPresents(DatabaseClient dbClient)
        {
            Logging.Write("Loading moderation presents... ");

            DataTable presents = dbClient.ReadDataTable("SELECT type, present FROM moderation_presents");
            if (presents != null && presents.Rows.Count > 0)
            {
                foreach(DataRow dataRow in presents.Rows)
                {
                    string type = (string)dataRow["type"];
                    if (type == "user")
                    {
                        this.UserPresents.Add((string)dataRow["present"]);
                    }
                    else if (type == "room")
                    {
                        this.RoomPresents.Add((string)dataRow["present"]);
                    }
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public void LoadSupportTickets(DatabaseClient dbClient)
        {
            Logging.Write("Loading support tickets... ");

            DataTable tickets = dbClient.ReadDataTable("SELECT * FROM moderation_tickets");
            if (tickets != null && tickets.Rows.Count > 0)
            {
                foreach(DataRow dataRow in tickets.Rows)
                {
                    uint id = (uint)dataRow["id"];
                    string sStatus = (string)dataRow["status"];

                    SupportTicketStatus status = SupportTicketStatus.Open;
                    switch(sStatus)
                    {
                        case "open":
                            {
                                status = SupportTicketStatus.Open;
                                break;
                            }
                        case "picked":
                            {
                                status = SupportTicketStatus.Picked;
                                break;
                            }
                        case "resolved":
                            {
                                status = SupportTicketStatus.Resolved;
                                break;
                            }
                        case "abusive":
                            {
                                status = SupportTicketStatus.Abusive;
                                break;
                            }
                        case "invalid":
                            {
                                status = SupportTicketStatus.Invalid;
                                break;
                            }
                        case "deleted":
                            {
                                status = SupportTicketStatus.Deleted;
                                break;
                            }
                        default:
                            {
                                status = SupportTicketStatus.Open;
                                break;
                            }
                    }

                    this.SupportTickets.Add(id, new SupportTicket(id, (int)dataRow["score"], (int)dataRow["type"], status, (uint)dataRow["sender_id"], (uint)dataRow["reported_id"], (uint)dataRow["picker_id"], (string)dataRow["message"], (uint)dataRow["room_id"], (string)dataRow["room_name"], (double)dataRow["timestamp"]));
                }
            }

            Logging.WriteLine("completed!", ConsoleColor.Green);
        }

        public ServerMessage SerializeModTool(GameClient session)
        {
            return BasicUtilies.GetRevisionPacketManager(session.Revision).GetOutgoing(OutgoingPacketsEnum.ModTool).Handle(new ValueHolder().AddValue("Session", session));
        }

        public ServerMessage SerializeUserInfo(uint userId)
        {
            DataRow dataRow = null;
            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
            if (target != null && target.GetHabbo() != null)
            {
                dataRow = target.GetHabbo().GetUserDataFactory().GetUserData();
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userId", userId);
                    dataRow = dbClient.ReadDataRow("SELECT username, account_created, last_online, ip_last FROM users WHERE id = @userId LIMIT 1");
                }
            }

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ModToolUserInfo);
            message.AppendUInt(userId);
            message.AppendString((string)dataRow["username"]);
            message.AppendInt32((int)Math.Ceiling((TimeUtilies.GetUnixTimestamp() - (double)dataRow["account_created"]) / 60.0)); //minutes since account created
            message.AppendInt32((int)Math.Ceiling((TimeUtilies.GetUnixTimestamp() - (double)dataRow["last_online"]) / 60.0)); //minutes since last login
            message.AppendBoolean(target != null);
            message.AppendInt32(this.SupportTickets.Values.Where(t => t.SenderID == userId).Count()); //cfhs
            message.AppendInt32(this.SupportTickets.Values.Where(t => t.Status == SupportTicketStatus.Abusive && t.SenderID == userId).Count()); //cfhs abusive
            message.AppendInt32(Skylight.GetGame().GetCautionManager().GetCauctionsByUserID(userId)); //cauctions
            message.AppendInt32(Skylight.GetGame().GetBanManager().GetBanCountByUserID(userId)); //bans
            return message;
        }

        public ServerMessage SerializeRoomInfo(RoomData data)
        {
            Room room = Skylight.GetGame().GetRoomManager().TryGetRoom(data.ID);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ModToolRoomInfo);
            message.AppendUInt(data.ID);
            message.AppendInt32(data.UsersNow);
            message.AppendBoolean(room != null ? room.RoomUserManager.GetUserByID(data.OwnerID) != null : false); //owner in room
            message.AppendUInt(data.OwnerID);
            message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(data.OwnerID));

            //room data
            message.AppendBoolean(true); //MUST be true for room information
            message.AppendString(data.Name);
            message.AppendString(data.Description);
            message.AppendInt32(data.Tags.Count);
            foreach(string tag in data.Tags)
            {
                message.AppendString(tag);
            }

            //event data
            if (room != null && room.RoomEvent != null)
            {
                message.AppendBoolean(true); //have event
                message.AppendString(room.RoomEvent.Name);
                message.AppendString(room.RoomEvent.Description);
                message.AppendInt32(room.RoomEvent.Tags.Count); //event tags count
                foreach (string tag in room.RoomEvent.Tags)
                {
                    message.AppendString(tag);
                }
            }
            else
            {
                message.AppendBoolean(false);
            }

            return message;
        }

        public bool HaveOpenSupportTickets(uint userId)
        {
            return this.SupportTickets.Values.Any(t => t.Status == SupportTicketStatus.Open && t.SenderID == userId);
        }

        public void DeletePendingSupportTicket(uint userId)
        {
            foreach (SupportTicket ticket in this.SupportTickets.Values.Where(t => t.Status == SupportTicketStatus.Open && t.SenderID == userId))
            {
                ticket.Delete(true);
                this.SerializeSupportTicketToMods(ticket);
                break;
            }
        }

        public SupportTicket TryGetSupportTicket(uint id)
        {
            SupportTicket ticket = null;
            this.SupportTickets.TryGetValue(id, out ticket);
            return ticket;
        }

        public void CallForHelp(GameClient session, string issue, int topic, uint reportedId)
        {
            uint roomId = session.GetHabbo().GetRoomSession().CurrentRoomID;
            string roomName = "";
            if (roomId > 0)
            {
                roomName = session.GetHabbo().GetRoomSession().CurrentRoomRoomUser.Room.RoomData.Name;
            }

            double timestamp = TimeUtilies.GetUnixTimestamp();
            uint ticketId = 0;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("score", 1);
                dbClient.AddParamWithValue("type", topic);
                dbClient.AddParamWithValue("senderId", session.GetHabbo().ID);
                dbClient.AddParamWithValue("reportedId", reportedId);
                dbClient.AddParamWithValue("message", issue);
                dbClient.AddParamWithValue("roomId", roomId);
                dbClient.AddParamWithValue("roomName", roomName);
                dbClient.AddParamWithValue("timestamp", timestamp);

                ticketId = (uint)dbClient.ExecuteQuery("INSERT INTO moderation_tickets(score, type, status, sender_id, reported_id, picker_id, message, room_id, room_name, timestamp) VALUES(@score, @type, 'open', @senderId, @reportedId, '0', @message, @roomId, @roomName, @timestamp)");
            }

            if (ticketId > 0)
            {
                SupportTicket ticket = new SupportTicket(ticketId, 1, topic, SupportTicketStatus.Open, session.GetHabbo().ID, reportedId, 0, issue, roomId, roomName, timestamp);
                this.SupportTickets.Add(ticketId, ticket);
                this.SerializeSupportTicketToMods(ticket);
            }
        }

        public void SerializeSupportTicketToMods(SupportTicket ticket)
        {
            foreach(GameClient session in Skylight.GetGame().GetGameClientManager().GetClients())
            {
                try
                {
                    if (session.GetHabbo().HasPermission("acc_supporttool"))
                    {
                        session.SendMessage(ticket.Serialize());
                    }
                }
                catch
                {

                }
            }
        }

        public ServerMessage GetRoomChatlog(uint roomId)
        {
            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ModeratorRoomChatlog);
            message.AppendBoolean(roomData != null ? roomData.IsPublicRoom : false);
            message.AppendUInt(roomId);
            message.AppendString(roomData != null ? roomData.Name : "![UNABLE TO FIND ROOM]!");

            List<RoomMessage> chatlog = Skylight.GetGame().GetChatlogManager().GetRoomChatlog(roomId);
            message.AppendInt32(chatlog.Count);
            foreach(RoomMessage msg in chatlog)
            {
                DateTime time = msg.GetDate();
                message.AppendInt32(time.Hour);
                message.AppendInt32(time.Minute);
                message.AppendUInt(msg.UserID);
                message.AppendString(msg.Username);
                if (!msg.Message.StartsWith("Whisper" + (char) 9))
                {
                    message.AppendString(msg.Message);
                }
                else
                {
                    string[] data = msg.Message.Split((char)9);
                    message.AppendString("<Whisper to " + Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(data[1])) + ">: " + data[2]);
                }
            }
            return message;
        }

        public ServerMessage GetRoomVisits(uint userId)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ModeratorRoomVisits);
            message.AppendUInt(userId);
            message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId));

            List<Roomvisit> roomvisits = Skylight.GetGame().GetRoomvisitManager().GetRoomvisits(userId);
            message.AppendInt32(roomvisits.Count);
            foreach(Roomvisit roomvisit in roomvisits)
            {
                RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomvisit.RoomID);
                message.AppendBoolean(roomData != null ? roomData.IsPublicRoom : false);
                message.AppendUInt(roomvisit.RoomID);
                message.AppendString(roomData != null ? roomData.Name : "![UNABLE TO FIND ROOM]!");

                DateTime entry = roomvisit.GetEntryDate();
                message.AppendInt32(entry.Hour);
                message.AppendInt32(entry.Minute);
            }
            return message;
        }

        public ServerMessage GetUserChatlog(uint userId)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.ModeratorUserChatlog);
            message.AppendUInt(userId);
            message.AppendString(Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId));

            List<Roomvisit> roomvisits = Skylight.GetGame().GetRoomvisitManager().GetRoomvisits(userId);
            if (roomvisits.Count > 0)
            {
                IEnumerable<IGrouping<uint, Roomvisit>> roomvisitsG = roomvisits.GroupBy(r => r.RoomID).Take(5);
                message.AppendInt32(roomvisitsG.Count());
                foreach (IGrouping<uint, Roomvisit> roomvisitPre in roomvisitsG)
                {
                    foreach(Roomvisit roomvisit in roomvisitPre)
                    {
                        RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomvisit.RoomID);
                        message.AppendBoolean(roomData != null ? roomData.IsPublicRoom : false);
                        message.AppendUInt(roomvisit.RoomID);
                        message.AppendString(roomData != null ? roomData.Name : "![UNABLE TO FIND ROOM]!");

                        List<RoomMessage> chatlog = Skylight.GetGame().GetChatlogManager().GetRoomChatlog(roomvisit.RoomID);
                        message.AppendInt32(chatlog.Count);
                        foreach(RoomMessage msg in chatlog)
                        {
                            DateTime time = msg.GetDate();
                            message.AppendInt32(time.Hour);
                            message.AppendInt32(time.Minute);
                            message.AppendUInt(msg.UserID);
                            message.AppendString(msg.Username);
                            if (!msg.Message.StartsWith("Whisper" + (char)9))
                            {
                                message.AppendString(msg.Message);
                            }
                            else
                            {
                                string[] data = msg.Message.Split((char)9);
                                message.AppendString("<Whisper to " + Skylight.GetGame().GetGameClientManager().GetUsernameByID(uint.Parse(data[1])) + ">: " + data[2]);
                            }
                        }
                        break;
                    }
                }
            }
            else
            {
                message.AppendInt32(0);
            }
            return message;
        }

        public Dictionary<string, List<ModerationIssue>> GetIssues()
        {
            return this.Issues;
        }

        public List<string> GetUserPresents()
        {
            return this.UserPresents;
        }

        public List<string> GetRoomPresents()
        {
            return this.RoomPresents;
        }

        public Dictionary<uint, SupportTicket> GetSupportTickets()
        {
            return this.SupportTickets;
        }

        public void Shutdown()
        {
            if (this.Issues != null)
            {
                this.Issues.Clear();
            }
            this.Issues = null;

            if (this.UserPresents != null)
            {
                this.UserPresents.Clear();
            }
            this.UserPresents = null;

            if (this.RoomPresents != null)
            {
                this.RoomPresents.Clear();
            }
            this.RoomPresents = null;

            if (this.SupportTickets != null)
            {
                this.SupportTickets.Clear();
            }
            this.SupportTickets = null;
        }
    }
}
