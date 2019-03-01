using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class PixelsCommand : Command
    {
        public override string CommandInfo()
        {
            return ":pixels [name] [amount] - Gives x amount of pixels to user";
        }

        public override string RequiredPermission()
        {
            return "cmd_pixels";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_pixels"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        int amount = 0;
                        if (int.TryParse(args[2], out amount))
                        {
                            target.GetHabbo().AddActivityPoints(0, amount);
                            target.GetHabbo().UpdateActivityPoints(0, true);
                            target.SendNotif("You just received " + amount + " pixels from staff!");

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
