using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
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
    class GuideStartSessionComposer<T> : OutgoingHandlerPacket where T : GuideStartSessionComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.GuideStartSession);
            message.AppendUInt(handler.RequesterID);
            message.AppendString(handler.RequesterUsername);
            message.AppendString(handler.RequesterLook);

            message.AppendUInt(handler.HelperID);
            message.AppendString(handler.HelperUsername);
            message.AppendString(handler.HelperLook);
            return message;
        }
    }
}
