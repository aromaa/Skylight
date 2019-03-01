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
    class UnseenItemMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message.Init(r63cOutgoing.NewItemAdded);
            message.AppendInt32(1);
            message.AppendInt32(valueHolder.GetValue<int>("Type"));
            message.AppendInt32(1);
            message.AppendInt32(valueHolder.GetValue<int>("ID"));
            return message;
        }
    }
}
