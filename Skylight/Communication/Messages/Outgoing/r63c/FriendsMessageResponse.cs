using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class FriendsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486);
            Message.Init(r63cOutgoing.MessengerFriends);
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
