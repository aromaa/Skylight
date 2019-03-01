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
    class FriendsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message.Init(r26Outgoing.MessengerFriends);
            message.AppendInt32(200);
            message.AppendInt32(200);
            message.AppendInt32(600);
            message.AppendBoolean(false);

            message.AppendInt32(friends.Count);
            foreach (MessengerFriend friend in friends)
            {
                friend.Serialize(message, true);
            }
            message.AppendString("PYH", null); //WTF IS THIS?
            return message;
        }
    }
}
