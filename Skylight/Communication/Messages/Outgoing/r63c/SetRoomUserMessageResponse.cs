using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Rooms;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class SetRoomUserMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomUnit> users = valueHolder.GetValue<List<RoomUnit>>("RoomUser");

            ServerMessage NewRoomUser = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            NewRoomUser.Init(r63cOutgoing.SetRoomUser);
            NewRoomUser.AppendInt32(users.Count); //count
            foreach (RoomUnit user in users)
            {
                user.Serialize(NewRoomUser);
            }
            return NewRoomUser;
        }
    }
}
