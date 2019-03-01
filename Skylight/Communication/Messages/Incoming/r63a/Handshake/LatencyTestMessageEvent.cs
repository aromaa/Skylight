﻿using SkylightEmulator.Communication.Headers;
using SkylightEmulator.Core;
using SkylightEmulator.HabboHotel.GameClients;
using SkylightEmulator.Messages;
using SkylightEmulator.Utilies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkylightEmulator.Communication.Messages.Incoming.r63a.Handshake
{
    class LatencyTestMessageEvent : IncomingPacket
    {
        public void Handle(GameClient session, ClientMessage message)
        {
            int id = message.PopWiredInt32();

            ServerMessage message_ = BasicUtilies.GetRevisionServerMessage(Revision.RELEASE63_35255_34886_201108111108);
            message_.Init(r63aOutgoing.LatencyTest);
            message_.AppendInt32(id);
            session.SendMessage(message_);
        }
    }
}
