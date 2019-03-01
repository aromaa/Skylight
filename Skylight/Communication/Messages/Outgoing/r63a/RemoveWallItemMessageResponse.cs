using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class RemoveWallItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage WallItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            WallItemRemoved.Init(r63aOutgoing.RemoveWallItemFromRoom);
            WallItemRemoved.AppendString(valueHolder.GetValue<uint>("ID").ToString());
            WallItemRemoved.AppendInt32(0);
            return WallItemRemoved;
        }
    }
}
