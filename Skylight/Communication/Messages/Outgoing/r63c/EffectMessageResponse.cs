using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class EffectMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.Effect);
            message.AppendInt32(valueHolder.GetValue<int>("VirtualID"));
            message.AppendInt32(valueHolder.GetValue<int>("EffectID"));
            message.AppendInt32(0); //idk
            return message;
        }
    }
}
