using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;

namespace SkylightEmulator.Communication.Messages.Outgoing.r26
{
    class NewFriendRequestMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            MessengerRequest request = valueHolder.GetValue<MessengerRequest>("Request");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Message.Init(r26Outgoing.NewFriendRequest);
            Message.AppendBoolean(true);
            Message.AppendString(request.FromUsername);
            Message.AppendString(request.FromID.ToString());
            return Message;
        }
    }
}
