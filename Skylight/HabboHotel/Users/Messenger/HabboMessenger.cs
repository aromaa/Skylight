using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Data;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Data.Interfaces;
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
        private Dictionary<int, MessengerCategory> FriendCategorys;
        private Dictionary<uint, MessengerRequest> FriendRequestsPending;
        private HashSet<uint> FriendRequestsSend;

        public HabboMessenger(uint id, Habbo habbo)
        {
            this.Friends = new Dictionary<uint, MessengerFriend>();
            this.FriendCategorys = new Dictionary<int, MessengerCategory>();
            this.FriendRequestsPending = new Dictionary<uint, MessengerRequest>();
            this.FriendRequestsSend = new HashSet<uint>();

            this.ID = id;
            this.Habbo = habbo;
        }

        public void LoadCategorys()
        {
            this.FriendCategorys.Clear();

            DataTable categorys = this.Habbo.GetUserDataFactory().GetMessengerFriendCategorys();
            if (categorys?.Rows.Count > 0)
            {
                foreach(DataRow category in categorys.Rows)
                {
                    int id = (int)category["id"];
                    this.FriendCategorys.Add(id, new MessengerCategory(id, (string)category["name"]));
                }
            }
        }

        public void LoadFriends()
        {
            this.Friends.Clear();

            DataTable friends = this.Habbo.GetUserDataFactory().GetMessengerFriends();
            if (friends?.Rows.Count > 0)
            {
                foreach (DataRow friend in friends.Rows)
                {
                    uint id = (uint)friend["friend_id"];
                    this.Friends.Add(id, new MessengerFriend(id, (int)friend["category"], (string)friend["look"], (string)friend["motto"], (double)friend["last_online"], (MessengerFriendRelation)int.Parse((string)friend["relation"])));
                }
            }

            if (this.Habbo.HasPermission("acc_staffchat"))
            {
                this.Friends.Add(0, new MessengerFriend(0, 0, this.Habbo.Look, "Staff Chat", 0, MessengerFriendRelation.None));
            }
        }

        public void LoadFriendRequestsPending()
        {
            this.FriendRequestsPending.Clear();

            DataTable requests = this.Habbo.GetUserDataFactory().GetMessengerFriendRequestsPending();
            if (requests?.Rows.Count > 0)
            {
                foreach (DataRow request in requests.Rows)
                {
                    uint fromId = (uint)request["from_id"];
                    this.FriendRequestsPending.Add(fromId, new MessengerRequest(this.ID, fromId, (string)request["username"], (string)request["look"]));
                }
            }
        }

        public void LoadFriendRequestsSend()
        {
            this.FriendRequestsSend.Clear();

            DataTable send = this.Habbo.GetUserDataFactory().GetMessengerFriendRequestsSend();
            if (send?.Rows.Count > 0)
            {
                foreach(DataRow request in send.Rows)
                {
                    this.FriendRequestsSend.Add((uint)request["to_id"]);
                }
            }
        }
        
        /// <summary>
        /// "Tries" add the user, this is only HashSet.Add
        /// </summary>
        /// <param name="userId">The user id that should be tried to add to friends</param>
        /// <returns>If the user has no previous pending friend requests</returns>
        public bool TrySendFriendRequestTo(uint userId)
        {
            return this.FriendRequestsSend.Add(userId);
        }

        public bool HasSendedFriendRequestTo(uint userId)
        {
            return this.FriendRequestsSend.Contains(userId);
        }

        public bool HasFriendRequestPendingFrom(uint userId)
        {
            return this.FriendRequestsPending.ContainsKey(userId);
        }

        public bool IsFriendWith(uint id)
        {
            return this.Friends.ContainsKey(id);
        }

        public void AddFriendRequest(MessengerRequest request)
        {
            this.FriendRequestsPending.Add(request.FromID, request);

            this.Habbo.GetSession().SendMessage(new MessengerReceiveFriendRequestComposerHandler(request.FromID, request.FromUsername, request.FromLook));
        }

        /// <summary>
        /// Removes the friend from this user and from the partner
        /// </summary>
        /// <param name="userId">Friend user id</param>
        public void RemoveFriendFromBoth(uint userId)
        {
            this.RemoveFriend(userId);

            Skylight.GetGame().GetGameClientManager().GetGameClientById(userId)?.GetHabbo()?.GetMessenger()?.RemoveFriend(this.ID);
        }

        /// <summary>
        /// Removes friend only from this user, does not remove it from the partner
        /// </summary>
        /// <param name="userId">Friend user id</param>
        public void RemoveFriend(uint userId)
        {
            if (this.Friends.Remove(userId))
            {
                this.Habbo.GetSession().SendMessage(new MessengerUpdateFriendsComposerHandler(null, new List<MessengerUpdateFriend>() { new MessengerUpdateFriendRemove(userId) }));
                this.Habbo.GetUserAchievements().CheckAchievement("FriendListSize");
            }
        }

        public void AddFriendToBoth(uint userId)
        {
            this.AddFriend(userId);

            Skylight.GetGame().GetGameClientManager().GetGameClientById(userId)?.GetHabbo()?.GetMessenger()?.AddFriend(new MessengerFriend(this.Habbo.ID, 0, this.Habbo.Look, this.Habbo.Motto, this.Habbo.LastOnline, MessengerFriendRelation.None));
        }

        /// <summary>
        /// Only adds uses to this user, does not add it to the partner
        /// </summary>
        /// <param name="id">Friend user id</param>
        public void AddFriend(uint id)
        {
            bool found = false;
            string look = null;
            string motto = null;
            double lastOnline = 0;

            GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(id);
            if (target?.GetHabbo() != null)
            {
                found = true;
                look = target.GetHabbo().Look;
                motto = target.GetHabbo().Motto;
                lastOnline = target.GetHabbo().LastOnline;
            }
            else
            {
                DataRow user;
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", id);
                    user = dbClient.ReadDataRow("SELECT motto,look,last_online FROM users WHERE id = @userid");
                }

                if (user != null)
                {
                    found = true;
                    look = (string)user["look"];
                    motto = (string)user["motto"];
                    lastOnline = (double)user["last_online"];
                }
            }

            if (found)
            {
                this.AddFriend(new MessengerFriend(id, 0, look, motto, lastOnline, MessengerFriendRelation.None));
            }
        }

        /// <summary>
        /// Only adds uses to this user, does not add it to the partner
        /// </summary>
        /// <param name="friend">The friend to be added</param>
        public void AddFriend(MessengerFriend friend)
        {
            this.FriendRequestsSend.Remove(friend.ID);
            this.FriendRequestsPending.Remove(friend.ID);
            this.Friends.Add(friend.ID, friend);

            this.Habbo.GetSession().SendMessage(new MessengerUpdateFriendsComposerHandler(null, new List<MessengerUpdateFriend>() { new MessengerUpdateFriendAdd(friend) }));
            this.Habbo.GetUserAchievements().CheckAchievement("FriendListSize");
        }

        public void DeclineAllFriendRequests()
        {
            this.FriendRequestsPending.Clear();
        }

        public void DeclineFriendRequest(uint userId)
        {
            this.FriendRequestsPending.Remove(userId);
        }

        public MessengerFriend GetFriend(uint userId)
        {
            MessengerFriend friend;
            this.Friends.TryGetValue(userId, out friend);
            return friend;
        }
        
        public void SendUpdates()
        {
            List<MessengerUpdateFriend> needUpdate = new List<MessengerUpdateFriend>();
            foreach (MessengerFriend friend in this.Friends.Values)
            {
                if (friend.NeedUpdate)
                {
                    friend.NeedUpdate = false;

                    needUpdate.Add(new MessengerUpdateFriendUpdate(friend));
                }
            }

            if (needUpdate.Count > 0)
            {
                this.Habbo.GetSession().SendMessage(new MessengerUpdateFriendsComposerHandler(null, needUpdate));
            }
        }

        public void UserStatusUpdated(uint userId)
        {
            MessengerFriend friend;
            if (this.Friends.TryGetValue(userId, out friend))
            {
                friend.NeedUpdate = true;
            }
        }

        public void UpdateAllFriends(bool updateInstantly)
        {
            foreach (MessengerFriend friend in this.Friends.Values.ToList())
            {
                try
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(friend.ID);
                    if (target?.GetHabbo()?.GetMessenger() != null)
                    {
                        target.GetHabbo().GetMessenger().UserStatusUpdated(this.ID);
                        if (updateInstantly)
                        {
                            target.GetHabbo().GetMessenger().SendUpdates();
                        }
                    }
                }
                catch
                {

                }
            }
        }

        public ICollection<MessengerFriend> GetFriends()
        {
            return this.Friends.Values;
        }

        public ICollection<MessengerCategory> GetCategorys()
        {
            return this.FriendCategorys.Values;
        }

        public ICollection<MessengerRequest> GetRequests()
        {
            return this.FriendRequestsPending.Values;
        }
    }
}
