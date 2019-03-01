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
    class FollowCommand : Command
    {
        public override string CommandInfo()
        {
            return ":follow [user] - Follows a user even if he isint in your friend list";
        }

        public override string RequiredPermission()
        {
            return "cmd_follow";
        }

        public override bool ShouldBeLogged()
        {
            return false;
        }

        public override bool OnUse(GameClient session, string[] args)
        {
            if (args.Length >= 2)
            {
                if (session.GetHabbo().HasPermission("cmd_follow"))
                {
                    GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientByUsername(args[1]);
                    if (target != null && target.GetHabbo().GetRoomSession().IsInRoom && target.GetHabbo().GetRoomSession().CurrentRoomID != session.GetHabbo().GetRoomSession().CurrentRoomID && !target.GetHabbo().GetUserSettings().HideInRoom)
                    {
                        ServerMessage followFriend = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
                        followFriend.Init(r63aOutgoing.RoomForward);
                        followFriend.AppendBoolean(target.GetHabbo().GetRoomSession().GetRoom().RoomData.IsPublicRoom);
                        followFriend.AppendUInt(target.GetHabbo().GetRoomSession().GetRoom().ID);
                        session.SendMessage(followFriend);
                    }
                    else
                    {
                        session.SendNotif("Unable to follow user");
                    }
                    return true;
                }
            }

            return false;
        }
    }
}
