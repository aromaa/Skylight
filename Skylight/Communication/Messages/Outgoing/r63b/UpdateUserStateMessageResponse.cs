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

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class UpdateUserStateMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<RoomUnit> users = valueHolder.GetValue<List<RoomUnit>>("RoomUser");
            bool everyone = valueHolder.GetValueOrDefault<bool>("Everyone");

            ServerMessage statues = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            statues.Init(r63bOutgoing.UserStatues);
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
