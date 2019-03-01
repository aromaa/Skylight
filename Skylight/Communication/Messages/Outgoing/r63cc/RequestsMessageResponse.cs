using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63cc
{
    class RequestsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerRequest> requests = valueHolder.GetValue<List<MessengerRequest>>("Requests");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.FriendRequests);
            Message.AppendInt32(requests.Count);
            Message.AppendInt32(requests.Count);
            foreach (MessengerRequest request in requests)
            {
                request.Serialize(Message);
            }
            return Message;
        }
    }
}
