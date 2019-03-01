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
    class AuthOkMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage AuthOK = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            AuthOK.Init(r63bOutgoing.AuthenicationOK);
            return AuthOK;
        }
    }
}
