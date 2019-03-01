using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Messenger
{
    public class HabboMessenger
    {
        private readonly uint ID;
        private Habbo Habbo;

        private Dictionary<uint, MessengerFriend> Friends;
        private Dictionary<uint, MessengerRequest> Requests;

        public HabboMessenger(uint id, Habbo habbo)
        {
            this.Friends = new Dictionary<uint, MessengerFriend>();
            this.Requests = new Dictionary<uint, MessengerRequest>();

            this.ID = id;
            this.Habbo = habbo;
        }

        public void LoadFriends()
        {
            this.Friends.Clear();
            DataTable friends = this.Habbo.GetUserDataFactory().GetMessengerFriends();
            if (friends != null)
            {
                foreach (DataRow friend in friends.Rows)
                {
                    uint id = (uint)friend["friend_id"];
                    this.Friends.Add(id, new MessengerFriend(id, (string)friend["look"], (string)friend["motto"], (double)friend["last_online"]));
                }
            }
        }

        public void LoadRequests()
        {
            this.Requests.Clear();
            DataTable requests = this.Habbo.GetUserDataFactory().GetMessengerRequests();
            if (requests != null)
            {
                foreach (DataRow request in requests.Rows)
                {
                    uint id = (uint)request["id"];
                    this.Requests.Add(id, new MessengerRequest(id, this.ID, (uint)request["from_id"]));
                }
            }
        }

        public void SendFriends()
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.MessengerFriends);
            Message.AppendInt32(6000);
            Message.AppendInt32(200);
            Message.AppendInt32(6000);
            Message.AppendInt32(900);
            Message.AppendInt32(0); //category count

            Message.AppendInt32(this.Friends.Count);
            foreach (MessengerFriend friend in this.Friends.Values.ToList())
            {
                friend.Serialize(Message, true);
            }
            this.Habbo.GetSession().SendMessage(Message);
        }

        public void SendRequests()
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.FriendRequests);
            Message.AppendInt32(this.Requests.Count);
            Message.AppendInt32(this.Requests.Count);
            foreach (MessengerRequest request in this.Requests.Values.ToList())
            {
                request.Serialize(Message);
            }
            this.Habbo.GetSession().SendMessage(Message);
        }

        public void SearchHabbo(string username)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.MessengerSearchResult);

            DataTable matchUsers = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("query", username + "%");
                matchUsers = dbClient.ReadDataTable("SELECT id, look, motto, last_online FROM users WHERE username LIKE @query LIMIT 50");
            }

            if (matchUsers != null)
            {
                List<DataRow> friends = new List<DataRow>();
                List<DataRow> randomPeople = new List<DataRow>();
                foreach (DataRow dataRow in matchUsers.Rows)
                {
                    if (this.Friends.ContainsKey((uint)dataRow["Id"]))
                    {
                        friends.Add(dataRow);
                    }
                    else
                    {
                        randomPeople.Add(dataRow);
                    }
                }

                Message.AppendInt32(friends.Count);
                foreach (DataRow dataRow in friends)
                {
                    this.Friends[(uint)dataRow["Id"]].Serialize(Message, false);
                }

                Message.AppendInt32(randomPeople.Count);
                foreach (DataRow dataRow in randomPeople)
                {
                    new MessengerFriend((uint)dataRow["Id"], (string)dataRow["look"], (string)dataRow["motto"], (double)dataRow["last_online"]).Serialize(Message, false);
                }
            }
            else
            {
                Message.AppendInt32(0);
                Message.AppendInt32(0);
            }

            this.Habbo.GetSession().SendMessage(Message);
        }

        public bool RequestSended(uint id)
        {
            if (this.Requests.ContainsKey(id))
            {
                return true;
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("toid", id);
                    dbClient.AddParamWithValue("fromid", this.ID);
                    return dbClient.ReadDataRow("SELECT null FROM messenger_requests WHERE to_id = @toid AND from_id = @fromid") != null;
                }
            }
        }

        public void RequestFriend(string username)
        {
            DataRow user = null;
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("username", username);
                user = dbClient.ReadDataRow("SELECT id, block_newfriends FROM users WHERE username = @username LIMIT 1;");
            }

            if (user != null)
            {
                if (TextUtilies.StringToBool((string)user["block_newfriends"]))
                {
                    ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                    Message.Init(r63aOutgoing.RequestFriendError);
                    Message.AppendInt32(39);
                    Message.AppendInt32(3);
                    this.Habbo.GetSession().SendMessage(Message);
                }
                else
                {
                    uint id = (uint)user["id"];
                    if (!this.RequestSended(id))
                    {
                        MessengerRequest request = null;
                        using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                        {
                            dbClient.AddParamWithValue("toid", id);
                            dbClient.AddParamWithValue("userid", this.ID);
                            dbClient.ExecuteQuery("INSERT INTO messenger_requests (to_id, from_id) VALUES (@toid, @userid)");

                            request = new MessengerRequest((uint)dbClient.GetID(), id, this.ID);
                        }

                        if (request != null)
                        {
                            this.AddFriendRequest(request);
                            GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(id);
                            if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null)
                            {
                                gameClient.GetHabbo().GetMessenger().AddFriendRequest(request);
                            }
                        }
                    }
                }
            }
        }

        public void AddFriendRequest(MessengerRequest request)
        {
            this.Requests.Add(request.ID, request);

            if (request.ToID == this.ID)
            {
                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.NewFriendRequest);
                request.Serialize(Message);
                this.Habbo.GetSession().SendMessage(Message);
            }
        }

        public MessengerRequest GetFriendRequest(uint id)
        {
            if (this.Requests.ContainsKey(id))
            {
                return this.Requests[id];
            }
            else
            {
                return null;
            }
        }

        public bool IsFriend(uint id)
        {
            if (this.Friends.ContainsKey(id))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void AcceptFriend(uint id)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("fromid", id);
                dbClient.AddParamWithValue("toid", this.ID);
                dbClient.ExecuteQuery("INSERT INTO messenger_friends (user_one_id,user_two_id) VALUES (@fromid,@toid)");
            }
            this.AddFriend(id);
            GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(id);
            if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null)
            {
                gameClient.GetHabbo().GetMessenger().AddFriend(this.ID);
            }
        }

        public void AddFriend(uint id)
        {
            GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(id);
            if (gameClient != null && gameClient.GetHabbo() != null)
            {
                MessengerFriend friend = new MessengerFriend(id, gameClient.GetHabbo().Look, gameClient.GetHabbo().Motto, gameClient.GetHabbo().LastOnline);
                friend.NeedUpdate = true;
                this.Friends.Add(id, friend);
            }
            else
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", id);
                    DataRow user = dbClient.ReadDataRow("SELECT motto,look,last_online FROM users WHERE id = @userid");
                    if (user != null)
                    {
                        MessengerFriend friend = new MessengerFriend(id, (string)user["look"], (string)user["motto"], (double)user["last_online"]);
                        friend.NeedUpdate = true;
                        this.Friends.Add(id, friend);
                    }
                }
            }
            this.SendUpdates();
        }

        public void SendUpdates()
        {
            List<MessengerFriend> needUpdate = new List<MessengerFriend>();
            foreach (MessengerFriend friend in this.Friends.Values)
            {
                if (friend.NeedUpdate)
                {
                    friend.NeedUpdate = false;
                    needUpdate.Add(friend);
                }
            }

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.MeesengerUpdate);
            Message.AppendInt32(0);
            Message.AppendInt32(needUpdate.Count);
            Message.AppendInt32(0);
            foreach (MessengerFriend friend in this.Friends.Values)
            {
                friend.Serialize(Message, true);
            }
            this.Habbo.GetSession().SendMessage(Message);
        }

        public void RemoveFriendRequest(uint fromId)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("fromid", fromId);
                dbClient.AddParamWithValue("toid", this.ID);
                dbClient.ExecuteQuery("DELETE FROM messenger_requests WHERE to_id = @toid AND from_id = @fromid LIMIT 1");
            }

            if (this.Requests.ContainsKey(fromId))
            {
                this.Requests.Remove(fromId);
            }
        }

        public void SendChatMessage(uint userId, string message)
        {
            if (!this.Friends.ContainsKey(userId))
            {
                this.ChatError(6, userId);
            }
            else
            {
                GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null)
                {
                    if (this.Habbo.IsMuted())
                    {
                        this.ChatError(4, userId);
                    }
                    else
                    {
                        if (gameClient.GetHabbo().IsMuted())
                        {
                            this.ChatError(3, userId);
                        }

                        //handle spam
                        string filteredMessage = TextUtilies.CheckBlacklistedWords(message);
                        //chatlog

                        ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                        Message.Init(r63aOutgoing.MessengerChatMessage);
                        Message.AppendUInt(this.ID);
                        Message.AppendString(filteredMessage);
                        gameClient.SendMessage(Message);
                    }
                }
                else
                {
                    this.ChatError(5, userId);
                }
            }
        }

        public void ChatError(int errorId, uint userId)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
            Message.Init(r63aOutgoing.MessengerChatError);
            Message.AppendInt32(errorId);
            Message.AppendUInt(userId);
            this.Habbo.GetSession().SendMessage(Message);
        }

        public void UpdateAllFriends(bool updateInstantly)
        {
            foreach (MessengerFriend friend in this.Friends.Values.ToList())
            {
                if (friend != null)
                {
                    try
                    {
                        GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(friend.ID);
                        if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null)
                        {
                            gameClient.GetHabbo().GetMessenger().UserLogIn(this.ID);
                            if (updateInstantly)
                            {
                                gameClient.GetHabbo().GetMessenger().SendUpdates();
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }

        public void UserLogIn(uint userId)
        {
            if (this.Friends.ContainsKey(userId))
            {
                this.Friends[userId].NeedUpdate = true;
            }
        }

        public void DeleteFriend(uint userId)
        {
            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("toid", userId);
                dbClient.AddParamWithValue("fromid", this.ID);
                dbClient.ExecuteQuery("DELETE FROM messenger_friends WHERE (user_one_id = @toid AND user_two_id = @fromid) OR (user_one_id = @fromid AND user_two_id = @toid) LIMIT 1");
            }
            this.RemoveFriend(userId);
            GameClient gameClient = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
            if (gameClient != null && gameClient.GetHabbo() != null && gameClient.GetHabbo().GetMessenger() != null)
            {
                gameClient.GetHabbo().GetMessenger().RemoveFriend(this.ID);
            }
        }

        public void RemoveFriend(uint userId)
        {
            if (this.Friends.ContainsKey(userId))
            {
                this.Friends.Remove(userId);

                ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Skylight.Revision);
                Message.Init(r63aOutgoing.MeesengerUpdate);
                Message.AppendInt32(0);
                Message.AppendInt32(1);
                Message.AppendInt32(-1);
                Message.AppendUInt(userId);
                this.Habbo.GetSession().SendMessage(Message);
            }
        }

        public void DeclineFriendRequest(uint userId)
        {
            this.RemoveFriendRequest(userId);
        }
    }
}
