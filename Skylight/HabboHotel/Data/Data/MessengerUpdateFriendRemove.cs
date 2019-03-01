using SkylightEmulator.HabboHotel.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Data.Data
{
    public class MessengerUpdateFriendRemove : MessengerUpdateFriend
    {
        public uint UserID { get; }

        public MessengerUpdateFriendRemove(uint userId)
        {
            this.UserID = userId;
        }

        public int StatusID => -1;
    }
}
