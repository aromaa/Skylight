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
    class MOTDMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<string> messages = valueHolder.GetValue<List<string>>("Message");

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            message_.Init(r63cOutgoing.MOTD);
            message_.AppendInt32(messages.Count);
            foreach (string message in messages)
            {
                message_.AppendString(message);
            }
            return message_;
        }
    }
}
