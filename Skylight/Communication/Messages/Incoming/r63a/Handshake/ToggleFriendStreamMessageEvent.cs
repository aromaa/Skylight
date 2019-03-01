using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ToggleFriendStreamMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().GetUserSettings().FriendStream = message.PopBase64Boolean();

            using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
            {
                dbClient.AddParamWithValue("userId", session.GetHabbo().ID);
                dbClient.AddParamWithValue("friendStream", TextUtilies.BoolToString(session.GetHabbo().GetUserSettings().FriendStream));

                dbClient.ExecuteQuery("UPDATE users SET friend_stream = @friendStream WHERE id = @userId LIMIT 1");
            }
        }
    }
}
