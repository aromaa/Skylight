using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UpdateItemsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_items - Update the items";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_items";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_items"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    Skylight.GetGame().GetItemManager().LoadItems(dbClient);
                    Skylight.GetGame().GetItemManager().LoadSoundtracks(dbClient);
                    Skylight.GetGame().GetItemManager().LoadNewbieRoomItems(dbClient);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
