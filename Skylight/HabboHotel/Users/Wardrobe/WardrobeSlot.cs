using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Wardrobe
{
    public class WardrobeSlot
    {
        public readonly int SlotID;
        public string Gender;
        public string Look;

        public WardrobeSlot(int slotId, string gender, string look)
        {
            this.SlotID = slotId;
            this.Gender = gender;
            this.Look = look;
        }
    }
}
