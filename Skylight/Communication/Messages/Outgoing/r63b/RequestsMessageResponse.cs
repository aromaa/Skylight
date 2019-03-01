using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63b
{
    class RequestsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerRequest> requests = valueHolder.GetValue<List<MessengerRequest>>("Requests");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Message.Init(r63bOutgoing.FriendRequests);
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
