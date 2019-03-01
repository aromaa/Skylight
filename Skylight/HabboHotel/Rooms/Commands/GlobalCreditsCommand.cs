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
    class GlobalCreditsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":globalcredits [amount] - Gives credits to everyone";
        }

        public override string RequiredPermission()
        {
            return "cmd_globalcredits";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_masscredits"))
                {
                    int amount = int.Parse(args[1]);

                    using (DatabaseClient dbClient = Skylight.GetDatabaseManager().GetClient())
                    {
                        dbClient.ExecuteQuery("UPDATE users SET credits = credits + " + amount);
                    }

                    foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.GetHabbo().Credits += amount;
                            session_.GetHabbo().UpdateCredits(true);
                            session_.SendNotif("You just received " + amount + " credits from staff!");
                        }
                        catch
                        {

                        }
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
