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
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.HabboHotel.Data.Data;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class MessengerUpdateFriendsComposer<T> : OutgoingHandlerPacket where T : MessengerUpdateFriendsComposerHandler
    {
        public override ServerMessage Handle(OutgoingHandler handler)
        {
            return this.Handle((T)handler);
        }

        public ServerMessage Handle(T handler)
        {
            ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.PRODUCTION_201601012205_226667486, r63cOutgoing.MessengerUpdateFriends);
            message.AppendInt32(handler.FriendCategorys.Count);
            foreach(MessengerCategory category in handler.FriendCategorys)
            {
                message.AppendInt32(category.Id);
                message.AppendString(category.Name);
            }

            message.AppendInt32(handler.Friends.Count);
            foreach(MessengerUpdateFriend friend in handler.Friends)
            {
                message.AppendInt32(friend.StatusID);

                MessengerUpdateFriendRemove remove = friend as MessengerUpdateFriendRemove;
                if (remove != null)
                {
                    message.AppendUInt(remove.UserID);
                }
                else
                {
                    MessengerFriend friend_ = (friend as MessengerUpdateFriendUpdate)?.Friend ?? (friend as MessengerUpdateFriendAdd)?.Friend;

                    message.AppendUInt(friend_.ID);
                    message.AppendString(friend_.Username);
                    message.AppendInt32(0); //gender, ye, its int
                    message.AppendBoolean(friend_.IsOnline);
                    message.AppendBoolean(friend_.InRoom);
                    message.AppendString(friend_.Look);
                    message.AppendInt32(friend_.Category);
                    message.AppendString(friend_.Motto);
                    message.AppendString(""); //real name
                    message.AppendString(""); //un used
                    message.AppendBoolean(false); //offline messages enabled
                    message.AppendBoolean(true); //idk
                    message.AppendBoolean(false); //pocked habbo
                    message.AppendShort((int)friend_.Relation);
                }
            }

            return message;
        }
    }
}
