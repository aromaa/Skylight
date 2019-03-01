using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63a
{
    class MessengerSearchMessageResponse : OutgoingPacket
    {
        public ServerMessage Handle(ValueHolder valueHolder = null)
        {
            List<DataRow> friends = valueHolder.GetValueOrDefault<List<DataRow>>("Friends");
            List<DataRow> randomPeople = valueHolder.GetValueOrDefault<List<DataRow>>("RandomPeople");

            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message.Init(r63aOutgoing.MessengerSearchResult);
            if (friends != null)
            {
                message.AppendInt32(friends.Count);
                foreach (DataRow dataRow in friends)
                {
                    //new MessengerFriend((uint)dataRow["Id"], (int)((long)dataRow["category"]), (string)dataRow["look"], (string)dataRow["motto"], (double)dataRow["last_online"]).Serialize(message, false);
                }
            }
            else
            {
                message.AppendInt32(0);
            }

            if (randomPeople != null)
            {
                message.AppendInt32(randomPeople.Count);
                foreach (DataRow dataRow in randomPeople)
                {
                    //new MessengerFriend((uint)dataRow["Id"], 0, (string)dataRow["look"], (string)dataRow["motto"], (double)dataRow["last_online"]).Serialize(message, false);
                }
            }
            else
            {
                message.AppendInt32(0);
            }
            return message;
        }
    }
}
