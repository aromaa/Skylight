using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class MessengerFriendRequestsComposer<T> : OutgoingHandlerPacket where T : MessengerFriendRequestsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerFriendRequests);
            message.AppendInt32(0); //unused

            message.AppendInt32(handler.Requests.Count);
            foreach(MessengerRequest request in handler.Requests)
            {
                message.AppendUInt(request.FromID);
                message.AppendString(request.FromUsername);
                message.AppendString(request.FromLook);
            }
            return message;
        }
    }
}
