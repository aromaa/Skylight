using Newtonsoft.Json;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class ChatlogManager
    {
        private ConcurrentQueue<RoomMessage> RoomMessagesInsertNeeded;
        private ConcurrentQueue<PrivateMessage> PrivateMessagesInsertNeeded;
        private ConcurrentDictionary<uint, List<RoomMessage>> RoomChatlog;
        private ConcurrentDictionary<uint, ManualResetEvent> RoomChatlogReading;

        public ChatlogManager()
        {
            this.RoomMessagesInsertNeeded = new ConcurrentQueue<RoomMessage>();
            this.PrivateMessagesInsertNeeded = new ConcurrentQueue<PrivateMessage>();
            this.RoomChatlog = new ConcurrentDictionary<uint, List<RoomMessage>>();
            this.RoomChatlogReading = new ConcurrentDictionary<uint, ManualResetEvent>();
        }

        public void LogRoomMessage(GameClient session, string message)
        {
            RoomMessage message_ = new RoomMessage(session.GetHabbo().ID, session.GetHabbo().Username, session.GetHabbo().GetRoomSession().CurrentRoomID, TimeUtilies.GetUnixTimestamp(), message, session.SessionID);
            this.RoomMessagesInsertNeeded.Enqueue(message_);

            List<RoomMessage> roomChatlog = null;
            if (!this.RoomChatlog.TryGetValue(session.GetHabbo().GetRoomSession().CurrentRoomID, out roomChatlog))
            {
                roomChatlog = this.GetRoomChatlog(session.GetHabbo().GetRoomSession().CurrentRoomID);
            }

            while (roomChatlog.Count >= 300)
            {
                roomChatlog.RemoveAt(roomChatlog.Count - 1); //remove the last
            }
            roomChatlog.Insert(0, message_);

            this.RoomChatlog.AddOrUpdate(session.GetHabbo().GetRoomSession().CurrentRoomID, roomChatlog, (key, oldValue) => roomChatlog);
        }

        public void LogPrivateMessage(GameClient sender, GameClient receiver, string message)
        {
            PrivateMessage message_ = new PrivateMessage(sender.GetHabbo().ID, sender.GetHabbo().Username, receiver.GetHabbo().ID, receiver.GetHabbo().Username, TimeUtilies.GetUnixTimestamp(), message, sender.SessionID, receiver.SessionID);
            this.PrivateMessagesInsertNeeded.Enqueue(message_);
        }

        public void LogStaffChatMessage(GameClient sender, List<uint> receiverIds, List<string> receiverUsernames, List<int> receiverSessionIds, string message)
        {
            PrivateMessage message_ = new PrivateMessage(sender.GetHabbo().ID, sender.GetHabbo().Username, 0, "Staff Chat", TimeUtilies.GetUnixTimestamp(), message, sender.SessionID, -1, JsonConvert.SerializeObject(new PrivateMessageExtraData() { ReceiverIds = receiverIds, ReceiverUsernames = receiverUsernames, ReceiverSessionIds = receiverSessionIds }));
            this.PrivateMessagesInsertNeeded.Enqueue(message_);
        }

        public void LogRoomInvite(GameClient sender, List<uint> receiverIds, List<string> receiverUsernames, List<int> receiverSessionIds, string message)
        {
            PrivateMessage message_ = new PrivateMessage(sender.GetHabbo().ID, sender.GetHabbo().Username, 0, "Room Invite", TimeUtilies.GetUnixTimestamp(), message, sender.SessionID, -1, JsonConvert.SerializeObject(new PrivateMessageExtraData() { ReceiverIds = receiverIds, ReceiverUsernames = receiverUsernames, ReceiverSessionIds = receiverSessionIds }));
            this.PrivateMessagesInsertNeeded.Enqueue(message_);
        }

        public List<RoomMessage> GetRoomChatlog(uint roomId)
        {
            List<RoomMessage> roomChatlog = null;
            if (!this.RoomChatlog.TryGetValue(roomId, out roomChatlog))
            {
                ManualResetEvent reading = null;
                if (this.RoomChatlogReading.TryAdd(roomId, new ManualResetEvent(false))) //try add the ManualResentEvent, if failed, another thread is reading data already
                {
                    //not even loaded
                    roomChatlog = new List<RoomMessage>();

                    DataTable chatlog = null;
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("roomId", roomId);
                        chatlog = dbClient.ReadDataTable("SELECT user_id, user_name, room_id, timestamp, message, user_session_id FROM chatlogs_rooms WHERE room_id = @roomId ORDER BY timestamp DESC LIMIT 300");
                    }

                    if (chatlog != null && chatlog.Rows.Count > 0)
                    {
                        foreach (DataRow dataRow in chatlog.Rows)
                        {
                            roomChatlog.Add(new RoomMessage((uint)dataRow["user_id"], (string)dataRow["user_name"], (uint)dataRow["room_id"], (double)dataRow["timestamp"], (string)dataRow["message"], (int)dataRow["user_session_id"]));
                        }
                    }

                    if (!this.RoomChatlog.TryAdd(roomId, roomChatlog)) //hmh, failed to add, probs called two times and the other method finished earlier(?)
                    {
                        if (!this.RoomChatlog.TryGetValue(roomId, out roomChatlog))
                        {
                            //what just happend?
                        }
                    }

                    if (this.RoomChatlogReading.TryRemove(roomId, out reading))
                    {
                        reading.Set(); //we are done!
                    }
                    else
                    {
                        //??? weird
                    }
                }
                else //another thread is reading
                {
                    if (this.RoomChatlogReading.TryGetValue(roomId, out reading))
                    {
                        if (!reading.WaitOne(1000)) //wait the reading is done
                        {
                            Console.WriteLine("ChatlogManager just timeout!");
                        }
                    }

                    if (!this.RoomChatlog.TryGetValue(roomId, out roomChatlog))
                    {
                        //what just happend?
                    }
                }
            }

            return roomChatlog;
        }

        public void PushRoomChatlogToDB()
        {
            if (this.RoomMessagesInsertNeeded.Count > 0)
            {
                StringBuilder query = new StringBuilder();

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    int i = 0;
                    RoomMessage message;
                    while (this.RoomMessagesInsertNeeded.TryDequeue(out message))
                    {
                        i++; //needed so params works correctly

                        dbClient.AddParamWithValue("userId" + i, message.UserID);
                        dbClient.AddParamWithValue("userName" + i, message.Username);
                        dbClient.AddParamWithValue("roomId" + i, message.RoomID);
                        dbClient.AddParamWithValue("timestamp" + i, message.Timestamp);
                        dbClient.AddParamWithValue("message" + i, message.Message);
                        dbClient.AddParamWithValue("userSessionId" + i, message.UserSessionID);

                        query.Append("INSERT INTO chatlogs_rooms(user_id, user_name, room_id, timestamp, message, user_session_id) VALUES(@userId" + i + ", @userName" + i + ", @roomId" + i + ", @timestamp" + i + ", @message" + i + ", @userSessionId" + i + "); ");
                    }

                    if (query.Length > 0)
                    {
                        dbClient.ExecuteQuery(query.ToString());
                    }
                }
            }
        }

        public void PushPrivateChatlogToDB()
        {
            if (this.PrivateMessagesInsertNeeded.Count > 0)
            {
                StringBuilder query = new StringBuilder();

                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    int i = 0;
                    PrivateMessage message;
                    while (this.PrivateMessagesInsertNeeded.TryDequeue(out message))
                    {
                        i++; //needed so params works correctly

                        dbClient.AddParamWithValue("senderId" + i, message.SenderID);
                        dbClient.AddParamWithValue("senderUsername" + i, message.SenderUsername);
                        dbClient.AddParamWithValue("receiverId" + i, message.ReceiverID);
                        dbClient.AddParamWithValue("receiverUsername" + i, message.ReceiverUsername);
                        dbClient.AddParamWithValue("timestamp" + i, message.Timestamp);
                        dbClient.AddParamWithValue("message" + i, message.Message);
                        dbClient.AddParamWithValue("senderSessionId" + i, message.SenderSessionID);
                        dbClient.AddParamWithValue("receiverSessionId" + i, message.ReceiverSessionID);
                        dbClient.AddParamWithValue("extraData" + i, message.ExtraData);

                        query.Append("INSERT INTO chatlogs_private(sender_id, sender_name, receiver_id, receiver_name, timestamp, message, sender_session_id, receiver_session_id, extra_data) VALUES(@senderId" + i + ", @senderUsername" + i + ", @receiverId" + i + ", @receiverUsername" + i + ", @timestamp" + i + ", @message" + i + ", @senderSessionId" + i + ", @receiverSessionId" + i + ", @extraData" + i + "); ");
                    }

                    if (query.Length > 0)
                    {
                        dbClient.ExecuteQuery(query.ToString());
                    }
                }
            }
        }

        public void Shutdown()
        {
            if (this.RoomChatlog != null)
            {
                this.RoomChatlog.Clear();
            }
            this.RoomChatlog = null;

            if (this.RoomMessagesInsertNeeded != null)
            {
                this.PushRoomChatlogToDB();
            }
            this.RoomMessagesInsertNeeded = null;

            if (this.PrivateMessagesInsertNeeded != null)
            {
                this.PushPrivateChatlogToDB();
            }
            this.PrivateMessagesInsertNeeded = null;

            if (this.RoomChatlogReading != null)
            {
                this.RoomChatlogReading.Clear();
            }
            this.RoomChatlogReading = null;
        }
    }
}
