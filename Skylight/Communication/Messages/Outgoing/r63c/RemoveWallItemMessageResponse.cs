using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RemoveWallItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage WallItemRemoved = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            WallItemRemoved.Init(r63cOutgoing.RemoveWallItem);
            WallItemRemoved.AppendString(valueHolder.GetValue<uint>("ID").ToString());
            WallItemRemoved.AppendUInt(valueHolder.GetValue<uint>("UserID"));
            return WallItemRemoved;
        }
    }
}
