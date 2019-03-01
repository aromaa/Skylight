using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;
using SkylightEmulator.Storage;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerRemoveFriendsEventHandler : IncomingPacket
    {
        protected uint[] RemovedFriends;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                foreach(uint userId in this.RemovedFriends)
                {
                    session.GetHabbo().GetMessenger().RemoveFriendFromBoth(userId);
                }

                string friendIds = string.Join(",", this.RemovedFriends);
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    dbClient.AddParamWithValue("userid", session.GetHabbo().ID);
                    dbClient.ExecuteQuery("DELETE FROM messenger_friends WHERE (user_one_id = @userid AND user_two_id IN(" + friendIds + ")) OR (user_one_id IN(" + friendIds + ") AND user_two_id = @userid) LIMIT " + this.RemovedFriends.Length);
                }
            }
        }
    }
}
