using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Algorithm
{
    public interface IPathNode
    {
        Boolean IsBlocked(int x, int y, bool lastTile);
    }
}
