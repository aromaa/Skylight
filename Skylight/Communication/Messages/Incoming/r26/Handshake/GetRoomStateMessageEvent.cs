using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetRoomStateMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message) //on r26 we only check is the room full or no and leave the current room if any
        {
            message.ReadBytes(1); //wot
            uint roomId = message.PopWiredUInt();

            if (Skylight.GetConfig()["emu.messages.rooms"] == "1")
            {
                Logging.WriteLine("[Rooms] Requesting Private Room [ID: " + roomId + "]");
            }

            session.GetHabbo().GetRoomSession().GetRoomState(roomId);
        }
    }
}
