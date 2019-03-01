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
    class SendUserActiveBadgesComposer<T> : OutgoingHandlerPacket where T : SendUserActiveBadgesComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.SendUserActiveBadges);
            message.AppendUInt(handler.Profile.UserID);

            message.AppendInt32(handler.Profile.Badges.Count);
            for(int i = 0; i < handler.Profile.Badges.Count; i++)
            {
                message.AppendInt32(i);
                message.AppendString(handler.Profile.Badges[i]);
            }
            return message;
        }
    }
}
