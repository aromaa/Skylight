using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class ShowNotificationsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage Logging = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Logging.Init(r63bOutgoing.Logging);
            Logging.AppendBoolean(true); //show notifications?
            return Logging;
        }
    }
}
