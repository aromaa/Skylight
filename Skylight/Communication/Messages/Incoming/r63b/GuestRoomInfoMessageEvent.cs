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

namespace SkylightEmulator.Communication.Messages.Incoming.r63b
{
    class GuestRoomInfoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            bool enterRoom = message.PopWiredBoolean();
            bool forward = message.PopWiredBoolean();

            RoomData roomData = Skylight.GetGame().GetRoomManager().TryGetAndLoadRoomData(roomId);
            if (roomData != null)
            {
                ServerMessage roomData_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
                roomData_.Init(r63aOutgoing.RoomData);
                roomData_.AppendBoolean(enterRoom); //entered room
                roomData.Serialize(roomData_, false);
                roomData_.AppendBoolean(forward); //forward
                roomData_.AppendBoolean(roomData.IsStaffPick); //is staff pick
                session.SendMessage(roomData_);
            }
        }
    }
}
