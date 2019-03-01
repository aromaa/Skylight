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
    public class MessengerDeclineFriendRequestEventHandler : IncomingPacket
    {
        protected bool All;
        protected uint[] DeclinedFriends;

        public virtual void Handle(GameClient session, ClientMessage message)
        {
            if (session?.GetHabbo().GetMessenger() != null)
            {
                if (!this.All)
                {
                    foreach(uint userId in this.DeclinedFriends)
                    {
                        session.GetHabbo().GetMessenger().DeclineFriendRequest(userId);
                    }

                    string requestIds = string.Join(",", this.DeclinedFriends);
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("DELETE FROM messenger_requests WHERE to_id = @userId AND from_id IN(" + requestIds + ") LIMIT " + this.DeclinedFriends.Length);
                    }
                }
                else
                {
                    session.GetHabbo().GetMessenger().DeclineAllFriendRequests();

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                        dbClient.ExecuteQuery("DELETE FROM messenger_requests WHERE to_id = @userId");
                    }
                }
            }
        }
    }
}
