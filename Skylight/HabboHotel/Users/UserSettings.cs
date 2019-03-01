using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class UserSettings
    {
        private readonly uint ID;
        public bool BlockNewFriends;
        public bool HideOnline;
        public bool HideInRoom;
        public int Volume;
        public bool AcceptTrading;

        public UserSettings(uint id, bool blockNewFriends, bool hideOnline, bool hideInRoom, int volume, bool acceptTrading)
        {
            this.ID = id;
            this.BlockNewFriends = blockNewFriends;
            this.HideOnline = hideOnline;
            this.HideInRoom = hideInRoom;
            this.Volume = volume;
            this.AcceptTrading = acceptTrading;
        }
    }
}
