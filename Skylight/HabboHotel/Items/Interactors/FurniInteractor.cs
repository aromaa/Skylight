using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.HabboHotel.Rooms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Items.Interactors
{
    public abstract class FurniInteractor
    {
        public abstract void OnUse(GameClient session, RoomItem item, int request, bool userHasRights);
    }
}
