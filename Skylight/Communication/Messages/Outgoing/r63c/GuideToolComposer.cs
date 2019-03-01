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
    class GuideToolComposer<T> : OutgoingHandlerPacket where T : GuideToolComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.GuideTool);
            message.AppendBoolean(handler.OnDuty);
            message.AppendInt32(handler.GuidesOnDuty);
            message.AppendInt32(handler.HelpersOnDuty);
            message.AppendInt32(handler.GuardiansOnDuty);
            return message;
        }
    }
}
