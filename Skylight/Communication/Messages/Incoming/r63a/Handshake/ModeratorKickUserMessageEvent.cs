using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class ModeratorKickUserMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("cmd_kick"))
            {
                uint userId = message.PopWiredUInt();
                string message_ = message.PopFixedString();

                GameClient target = Skylight.GetGame().GetGameClientManager().GetGameClientById(userId);
                if (target != null && target.GetHabbo() != null && target.GetHabbo().GetRoomSession() != null && target.GetHabbo().GetRoomSession().CurrentRoomID > 0)
                {
                    if (target.GetHabbo().Rank < session.GetHabbo().Rank)
                    {
                        target.GetHabbo().GetRoomSession().GetRoomUser().Room.RoomUserManager.KickUser(target, false);
                        target.SendNotif("Moderator kicked you out of room for reason: " + message_);
                    }
                    else
                    {
                        session.SendNotif("You don't have permissions to kick this user.");
                    }
                }
                else
                {
                    session.SendNotif("User not found or not in room.");
                }
            }
        }
    }
}
