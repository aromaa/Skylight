using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Outgoing.r63c
{
    class RequestRoomMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            uint roomId = message.PopWiredUInt();
            string password = message.PopFixedString();

            if (Skylight.GetConfig()["emu.messages.rooms"] == "1")
            {
                Logging.WriteLine("[Rooms] Requesting Private Room [ID: " + roomId + "]");
            }

            session.GetHabbo().GetRoomSession().RequestPrivateRoom(roomId, password);
        }
    }
}
