using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Games
{
    public class WSPlayer
    {
        public bool NeedUpdate = false;
        public bool HasUsedResetLean = false;
        public int Lean;
        public int Location;
        public string Action;
        public bool LeftSide;
        public bool BeenHit;
        public int HitsTakenTotal;

        public WSPlayer(int location, bool leftSide)
        {
            this.Location = location;
            this.LeftSide = leftSide;
        }
    }
}
