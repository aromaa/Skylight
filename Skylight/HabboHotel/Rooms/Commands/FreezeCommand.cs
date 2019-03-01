using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.Data.Enums;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class FreezeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":freeze [name] - Freezes the user";
        }

        public override string RequiredPermission()
        {
            return "cmd_freeze";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_freeze"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().GetRoom().ID == session.GetHabbo().GetRoomSession().CurrentRoomID)
                        {
                            if ((target.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType & RestrictMovementType.Client) != 0)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType &= ~RestrictMovementType.Client;
                            }
                            else
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType |= RestrictMovementType.Client;
                            }
                        }
                    }
                    else
                    {

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
