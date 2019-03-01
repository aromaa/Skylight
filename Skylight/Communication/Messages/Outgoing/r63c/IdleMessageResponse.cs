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
    class IdleMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage sleeps = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            sleeps.Init(r63cOutgoing.Sleeps);
            sleeps.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            sleeps.AppendBoolean(valueHolder.GetValue<bool>("Sleep"));
            return sleeps;
        }
    }
}
