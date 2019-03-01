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
    class ModeratorGetUserInfoMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_supporttool"))
            {
                uint userId = message.PopWiredUInt();
                if (Skylight.GetGame().GetGameClientManager().GetUsernameByID(userId) != null)
                {
                    session.SendMessage(Skylight.GetGame().GetModerationToolManager().SerializeUserInfo(userId));
                }
                else
                {
                    session.SendNotif("Unable to find user, did you click pet?");
                }
            }
        }
    }
}
