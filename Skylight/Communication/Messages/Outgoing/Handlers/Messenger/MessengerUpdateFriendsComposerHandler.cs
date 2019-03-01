using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.Handlers.Messenger
{
    public class MessengerUpdateFriendsComposerHandler : OutgoingHandler
    {
        private static readonly List<MessengerCategory> EmptyCategorys = new List<MessengerCategory>(0);
        private static readonly List<MessengerUpdateFriend> EmptyFriends = new List<MessengerUpdateFriend>(0);

        public List<MessengerCategory> FriendCategorys { get; }
        public List<MessengerUpdateFriend> Friends { get; }

        public MessengerUpdateFriendsComposerHandler(List<MessengerCategory> categorys = null, List<MessengerUpdateFriend> friends = null)
        {
            this.FriendCategorys = categorys ?? MessengerUpdateFriendsComposerHandler.EmptyCategorys;
            this.Friends = friends ?? MessengerUpdateFriendsComposerHandler.EmptyFriends;
        }
    }
}
