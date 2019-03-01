using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Games
{
    public class RoomGameboardUser
    {
        public RoomUnitUser User;
        public string Side = "";

        public RoomGameboardUser(RoomUnitUser user)
        {
            this.User = user;
        }
    }
}
