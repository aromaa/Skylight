using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Users
{
    public class UserStats
    {
        public readonly uint HabboID;
        public int RespectReceived;
        public int DailyRespectPointsLeft;
        public int DailyPetRespectPointsLeft;

        public UserStats(uint ID)
        {
            this.HabboID = ID;

            this.LoadStats();
        }

        private void LoadStats()
        {

        }
    }
}
