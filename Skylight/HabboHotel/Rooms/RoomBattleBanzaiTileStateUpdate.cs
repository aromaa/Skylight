using SkylightEmulator.HabboHotel.Rooms.Games;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms
{
    public class RoomBattleBanzaiTileStateUpdate
    {
        public int X;
        public int Y;
        public GameTeam GameTeam;

        public RoomBattleBanzaiTileStateUpdate(int x, int y, GameTeam gameTeam)
        {
            this.X = x;
            this.Y = y;
            this.GameTeam = gameTeam;
        }
    }
}
