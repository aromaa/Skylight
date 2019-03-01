using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Messages.MultiRevision;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class SignMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int packetId = message.PopWiredInt32();

            session.GetHabbo().GetRoomSession().CurrentRoomRoomUser.AddStatus("sign", packetId.ToString(), 0.000000000000000000001);
            session.GetHabbo().GetRoomSession().GetRoom().SendToAll(new MultiRevisionServerMessage(OutgoingPacketsEnum.UpdateUserState, new ValueHolder().AddValue("Everyone", false).AddValue("RoomUser", new List<RoomUnit>() { session.GetHabbo().GetRoomSession().CurrentRoomRoomUser })));
        }
    }
}
