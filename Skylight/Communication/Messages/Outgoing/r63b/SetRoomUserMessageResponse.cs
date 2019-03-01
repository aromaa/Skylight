using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class SetRoomUserMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomUnit> users = valueHolder.GetValue<List<RoomUnit>>("RoomUser");

            ServerMessage NewRoomUser = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            NewRoomUser.Init(r63bOutgoing.SetRoomUser);
            NewRoomUser.AppendInt32(users.Count); //count
            foreach (RoomUnit user in users)
            {
                user.Serialize(NewRoomUser);
            }
            return NewRoomUser;
        }
    }
}
