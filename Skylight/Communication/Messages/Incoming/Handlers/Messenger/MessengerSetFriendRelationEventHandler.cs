using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.HabboHotel.Data.Data;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerSetFriendRelationEventHandler : IncomingPacket
    {
        protected uint UserID;
        protected MessengerFriendRelation Relation;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                MessengerFriend friend = session.GetHabbo().GetMessenger().GetFriend(this.UserID);
                if (friend != null && friend.Relation != this.Relation)
                {
                    friend.Relation = this.Relation;

                    session.SendMessage(new MessengerUpdateFriendsComposerHandler(null, new List<MessengerUpdateFriend>() { new MessengerUpdateFriendUpdate(friend) }));

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.AddParamWithValue("friendId", friend.ID);
                        dbClient.AddParamWithValue("relation", (int)this.Relation);
                        dbClient.ExecuteQuery("UPDATE messenger_friends SET user_one_relation = IF(user_one_id = @userId, @relation, user_one_relation), user_two_relation = IF(user_two_id = @userId, @relation, user_two_relation) WHERE (user_one_id = @userId AND user_two_id = @friendId) OR (user_one_id = @friendId AND user_two_id = @userId) LIMIT 1");
                    }
                }
            }
        }
    }
}
