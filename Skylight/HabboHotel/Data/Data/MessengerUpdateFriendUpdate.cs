using SkylightEmulator.HabboHotel.Data.Interfaces;
using SkylightEmulator.HabboHotel.Users.Messenger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Data.Data
{
    public class MessengerUpdateFriendUpdate : MessengerUpdateFriend
    {
        public MessengerFriend Friend { get; }

        public MessengerUpdateFriendUpdate(MessengerFriend friend)
        {
            this.Friend = friend;
        }

        public int StatusID => 0;
    }
}
