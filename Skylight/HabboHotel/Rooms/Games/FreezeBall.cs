using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Games
{
    public class FreezeBall
    {
        public int Range;
        public FreezeBallType BallType;
        public RoomItem Source;
        public FreezePlayer Player;
        public double Created;

        public FreezeBall(int range, FreezeBallType type, RoomItem source, FreezePlayer player)
        {
            this.Range = range;
            this.BallType = type;
            this.Source = source;
            this.Player = player;
            this.Created = TimeUtilies.GetUnixTimestamp();
        }

        public bool HitGround
        {
            get
            {
                return TimeUtilies.GetUnixTimestamp() - this.Created >= 2.0;
            }
        }
    }
}
