using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class SetRoomUserMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomUnit> users = valueHolder.GetValue<List<RoomUnit>>("RoomUser");

            ServerMessage NewRoomUser = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            NewRoomUser.Init(r63aOutgoing.SetRoomUser);
            NewRoomUser.AppendInt32(users.Count); //count
            foreach (RoomUnit user in users)
            {
                user.Serialize(NewRoomUser);
            }
            return NewRoomUser;
        }
    }
}
