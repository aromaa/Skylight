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

namespace SkylightEmulator.Communication.Messages.Incoming.r63c
{
    class RequestRoomDataMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(message.PopWiredUInt());
            if (roomData != null)
            {
                int unknown = message.PopWiredInt32();
                int unknown2 = message.PopWiredInt32();

                ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
                message_.Init(r63cOutgoing.RoomData);
                roomData.SerializeRoomEntry(message_, unknown == 1 && unknown2 == 0, true);
                session.SendMessage(message_);
            }
        }
    }
}
