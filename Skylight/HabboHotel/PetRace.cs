using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel
{
    public class PetRace
    {
        public readonly int RaceID;
        public readonly int Color1;

        public PetRace(int raceId, int color1)
        {
            this.RaceID = raceId;
            this.Color1 = color1;
        }
    }
}
