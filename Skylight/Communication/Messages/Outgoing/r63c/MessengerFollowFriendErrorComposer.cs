using SkylightEmulator.Communication.Messages.Outgoing.r63c;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class MessengerFollowFriendErrorComposer<T> : OutgoingHandlerPacket where T : MessengerFollowFriendErrorComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerFollowError);
            message.AppendInt32((int)handler.ErrorCode);
            return message;
        }
    }
}
