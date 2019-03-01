using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Badges
{
    public class Badge
    {
        public readonly string BadgeID;
        public int SlotID;

        public Badge(string badgeId, int slot)
        {
            this.BadgeID = badgeId;
            this.SlotID = slot;
        }
    }
}
