using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Storage;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class QueryCommand : Command
    {
        public override string CommandInfo()
        {
            return ":query [query] - Runs query to DB";
        }

        public override string RequiredPermission()
        {
            return "cmd_query";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            try
            {
                if (session.GetHabbo().HasPermission("cmd_query"))
                {
                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.ExecuteQuery(TextUtilies.MergeArrayToString(args, 1));
                    }
                }
            }
            catch(Exception ex)
            {
                session.SendNotif("Query command failed:\r\n" + ex.ToString());
            }

            return true;
        }
    }
}
