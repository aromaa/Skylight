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
    class IdleMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage sleeps = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            sleeps.Init(r63aOutgoing.Sleeps);
            sleeps.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            sleeps.AppendBoolean(valueHolder.GetValue<bool>("Sleep"));
            return sleeps;
        }
    }
}
