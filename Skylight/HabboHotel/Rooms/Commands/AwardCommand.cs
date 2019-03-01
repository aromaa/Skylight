using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class AwardCommand : Command
    {
        public override string CommandInfo()
        {
            return ":award [name] [group] <level> - Gives a achievement";
        }

        public override string RequiredPermission()
        {
            return "cmd_award";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 3)
            {
                if (session.GetHabbo().HasPermission("cmd_award"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (args.Length >= 4)
                        {
                            Skylight.GetGame().GetAchievementManager().AddAchievement(target, args[2], int.Parse(args[3]));
                        }
                        else
                        {
                            Skylight.GetGame().GetAchievementManager().AddAchievement(target, args[2]);
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
            else
            {
                return false;
            }
        }
    }
}
