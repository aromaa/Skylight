using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class GiveItemCommand : Command
    {
        public override string CommandInfo()
        {
            return ":giveitem x - Gives your handitem to user";
        }

        public override string RequiredPermission()
        {
            return "";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().GetRoomSession().GetRoomUser().Handitem > 0)
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().ID != session.GetHabbo().ID)
                        {
                            if (target.GetHabbo().GetRoomSession().GetRoom().ID == session.GetHabbo().GetRoomSession().GetRoom().ID && Math.Abs(session.GetHabbo().GetRoomSession().GetRoomUser().X - target.GetHabbo().GetRoomSession().GetRoomUser().X) < 3 && Math.Abs(session.GetHabbo().GetRoomSession().GetRoomUser().Y - target.GetHabbo().GetRoomSession().GetRoomUser().Y) < 3)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().SetHanditem(session.GetHabbo().GetRoomSession().GetRoomUser().Handitem);
                                session.GetHabbo().GetRoomSession().GetRoomUser().SetHanditem(0);
                            }
                        }
                        else
                        {
                            session.GetHabbo().GetRoomSession().GetRoomUser().SetHanditem(session.GetHabbo().GetRoomSession().GetRoomUser().Handitem);
                        }
                    }
                }
                else
                {
                    session.SendNotif("Yo're not holding anything");
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
