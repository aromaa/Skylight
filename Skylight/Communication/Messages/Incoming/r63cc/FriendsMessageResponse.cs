using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Communication.Headers;

namespace SkylightEmulator.Communication.Messages.Incoming.r63cc
{
    class FriendsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201611291003_338511768);
            Message.Init(r63ccOutgoing.MessengerFriends);
            Message.AppendInt32(300);
            Message.AppendInt32(300);

            Message.AppendInt32(friends.Count);
            foreach (MessengerFriend friend in friends)
            {
                friend.Serialize(Message, true);
            }
            return Message;
        }
    }
}
