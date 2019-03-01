﻿using SkylightEmulator.Communication.Headers;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r26.Handshake
{
    class GetCreditsMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            session.GetHabbo().UpdateCredits(false);

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.R26_20080915_0408_7984_61ccb5f8b8797a3aba62c1fa2ca80169);
            message_.Init(r26Outgoing.CameraFilm);
            message_.AppendInt32(99);
            session.SendMessage(message_);
        }
    }
}
