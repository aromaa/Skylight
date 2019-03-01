using SkylightEmulator.HabboHotel.Items;
using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Utilies
{
    public class OldSchoolUtils
    {
        public static string GetOldSchoolExtraData(RoomItem item)
        {
            if (item is RoomItemGate)
            {
                return item.ExtraData == "1" ? "o" : "c";
            }
            else
            {
                int extraData;
                if (int.TryParse(item.ExtraData, out extraData))
                {
                    return (extraData + 1).ToString();
                }
                else
                {
                    return item.ExtraData;
                }
            }
        }
    }
}
