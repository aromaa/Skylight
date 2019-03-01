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
    class SpullCommand : Command
    {
        public override string CommandInfo()
        {
            return ":spull [user] - Pulls the user from anywhere to you";
        }

        public override string RequiredPermission()
        {
            return "cmd_spull";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_spull"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID == session.GetHabbo().GetRoomSession().CurrentRoomID && (target.GetHabbo().GetRoomSession().GetRoomUser().RestrictMovementType & RestrictMovementType.Client) == 0)
                        {
                            session.GetHabbo().GetRoomSession().GetRoomUser().Speak("*pulls " + target.GetHabbo().Username + " to them*", false);
                            if (session.GetHabbo().GetRoomSession().GetRoomUser().HeadRotation == 0)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y - 1);
                            }
                            else if (session.GetHabbo().GetRoomSession().GetRoomUser().HeadRotation == 2)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X + 1, session.GetHabbo().GetRoomSession().GetRoomUser().Y);
                            }
                            else if (session.GetHabbo().GetRoomSession().GetRoomUser().HeadRotation == 4)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y + 1);
                            }
                            else if (session.GetHabbo().GetRoomSession().GetRoomUser().HeadRotation == 6)
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X - 1, session.GetHabbo().GetRoomSession().GetRoomUser().Y);
                            }
                            else
                            {
                                target.GetHabbo().GetRoomSession().GetRoomUser().MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y + 1);
                            }
                        }
                    }

                    return true;
                }
            }

            return false;
        }
    }
}
