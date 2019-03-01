using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Misc;
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
    class SendUserTagsComposer<T> : OutgoingHandlerPacket where T : SendUserTagsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendUserTags);
            message.AppendUInt(handler.UserID);

            message.AppendInt32(handler.Tags.Count);
            foreach(string tag in handler.Tags)
            {
                message.AppendString(tag);
            }
            return message;
        }
    }
}
