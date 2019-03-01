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
    class FriendsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_201211141113_913728051);
            Message.Init(r63bOutgoing.MessengerFriends);
            Message.AppendInt32(6000);
            Message.AppendInt32(200);
            Message.AppendInt32(6000);
            Message.AppendInt32(900);
            Message.AppendInt32(0); //category count

            Message.AppendInt32(friends.Count);
            foreach (MessengerFriend friend in friends)
            {
                friend.Serialize(Message, true);
            }
            return Message;
        }
    }
}
