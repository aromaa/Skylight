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
    class SpushCommand : Command
    {
        public override string CommandInfo()
        {
            return ":spush [user] - Push user 5 blocks";
        }

        public override string RequiredPermission()
        {
            return "cmd_spush";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_spush"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        RoomUnit me = session.GetHabbo().GetRoomSession().GetRoomUser();
                        RoomUnit other = target.GetHabbo().GetRoomSession().GetRoomUser();
                        if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID == session.GetHabbo().GetRoomSession().CurrentRoomID && (other.RestrictMovementType & RestrictMovementType.Client) == 0)
                        {
                            bool doit = true;
                            if ((me.X + 1 != other.X || me.Y != other.Y) && (me.X - 1 != other.X || me.Y != other.Y) && (me.Y + 1 != other.Y || me.X != other.X))
                            {
                                bool skip = false;
                                if (me.X - 1 == other.X)
                                {
                                    if (me.Y == other.Y)
                                    {
                                        skip = true;
                                    }
                                }

                                if (!skip)
                                {
                                    doit = me.X == other.X || me.Y == other.Y;
                                }
                            }

                            if (doit)
                            {
                                me.Speak("*pushes " + target.GetHabbo().Username + "*", false);
                                if (me.HeadRotation == 0)
                                {
                                    for (int i = 1; i <= 5; i++)
                                    {
                                        other.MoveTo(target.GetHabbo().GetRoomSession().GetRoomUser().X, target.GetHabbo().GetRoomSession().GetRoomUser().Y - i);
                                    }
                                }
                                else if (me.HeadRotation == 2)
                                {
                                    for (int i = 1; i <= 5; i++)
                                    {
                                        other.MoveTo(target.GetHabbo().GetRoomSession().GetRoomUser().X + i, target.GetHabbo().GetRoomSession().GetRoomUser().Y);
                                    }
                                }
                                else if (me.HeadRotation == 4)
                                {
                                    for (int i = 1; i <= 5; i++)
                                    {
                                        other.MoveTo(target.GetHabbo().GetRoomSession().GetRoomUser().X, target.GetHabbo().GetRoomSession().GetRoomUser().Y + i);
                                    }
                                }
                                else if (me.HeadRotation == 6)
                                {
                                    for (int i = 1; i <= 5; i++)
                                    {
                                        other.MoveTo(target.GetHabbo().GetRoomSession().GetRoomUser().X - i, target.GetHabbo().GetRoomSession().GetRoomUser().Y);
                                    }
                                }
                                else
                                {
                                    for (int i = 1; i <= 5; i++)
                                    {
                                        other.MoveTo(target.GetHabbo().GetRoomSession().GetRoomUser().X, target.GetHabbo().GetRoomSession().GetRoomUser().Y + i);
                                    }
                                }
                            }

                            return true;
                        }
                    }
                }
            }

            return false;
        }
    }
}
