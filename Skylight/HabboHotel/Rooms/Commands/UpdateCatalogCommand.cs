using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class UpdateCatalogCommand : Command
    {
        public override string CommandInfo()
        {
            return ":update_catalog - Updates the catalog";
        }

        public override string RequiredPermission()
        {
            return "cmd_update_catalog";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (session.GetHabbo().HasPermission("cmd_update_catalog"))
            {
                using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                {
                    Skylight.GetGame().GetCatalogManager().LoadCatalogItems(dbClient);
                    Skylight.GetGame().GetCatalogManager().LoadCatalogPages(dbClient);
                    Skylight.GetGame().GetCatalogManager().LoadPetRaces(dbClient);
                    Skylight.GetGame().GetCatalogManager().LoadPresents(dbClient);
                }

                ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                message.Init(r63aOutgoing.UpdateCatalog);
                byte[] data = message.GetBytes();

                foreach(GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                {
                    session_.SendData(data);
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
