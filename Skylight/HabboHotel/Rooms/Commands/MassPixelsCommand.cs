using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MassPixelsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":masspixels [amount] - Gives x amount of pixels to every online users";
        }

        public override string RequiredPermission()
        {
            return "cmd_masspixels";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_masspixels"))
                {
                    int amount = int.Parse(args[1]);

                    foreach (GameClient session_ in Skylight.GetGame().GetGameClientManager().GetClients())
                    {
                        try
                        {
                            session_.GetHabbo().AddActivityPoints(0, amount);
                            session_.GetHabbo().UpdateActivityPoints(0, true);
                            session_.SendNotif("You just received " + amount + " pixels from staff!");
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
