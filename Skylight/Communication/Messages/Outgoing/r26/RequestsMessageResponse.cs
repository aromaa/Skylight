using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class RequestsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerRequest> requests = valueHolder.GetValue<List<MessengerRequest>>("Requests");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Message.Init(r26Outgoing.FriendRequests);
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
