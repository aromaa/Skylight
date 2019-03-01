using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RoomDataMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            RoomData roomData = valueHolder.GetValue<RoomData>("Room");

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.RoomData);
            roomData.SerializeRoomEntry(message_, valueHolder.GetValueOrDefault<bool>("Entry"), valueHolder.GetValueOrDefault<bool>("Forward"));
            return message_;
        }
    }
}
