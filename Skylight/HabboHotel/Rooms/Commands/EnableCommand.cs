using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class EnableCommand : Command
    {
        public override string CommandInfo()
        {
            return ":enable [id] - Gives the effect by id";
        }

        public override string RequiredPermission()
        {
            return "cmd_enable";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_enable"))
                {
                    int effectId = -1;
                    int.TryParse(args[1], out effectId);

                    if (ServerConfiguration.BlacklistedEffects.ContainsKey(effectId) && ServerConfiguration.BlacklistedEffects[effectId] > session.GetHabbo().Rank)
                    {
                        session.SendNotif("You do not have permissions to use this effect!");
                    }
                    else
                    {
                        session.GetHabbo().GetRoomSession().GetRoomUser().ApplyEffect(effectId);
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (args.Length >= 1)
            {
                session.GetHabbo().GetRoomSession().GetRoomUser().ApplyEffect(-1);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
