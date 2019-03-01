using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MassCreditsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":masscredits [amount] - Gives x amount credits to all users online";
        }

        public override string RequiredPermission()
        {
            return "cmd_masscredits";
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
