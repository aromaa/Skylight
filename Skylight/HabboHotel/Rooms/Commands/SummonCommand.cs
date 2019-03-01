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
    class SummonCommand : Command
    {
        public override string CommandInfo()
        {
            return ":summon [user] - Summons the user to the same room";
        }

        public override string RequiredPermission()
        {
            return "cmd_summon";
        }

        public override bool ShouldBeLogged()
        {
            return true;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_summon"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null)
                    {
                        ServerMessage message = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        message.Init(r63aOutgoing.RoomForward);
                        message.AppendBoolean(session.GetHabbo().GetRoomSession().GetRoom().RoomData.IsPublicRoom);
                        message.AppendUInt(session.GetHabbo().GetRoomSession().CurrentRoomID);
                        target.SendMessage(message);

                        target.SendNotif(session.GetHabbo().Username + " has summoned you to them!");
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
