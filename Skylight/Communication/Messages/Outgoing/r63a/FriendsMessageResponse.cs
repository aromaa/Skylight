using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class FriendsMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<MessengerCategory> categorys = valueHolder.GetValueOrDefault<List<MessengerCategory>>("Categorys");
            List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            Message.Init(r63aOutgoing.MessengerFriends);
            Message.AppendInt32(6000);
            Message.AppendInt32(200);
            Message.AppendInt32(6000);
            Message.AppendInt32(900);
            if (categorys != null)
            {
                Message.AppendInt32(categorys.Count);
                foreach (MessengerCategory category in categorys)
                {
                    Message.AppendInt32(category.Id);
                    Message.AppendString(category.Name);
                }
            }
            else
            {
                Message.AppendInt32(0);
            }

            Message.AppendInt32(friends.Count);
            foreach (MessengerFriend friend in friends)
            {
                friend.Serialize(Message, true);
            }
            return Message;
        }
    }
}
