﻿using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class RapeCommand : Command
    {
        public override string CommandInfo()
        {
            return ":rape [user] - Be an badass and commit a crime!";
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
                    RoomUnit other = target.GetHabbo().GetRoomSession().GetRoomUser();
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
                            me.Speak("*Hold hands behind back, pins them down and sticks it inside them*", false);
                            other.Speak("*Screams in pain*", true);
                        }

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
