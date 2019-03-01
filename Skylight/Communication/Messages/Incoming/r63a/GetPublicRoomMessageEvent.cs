using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a
{
    class GetPublicRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            RoomData data = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(message.PopWiredUInt());

            if (data != null)
            {
                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message_.Init(r63aOutgoing.PublicRoom);
                message_.AppendUInt(data.ID);
                message_.AppendString(data.PublicCCTs);
                message_.AppendUInt(data.ID);
                session.SendMessage(message_);
            }
        }
    }
}
