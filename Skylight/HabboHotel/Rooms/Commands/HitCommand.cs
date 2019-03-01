using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class HitCommand : Command
    {
        public override string CommandInfo()
        {
            return "";
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
            if (session.GetHabbo().GetRoomSession().GetRoom().RoomData.ExtraData.RoleplayEnabled)
            {
                if (args.Length >= 2)
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        RoomUnitUser me = session.GetHabbo().GetRoomSession().GetRoomUser();
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
                                int damage = RandomUtilies.GetRandom(5, 12);
                                other.Health -= damage;
                                if (other.Health - damage < 0)
                                {
                                    me.Speak("*Hits " + target.GetHabbo().Username + ", and kills them!*", false);
                                    other.Speak("*dies*", false);
                                    session.GetHabbo().GetRoomSession().GetRoom().RoomUserManager.KickUser(target, true);
                                }
                                else
                                {
                                    me.Speak("*Hits " + target.GetHabbo().Username + ", causing " + damage + " damage *", false);
                                    other.Speak("*Suffers " + damage + " damage from " + me.Session.GetHabbo().Username + ", leaving me with " + other.Health + " health*", false);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                session.SendNotif("RP is disabled on this room!");
            }

            return true;
        }
    }
}
