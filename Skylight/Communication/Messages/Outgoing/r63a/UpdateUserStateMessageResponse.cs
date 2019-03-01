using SkylightEmulator.Communication.Headers;
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
    class UpdateUserStateMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomUnit> users = valueHolder.GetValue<List<RoomUnit>>("RoomUser");
            bool everyone = valueHolder.GetValueOrDefault<bool>("Everyone");

            ServerMessage statues = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            statues.Init(r63aOutgoing.UserStatues);
            statues.AppendInt32(users.Count);
            foreach (RoomUnit user in users)
            {
                if (!everyone)
                {
                    user.NeedUpdate = false;
                }

                user.SerializeStatus(statues);
            }
            return statues;
        }
    }
}
