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
    class MessengerUpdateMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            int type = valueHolder.GetValueOrDefault<int>("Type");
            List<MessengerCategory> categorys = valueHolder.GetValueOrDefault<List<MessengerCategory>>("Categorys");

            ServerMessage Message = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            Message.Init(r63aOutgoing.MeesengerUpdate);
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

            if (type != -1)
            {
                List<MessengerFriend> friends = valueHolder.GetValue<List<MessengerFriend>>("Friends");

                Message.AppendInt32(friends.Count);
                foreach (MessengerFriend friend in friends)
                {
                    Message.AppendInt32(type); //type
                    friend.Serialize(Message, true);
                }
            }
            else
            {
                List<uint> friends = valueHolder.GetValue<List<uint>>("Friends");

                Message.AppendInt32(friends.Count);
                foreach (uint friend in friends)
                {
                    Message.AppendInt32(type); //type
                    Message.AppendUInt(friend);
                }
            }
            return Message;
        }
    }
}
