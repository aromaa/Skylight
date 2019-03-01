using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class UserPerksMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            message.Init(r63ccOutgoing.UserPerks);
            message.AppendInt32(2);

            message.AppendString("NAVIGATOR_PHASE_ONE_2014");
            message.AppendString("");
            message.AppendBoolean(true);

            message.AppendString("NAVIGATOR_PHASE_TWO_2014");
            message.AppendString("");
            message.AppendBoolean(true);
            return message;
        }
    }
}
