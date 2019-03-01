using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class Roomvisit
    {
        public readonly uint UserID;
        public readonly uint RoomID;
        public readonly double EntryTimestamp;

        public Roomvisit(uint userId, uint roomId, double entryTimestamp)
        {
            this.UserID = userId;
            this.RoomID = roomId;
            this.EntryTimestamp = entryTimestamp;
        }

        public DateTime GetEntryDate()
        {
            return TimeUtilies.UnixTimestampToDateTime(this.EntryTimestamp);
        }
    }
}
