using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class GetUserTagsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint userId = message.PopWiredUInt();

            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                RoomUnitUser user = room.RoomUserManager.GetUserByID(userId);
                if (user != null)
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.UserTags);
                    message_.AppendUInt(userId);
                    message_.AppendInt32(user.Session.GetHabbo().Tags.Count);
                    foreach(string tag in user.Session.GetHabbo().Tags)
                    {
                        message_.AppendString(tag);
                    }
                    session.SendMessage(message_);
                }
            }
        }
    }
}
