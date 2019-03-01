using SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.Users.Messenger;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class MessengerFriendsComposer<T> : OutgoingHandlerPacket where T : MessengerFriendsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerFriends);
            message.AppendInt32(300); //idk
            message.AppendInt32(300); //idk

            message.AppendInt32(handler.Friends.Count);
            foreach(MessengerFriend friend in handler.Friends)
            {
                message.AppendUInt(friend.ID);
                message.AppendString(friend.Username);
                message.AppendInt32(0); //gender, ye, its int
                message.AppendBoolean(friend.IsOnline);
                message.AppendBoolean(friend.InRoom);
                message.AppendString(friend.Look);
                message.AppendInt32(friend.Category);
                message.AppendString(friend.Motto);
                message.AppendString(""); //real name
                message.AppendString(""); //un used
                message.AppendBoolean(false); //offline messages enabled
                message.AppendBoolean(true); //idk
                message.AppendBoolean(false); //pocked habbo
                message.AppendShort((int)friend.Relation);
            }
            return message;
        }
    }
}
