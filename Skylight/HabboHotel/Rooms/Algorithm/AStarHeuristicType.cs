using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Algorithm
{
    public enum AStarHeuristicType
    {
        FAST_SEARCH = 0,
        BETWEEN = 2,
        SHORTEST_PATH = 3,
        EXPERIMENTAL_SEARCH,
    }
}
