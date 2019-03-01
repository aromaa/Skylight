using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Core;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a
{
    class OpenConnectionMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            message.PopWiredInt32();
            uint roomId = message.PopWiredUInt();
            
            if (Skylight.GetConfig()["emu.messages.rooms"] == "1")
            {
                Logging.WriteLine("[Rooms] Requesting Public Room [ID: " + roomId + "]");
            }

            session.GetHabbo().GetRoomSession().RequestPrivateRoom(roomId, "");
        }
    }
}
