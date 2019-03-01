using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.Handlers.Messenger
{
    public class MessengerAcceptFriendRequestEventHandler : IncomingPacket
    {
        protected uint[] FriendRequests;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo()?.GetMessenger() != null)
            {
                uint adderId = session.GetHabbo().ID;
                bool adderFriendStreamEnabled = session.GetHabbo().GetUserSettings().FriendStream;

                StringBuilder query = new StringBuilder();
                foreach(uint userId in this.FriendRequests)
                {
                    session.GetHabbo().GetMessenger().AddFriendToBoth(userId);

                    query.Append("DELETE FROM messenger_requests WHERE (to_id = '" + adderId + "' AND from_id = '" + userId + "') OR (to_id = '" + userId + "' AND from_id = '" + adderId + "') LIMIT 2; ");
                    query.Append("INSERT INTO messenger_friends (user_one_id,user_two_id) VALUES (" + adderId + "," + userId + "); ");
                    query.Append("INSERT INTO user_friend_stream(type, user_id, timestamp, extra_data) SELECT '0', '" + userId + "', UNIX_TIMESTAMP(), '" + adderId + "' FROM users WHERE id = '" + userId + "' AND friend_stream = '1'; ");

                    if (adderFriendStreamEnabled)
                    {
                        query.Append("INSERT INTO user_friend_stream(type, user_id, timestamp, extra_data) VALUES('0', " + adderId + ", UNIX_TIMESTAMP(), " + userId + "); ");
                    }
                }

                if (query.Length > 0)
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.ExecuteQuery(query.ToString());
                    }
                }
            }
        }
    }
}
