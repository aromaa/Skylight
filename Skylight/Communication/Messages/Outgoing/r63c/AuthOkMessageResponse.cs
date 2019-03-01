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
    class AuthOkMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            ServerMessage AuthOK = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            AuthOK.Init(r63cOutgoing.AuthenicationOK);
            return AuthOK;
        }
    }
}
