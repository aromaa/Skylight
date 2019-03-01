using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class PunchCommand : Command
    {
        public override string CommandInfo()
        {
            return ":punch [name] - This command is used to Punch another user.";
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
                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                if (target != null)
                {
                    RoomUnit me = session.GetHabbo().GetRoomSession().GetRoomUser();
                    RoomUnitUser other = target.GetHabbo().GetRoomSession().GetRoomUser();
                    if (target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID == session.GetHabbo().GetRoomSession().CurrentRoomID)
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
                            me.Speak("*Swings for " + target.GetHabbo().Username + " and knocks them out cold*", false);
                            other.ApplyEffect(53, 6);
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
