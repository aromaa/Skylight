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
    class GetPostItDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            Room room = session.GetHabbo().GetRoomSession().GetRoom();
            if (room != null)
            {
                uint itemId = message.PopWiredUInt();
                RoomItem item = room.RoomItemManager.TryGetRoomItem(itemId);
                if (item != null && item.GetBaseItem().InteractionType.ToLower() == "postit")
                {
                    ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                    message_.Init(r63aOutgoing.PostItData);
                    message_.AppendString(itemId.ToString());
                    message_.AppendString(item.ExtraData);
                    session.SendMessage(message_);
                }
            }
        }
    }
}
