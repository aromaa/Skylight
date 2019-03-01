using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class Caution
    {
        public readonly int ID;
        public readonly uint UserID;
        public readonly string Reason;
        public readonly uint AddedByID;
        public readonly double AddedOn;

        public Caution(int id, uint userId, string reason, uint addedById, double addedOn)
        {
            this.ID = id;
            this.UserID = userId;
            this.Reason = reason;
            this.AddedByID = addedById;
            this.AddedOn = addedOn;
        }
    }
}
