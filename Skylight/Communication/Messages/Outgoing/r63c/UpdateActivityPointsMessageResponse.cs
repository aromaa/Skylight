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
    class UpdateActivityPointsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.UpdateActivityPoints);
            Message.AppendInt32(valueHolder.GetValue<int>("Points"));
            Message.AppendInt32(0); //change, unused
            Message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return Message;
        }
    }
}
