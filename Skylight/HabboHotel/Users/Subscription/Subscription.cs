using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users.Subscription
{
    public class Subscription
    {
        public readonly int ID;
        private string Name;
        private double Started;
        private double Expires;

        public Subscription(int id, string name, double started, double expires)
        {
            this.ID = id;
            this.Name = name;
            this.Started = started;
            this.Expires = expires;
        }

        public bool IsActive()
        {
            return this.Expires >= TimeUtilies.GetUnixTimestamp();
        }

        public void Expand(double secounds)
        {
            this.Expires += secounds;
        }

        public void End()
        {
            this.Expires = TimeUtilies.GetUnixTimestamp();
        }

        public double GetExpires()
        {
            return this.Expires;
        }

        public double SecoundsLeft()
        {
            return this.Expires - TimeUtilies.GetUnixTimestamp();
        }

        public int DaysLeft()
        {
            return (int)Math.Ceiling(this.SecoundsLeft() / 86400.0);
        }

        public double GetStarted()
        {
            return this.Started;
        }
    }
}
