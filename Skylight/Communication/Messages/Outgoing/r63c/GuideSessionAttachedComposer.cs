using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Guide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Data.Enums;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class GuideSessionAttachedComposer<T> : OutgoingHandlerPacket where T : GuideSessionAttachedComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.GuideSessionAttached);
            message.AppendBoolean(handler.IsHelper);
            message.AppendInt32(GuideRequestTypeUtils.GuideRequestTypeToInt(handler.Type));
            message.AppendString(handler.Message);
            message.AppendInt32(handler.TimeLeft);
            return message;
        }
    }
}
