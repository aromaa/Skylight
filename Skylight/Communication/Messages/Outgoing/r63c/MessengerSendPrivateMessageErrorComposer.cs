using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
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
    class MessengerSendPrivateMessageErrorComposer<T> : OutgoingHandlerPacket where T : MessengerSendPrivateMessageErrorComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerSendPrivateMessageError);
            message.AppendInt32((int)handler.ErrorCode);
            message.AppendUInt(handler.UserID);
            message.AppendString(""); //message
            return message;
        }
    }
}
