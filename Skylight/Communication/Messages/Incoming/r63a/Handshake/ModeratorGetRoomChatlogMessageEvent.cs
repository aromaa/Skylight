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
    class ModeratorGetRoomChatlogMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            if (session.GetHabbo().HasPermission("acc_chatlogs"))
            {
                int bool_ = message.PopWiredInt32(); //bool
                uint roomId = message.PopWiredUInt();

                session.SendMessage(Skylight.GetGame().GetModerationToolManager().GetRoomChatlog(roomId));
            }
        }
    }
}
