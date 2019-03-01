using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.HabboHotel.Rooms.Commands
{
    class MimicCommand : Command
    {
        public override string CommandInfo()
        {
            return ":mimic [user] - Coyps the user look";
        }

        public override string RequiredPermission()
        {
            return "cmd_mimic";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_mimic"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        session.GetHabbo().Look = target.GetHabbo().Look;

                        session.GetHabbo().GetRoomSession().GetRoomUser().FootballGateFigureActive = false;

                        ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_.Init(r63aOutgoing.UpdateUser);
                        message_.AppendInt32(-1);
                        message_.AppendString(session.GetHabbo().Look);
                        message_.AppendString(session.GetHabbo().Gender.ToLower());
                        message_.AppendString(session.GetHabbo().Motto);
                        message_.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                        session.SendMessage(message_);

                        ServerMessage message_2 = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message_2.Init(r63aOutgoing.UpdateUser);
                        message_2.AppendInt32(session.GetHabbo().GetRoomSession().GetRoomUser().VirtualID);
                        message_2.AppendString(session.GetHabbo().Look);
                        message_2.AppendString(session.GetHabbo().Gender.ToLower());
                        message_2.AppendString(session.GetHabbo().Motto);
                        message_2.AppendInt32(session.GetHabbo().GetUserStats().AchievementPoints);
                        session.GetHabbo().GetRoomSession().GetRoom().SendToAll(message_2);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
