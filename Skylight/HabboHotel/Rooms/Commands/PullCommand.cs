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
    class PullCommand : Command
    {
        public override string CommandInfo()
        {
            return ":pull [user] - Pulls the user";
        }

        public override string RequiredPermission()
        {
            return "cmd_pull";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_push"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        RoomUnit me = session.GetHabbo().GetRoomSession().GetRoomUser();
                        RoomUnit other = target.GetHabbo().GetRoomSession().GetRoomUser();
                        if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID == session.GetHabbo().GetRoomSession().CurrentRoomID && (other.RestrictMovementType & RestrictMovementType.Client) == 0)
                        {
                            if (Math.Abs(me.X - other.X) < 3 && Math.Abs(me.Y - other.Y) < 3)
                            {
                                me.Speak("*pulls " + target.GetHabbo().Username + " to them*", false);
                                if (me.HeadRotation == 0)
                                {
                                    other.MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y - 1);
                                }
                                else if (me.HeadRotation == 2)
                                {
                                    other.MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X + 1, session.GetHabbo().GetRoomSession().GetRoomUser().Y);
                                }
                                else if (me.HeadRotation == 4)
                                {
                                    other.MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y + 1);
                                }
                                else if (me.HeadRotation == 6)
                                {
                                    other.MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X - 1, session.GetHabbo().GetRoomSession().GetRoomUser().Y);
                                }
                                else
                                {
                                    other.MoveTo(session.GetHabbo().GetRoomSession().GetRoomUser().X, session.GetHabbo().GetRoomSession().GetRoomUser().Y + 1);
                                }

                                return true;
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
