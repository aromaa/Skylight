using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Games
{
    public class FreezeBallHit
    {
        public readonly long ID;
        public readonly Point Point;
        public readonly FreezePlayer Player;
        public int Direction;
        public int Ticks;

        public FreezeBallHit(long id, Point point, FreezePlayer player, int direction, int ticks)
        {
            this.ID = id;
            this.Point = point;
            this.Player = player;
            this.Direction = direction;
            this.Ticks = ticks;
        }
    }
}
