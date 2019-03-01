using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class EmptyItemsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":emptyitems - Clears your floor & wall items";
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            session.GetHabbo().GetInventoryManager().DeleteAllItems();
            return true;
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }
    }
}
