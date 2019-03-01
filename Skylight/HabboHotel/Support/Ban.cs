using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Support
{
    public class Ban
    {
        public readonly int ID;
        public readonly BanType BanType;
        public readonly string Value;
        public readonly string Reason;
        public readonly double Expires;
        public readonly uint AddedByID;
        public readonly double AddedOn;
        public readonly bool Active;

        public Ban(int id, BanType banType, string value, string reason, double expires, uint addedById, double addedOn, bool active)
        {
            this.ID = id;
            this.BanType = banType;
            this.Value = value;
            this.Reason = reason;
            this.Expires = expires;
            this.AddedByID = addedById;
            this.AddedOn = addedOn;
            this.Active = active;
        }

        public bool Permament
        {
            get
            {
                return this.Expires == -1;
            }
        }

        public bool Expired
        {
            get
            {
                return !this.Permament && TimeUtilies.GetUnixTimestamp() >= this.Expires;
            }
        }
    }
}
